using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile]
public partial struct TriggerDetectionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();

        state.Dependency = new TriggerJob
        {
            ecb = ecb,
            triggerComponents = SystemAPI.GetComponentLookup<TriggerTag>(true),
            goodComponents = SystemAPI.GetComponentLookup<GoodTag>(true),
            existsLookup = SystemAPI.GetEntityStorageInfoLookup()
        }.Schedule(simulation, state.Dependency);
    }

    [BurstCompile]
    struct TriggerJob : ITriggerEventsJob
    {
        public EntityCommandBuffer ecb;
        [ReadOnly] public ComponentLookup<TriggerTag> triggerComponents;
        [ReadOnly] public ComponentLookup<GoodTag> goodComponents;
        [ReadOnly] public EntityStorageInfoLookup existsLookup;

        [BurstCompile]
        public void Execute(TriggerEvent triggerEvent)
        {
            bool isA = triggerComponents.HasComponent(triggerEvent.EntityA);
            bool isB = triggerComponents.HasComponent(triggerEvent.EntityB);

            if (isA == isB) return;

            Entity triggerEntity = isA ? triggerEvent.EntityA : triggerEvent.EntityB;
            Entity otherEntity = isA ? triggerEvent.EntityB : triggerEvent.EntityA;

            if (existsLookup.Exists(otherEntity) &&
               goodComponents.HasComponent(otherEntity))
            {
                ecb.AddComponent(otherEntity, new ScorePointTag());
            }
        }
    }
}
