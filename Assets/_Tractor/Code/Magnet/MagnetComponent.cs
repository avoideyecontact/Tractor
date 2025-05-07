using Unity.Entities;

public struct MagnetComponent : IComponentData
{
    public Entity magnetPoint;
    public float magnetForce;
    public bool isEnabled;
}
