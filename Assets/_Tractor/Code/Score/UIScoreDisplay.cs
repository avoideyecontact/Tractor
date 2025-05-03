using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreDisplay : MonoBehaviour
{
    private Text _scoreText;
    private EntityManager _entityManager;
    private EntityQuery _scoreQuery;

    void Start()
    {
        _scoreText = GetComponent<Text>();
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _scoreQuery = _entityManager.CreateEntityQuery(ComponentType.ReadOnly<ScoreComponent>());
    }

    void FixedUpdate()
    {
        if (_scoreQuery.TryGetSingleton<ScoreComponent>(out var score))
        {
            _scoreText.text = $"Score: {score.Score}";
        }
    }
}
