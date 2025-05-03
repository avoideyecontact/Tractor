using Unity.Entities;
using UnityEngine;

public class ScoreAuthoring : MonoBehaviour
{
    class ScoreBaker : Baker<ScoreAuthoring>
    {
        public override void Bake(ScoreAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new ScoreComponent());
        }
    }
}
