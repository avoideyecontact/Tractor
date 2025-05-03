using Unity.Entities;
using UnityEngine;

public class ScoreAuthoring : MonoBehaviour
{
    [SerializeField] private int score;

    class ScoreBaker : Baker<ScoreAuthoring>
    {
        public override void Bake(ScoreAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);

            AddComponent(entity, new ScoreComponent
            {
                Score = authoring.score,
            });
        }
    }
}
