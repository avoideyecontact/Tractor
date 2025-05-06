using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

[BurstCompile]
public partial struct DestroySystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<DestroyRequest>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Allocator.TempJob);
        var ecbParallel = ecb.AsParallelWriter();

        var processJob = new DestroyJob
        {
            ecb = ecbParallel,
        };

        state.Dependency = processJob.ScheduleParallel(state.Dependency);
        state.Dependency.Complete();

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    [BurstCompile]
    [WithAll(typeof(DestroyRequest))]
    public partial struct DestroyJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;

        [BurstCompile]
        public void Execute(Entity entity, [EntityIndexInQuery] int sortKey)
        {
            ecb.DestroyEntity(sortKey, entity);
        }
    }
}
