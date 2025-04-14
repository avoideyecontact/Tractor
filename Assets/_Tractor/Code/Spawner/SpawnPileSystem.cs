using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct SpawnPileSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (!SystemAPI.TryGetSingletonEntity<SpawnPileComponent>(out Entity spawnerEntity))
            return;

        RefRW<SpawnPileComponent> spawner = SystemAPI.GetComponentRW<SpawnPileComponent>(spawnerEntity);

        if (spawner.ValueRO.done)
            return;

        EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);

        for (int i = 0; i < spawner.ValueRO.quantity; i++)
        {
            for (int j = 0; j < spawner.ValueRO.quantity; j++)
            {
                for (int z = 0; z < spawner.ValueRO.quantity; z++)
                {
                    Entity newEntity = ecb.Instantiate(spawner.ValueRO.prefab);
                    ecb.AddComponent(newEntity, new GoodTag { });
                    ecb.AddComponent(newEntity, new LocalTransform
                    {
                        Position = (new float3(i, j + spawner.ValueRO.scale, z) + spawner.ValueRO.position) * spawner.ValueRO.scale,
                        Rotation = Quaternion.identity,
                        Scale = spawner.ValueRO.scale
                    });
                }
            }
        }

        spawner.ValueRW.done = true;
        ecb.Playback(state.EntityManager);
    }
}
