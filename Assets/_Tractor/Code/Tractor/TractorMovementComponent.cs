using Unity.Entities;

public struct TractorMovementComponent : IComponentData
{
    public float movementForce;
    public float basePosY;
}
