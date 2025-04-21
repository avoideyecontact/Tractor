using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct SpawnPileSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SpawnPileComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var Ecb = new EntityCommandBuffer(Allocator.TempJob);
        var ecbParallel = Ecb.AsParallelWriter();

        var processJob = new SpawnAndDeleteJob
        {
            ecb = ecbParallel,
        };

        state.Dependency = processJob.ScheduleParallel(state.Dependency);
        state.Dependency.Complete();

        Ecb.Playback(state.EntityManager);
        Ecb.Dispose();
    }

    [BurstCompile]
    [WithAll(typeof(SpawnPileComponent))]
    public partial struct SpawnAndDeleteJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecb;

        public void Execute(Entity entity, [EntityIndexInQuery] int sortKey, ref SpawnPileComponent spawner)
        {
            for (int i = 0; i < spawner.quantity; i++)
            {
                for (int j = 0; j < spawner.quantity; j++)
                {
                    for (int z = 0; z < spawner.quantity; z++)
                    {
                        int newSortKey = sortKey + (i * 100 + j * 10 + z) * spawner.quantity;
                        Entity newEntity = ecb.Instantiate(newSortKey, spawner.prefab);
                        ecb.AddComponent(newSortKey, newEntity, new GoodTag { });
                        ecb.AddComponent(newSortKey, newEntity, new LocalTransform
                        {
                            Position = (new float3(i, j + spawner.scale, z) + spawner.position) * spawner.scale,
                            Rotation = quaternion.identity,
                            Scale = spawner.scale
                        });
                    }
                }
            }

            ecb.DestroyEntity(sortKey, entity);
        }
    }
}
