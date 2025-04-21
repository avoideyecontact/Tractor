using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Extensions;
using Unity.Physics;
using UnityEngine;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile]
public partial struct TractorMovementSystem : ISystem
{
    private float3 _inputDirection;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<TractorMovementComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingleton<TractorInputComponent>(out var input)) return;

        var job = new TractorMovementJob
        {
            DeltaTime = SystemAPI.Time.DeltaTime,
            Input = input
        };

        job.Schedule();
    }

    [BurstCompile]
    public partial struct TractorMovementJob : IJobEntity
    {
        public float DeltaTime;
        public TractorInputComponent Input;

        [BurstCompile]
        public void Execute(
            ref PhysicsVelocity velocity,
            ref LocalTransform transform,
            in PhysicsMass mass,
            in TractorMovementComponent tractor)
        {

            // Movement
            var forward = math.mul(transform.Rotation, math.forward());
            var linear = forward * (Input.Vertical * tractor.moveSpeed);
            velocity.ApplyLinearImpulse(in mass, linear * DeltaTime);

            // Rotation
            var angular = new float3(0, Input.Horizontal * tractor.rotationSpeed, 0);
            velocity.ApplyAngularImpulse(in mass, angular * DeltaTime);

            // Pos correction
            transform.Position.y = tractor.baseHeight;

            // Angle correction
            Quaternion quaternion = transform.Rotation;
            float3 euler = quaternion.eulerAngles;
            transform.Rotation = Quaternion.Euler(0f, euler.y, 0f);
        }
    }
}
