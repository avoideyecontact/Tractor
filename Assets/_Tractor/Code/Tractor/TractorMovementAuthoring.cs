using Unity.Entities;
using UnityEngine;

public class TractorMovementAuthoring : MonoBehaviour
{
    public float movementForce;
    public float basePosY;
}

class TractorMovementBaker : Baker<TractorMovementAuthoring>
{
    public override void Bake(TractorMovementAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);

        AddComponent(entity, new TractorMovementComponent
        {
            movementForce = authoring.movementForce,
            basePosY = authoring.basePosY
        });
    }
}
