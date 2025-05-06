using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
public partial struct ScoreSystem : ISystem
{
    private NativeArray<int> _eventCounter;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ScorePointTag>();
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

        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var ecbParallel = ecb.AsParallelWriter();

        var scoreEntity = SystemAPI.GetSingletonEntity<ScoreComponent>();
        var currentScore = SystemAPI.GetComponent<ScoreComponent>(scoreEntity).Score;       

        var processJob = new UpdateScoreJob
        {
            ecb = ecbParallel,
            eventCounter = _eventCounter,
            scoreEntity = scoreEntity,
            currentScore = currentScore
        };

        state.Dependency = processJob.ScheduleParallel(state.Dependency);
        state.Dependency.Complete();

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    [WithAll(typeof(ScorePointTag))]
    public partial struct UpdateScoreJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;
        public Entity scoreEntity;
        public int currentScore;

        [NativeDisableParallelForRestriction]
        public NativeArray<int> eventCounter;

        [BurstCompile]
        public void Execute(Entity entity, [EntityIndexInQuery] int sortKey)
        {
            eventCounter[0] += 1;

            ecb.SetComponent(sortKey, scoreEntity, new ScoreComponent
            {
                Score = currentScore + eventCounter[0]
            });

            ecb.AddComponent(sortKey, entity, new DestroyRequest());
        }
    }
}
