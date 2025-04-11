using Unity.Entities;
using Unity.Physics;
using UnityEngine;

public class TriggerAuthoring : MonoBehaviour
{
    class Baker : Baker<TriggerAuthoring>
    {
        public override void Bake(TriggerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent<TriggerDetectionSystem.TriggerTag>(entity);
        }
    }
}
