using Unity.Entities;
using UnityEngine;

public class SpawnPileAuthoring : MonoBehaviour
{
    //public GameObject prefab;
    public int quantity;
    public float scale;
    public GameObject[] gameObjects;
}

class SpawnPileBaker : Baker<SpawnPileAuthoring>
{
    public override void Bake(SpawnPileAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);

        AddComponent(entity, new SpawnPileComponent
        {
            quantity = authoring.quantity,
            position = authoring.transform.position,
            scale = authoring.scale
        });

        DynamicBuffer<SpawnerBufferElement> buffer = AddBuffer<SpawnerBufferElement>(entity);

        foreach (var go in authoring.gameObjects)
        {
            if (go != null)
            {
                buffer.Add(new SpawnerBufferElement
                {
                    Value = GetEntity(go, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}
