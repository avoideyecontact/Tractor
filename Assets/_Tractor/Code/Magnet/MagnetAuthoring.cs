using Unity.Entities;
using UnityEngine;

public class MagnetAuthoring : MonoBehaviour
{
    public GameObject magnetPoint;
    public float magnetForce;
    public bool isEnabled;
}

class MagnetBaker : Baker<MagnetAuthoring>
{
    public override void Bake(MagnetAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);

        AddComponent(entity, new MagnetComponent
        {
            magnetPoint = GetEntity(authoring.magnetPoint, TransformUsageFlags.Dynamic),
            magnetForce = authoring.magnetForce,
            isEnabled = authoring.isEnabled
        });
    }
}
