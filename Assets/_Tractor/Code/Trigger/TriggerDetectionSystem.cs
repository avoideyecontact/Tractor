using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[BurstCompile]
public partial struct TriggerDetectionSystem : ISystem
{
    private NativeArray<int> _eventCounter;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
        _eventCounter = new NativeArray<int>(1, Allocator.Persistent);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
        if (_eventCounter.IsCreated)
            _eventCounter.Dispose();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _eventCounter[0] = 0;

        var ecb = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);

        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();

        var scoreEntity = SystemAPI.GetSingletonEntity<ScoreComponent>();
        var currentScore = SystemAPI.GetComponent<ScoreComponent>(scoreEntity).Score;


        state.Dependency = new TriggerJob
        {
            ecb = ecb,
            eventCounter = _eventCounter,
            triggerComponents = SystemAPI.GetComponentLookup<TriggerTag>(true),
            goodComponents = SystemAPI.GetComponentLookup<GoodTag>(true),
            existsLookup = SystemAPI.GetEntityStorageInfoLookup()
        }.Schedule(simulation, state.Dependency);

        state.Dependency = new UpdateScoreJob
        {
            ecb = ecb,
            eventCounter = _eventCounter,
            scoreEntity = scoreEntity,
            currentScore = currentScore
        }.Schedule(state.Dependency);
    }

    [BurstCompile]
    struct TriggerJob : ITriggerEventsJob
    {
        public EntityCommandBuffer ecb;
        [ReadOnly] public ComponentLookup<TriggerTag> triggerComponents;
        [ReadOnly] public ComponentLookup<GoodTag> goodComponents;
        [ReadOnly] public EntityStorageInfoLookup existsLookup;

        [NativeDisableParallelForRestriction]
        public NativeArray<int> eventCounter;

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
                eventCounter[0] += 1;
                ecb.DestroyEntity(otherEntity);
            }
        }
    }

    // todo: разделить на GoodsCollectSystem ScoreSystem DestroySystem
    [BurstCompile]
    struct UpdateScoreJob : IJob
    {
        public EntityCommandBuffer ecb;
        public Entity scoreEntity;
        public int currentScore;
        [ReadOnly] public NativeArray<int> eventCounter;

        public void Execute()
        {
            ecb.SetComponent(scoreEntity, new ScoreComponent
            {
                Score = currentScore + eventCounter[0]
            });
        }
    }
}
