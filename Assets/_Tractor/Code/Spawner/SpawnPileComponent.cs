using Unity.Entities;
using Unity.Mathematics;

public struct SpawnPileComponent : IComponentData
{
    public int quantity;
    public float3 position;
    public float scale;
}
