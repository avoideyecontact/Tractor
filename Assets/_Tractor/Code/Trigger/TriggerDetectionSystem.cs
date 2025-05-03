using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
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

        var scoreEntity = SystemAPI.GetSingletonEntity<ScoreComponent>();
        var currentScore = SystemAPI.GetComponent<ScoreComponent>(scoreEntity).Score;

        state.Dependency = new TriggerJob
        {
            ecb = ecb,
            scoreEntity = scoreEntity,
            currentScore = currentScore,
            triggerComponents = SystemAPI.GetComponentLookup<TriggerTag>(true),
            goodComponents = SystemAPI.GetComponentLookup<GoodTag>(true),
            existsLookup = SystemAPI.GetEntityStorageInfoLookup()
        }.Schedule(simulation, state.Dependency);
    }

    [BurstCompile]
    struct TriggerJob : ITriggerEventsJob
    {
        public EntityCommandBuffer ecb;
        public Entity scoreEntity;
        public int currentScore;
        [ReadOnly] public ComponentLookup<TriggerTag> triggerComponents;
        [ReadOnly] public ComponentLookup<GoodTag> goodComponents;
        [ReadOnly] public EntityStorageInfoLookup existsLookup;

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
                ecb.AddComponent(scoreEntity, new ScoreComponent
                {
                    Score = currentScore + 1
                });

                ecb.DestroyEntity(otherEntity);
            }
        }
    }
}
