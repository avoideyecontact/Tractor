using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class Endgame : MonoBehaviour
{
    [SerializeField] private GameObject endgameObject;
    private EntityManager _entityManager;
    private EntityQuery _goodsQuery;

    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _goodsQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<GoodTag>()
            .Build(_entityManager);
    }

    private void FixedUpdate()
    {
        if (_goodsQuery.CalculateEntityCount() == 0)
        {
            endgameObject.SetActive(true);
        }
        else
        {
            endgameObject.SetActive(false);
        }
    }
}
