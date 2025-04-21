using Unity.Entities;

public struct TractorMovementComponent : IComponentData
{
    public float moveSpeed;
    public float rotationSpeed;
    public float baseHeight;
}
