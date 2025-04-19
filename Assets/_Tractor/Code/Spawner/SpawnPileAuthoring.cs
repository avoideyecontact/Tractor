using Unity.Entities;
using UnityEngine;

public class SpawnPileAuthoring : MonoBehaviour
{
    public GameObject prefab;
    public int quantity;
    public float scale;
}

class SpawnPileBaker : Baker<SpawnPileAuthoring>
{
    public override void Bake(SpawnPileAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);

        AddComponent(entity, new SpawnPileComponent
        {
            prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
            quantity = authoring.quantity,
            position = authoring.transform.position,
            scale = authoring.scale
        });
    }
}
