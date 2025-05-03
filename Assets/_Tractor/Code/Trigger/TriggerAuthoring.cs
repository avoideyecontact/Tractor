using Unity.Entities;
using UnityEngine;

public class TriggerAuthoring : MonoBehaviour
{
    class TriggerBaker : Baker<TriggerAuthoring>
    {
        public override void Bake(TriggerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<TriggerTag>(entity);
        }
    }
}
