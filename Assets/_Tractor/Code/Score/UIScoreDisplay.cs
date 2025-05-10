using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class UIScoreDisplay : MonoBehaviour
{
    private Text _scoreText;
    private EntityManager _entityManager;
    private EntityQuery _goodsQuery;

    void Start()
    {
        _scoreText = GetComponent<Text>();
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _goodsQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<GoodTag>()
            .Build(_entityManager);
    }

    void FixedUpdate()
    {
        _scoreText.text = $"{_goodsQuery.CalculateEntityCount()}";
    }
}
