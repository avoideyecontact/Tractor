using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public partial struct SingleMagnetSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MagnetComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<MagnetComponent>(out var magnetEntity))
            return;

        var magnet = SystemAPI.GetComponent<MagnetComponent>(magnetEntity);

        if (!magnet.isEnabled)
            return;

        var magnetPoint = SystemAPI.GetComponent<LocalToWorld>(magnet.magnetPoint).Position;

        var deltaTime = SystemAPI.Time.DeltaTime;

        new MagnetForceJob
        {
            MagnetPoint = magnetPoint,
            MagnetForce = magnet.magnetForce,
            DeltaTime = deltaTime
        }.ScheduleParallel();
    }

    [BurstCompile]
    partial struct MagnetForceJob : IJobEntity
    {
        [ReadOnly] public float3 MagnetPoint;
        public float MagnetForce;
        public float DeltaTime;

        public void Execute(
            ref PhysicsVelocity velocity,
            in LocalTransform transform,
            in GoodTag tag)
        {
            float3 direction = MagnetPoint - transform.Position;
            float distance = math.length(direction);

            if (distance < 0.1f) return;

            float3 force = math.normalize(direction) * MagnetForce * DeltaTime;

            velocity.Linear += force;
        }
    }
}
