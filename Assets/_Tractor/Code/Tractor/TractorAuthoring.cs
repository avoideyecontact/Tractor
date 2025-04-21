using Unity.Entities;
using UnityEngine;

public class TractorAuthoring : MonoBehaviour
{
    public float moveSpeed;
    public float rotationSpeed;
    public float baseHeight;
}

class TractorBaker : Baker<TractorAuthoring>
{
    public override void Bake(TractorAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);

        AddComponent(entity, new TractorInputComponent
        {
            Horizontal = 0,
            Vertical = 0
        });
        AddComponent(entity, new TractorMovementComponent
        {
            moveSpeed = authoring.moveSpeed,
            rotationSpeed = authoring.rotationSpeed,
            baseHeight = authoring.baseHeight,
        });
    }
}
