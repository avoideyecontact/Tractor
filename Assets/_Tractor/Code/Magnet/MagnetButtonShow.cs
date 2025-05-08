using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class MagnetButtonShow : MonoBehaviour
{
    [SerializeField] private GameObject magnetButton;
    [SerializeField] private int goodsLimitToEnableMagnet;
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
        if (_goodsQuery.CalculateEntityCount() < goodsLimitToEnableMagnet)
        {
            magnetButton.SetActive(true);
        }
        else
        {
            magnetButton.SetActive(false);
        }
    }
}
