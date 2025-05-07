using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class MagnetToggle : MonoBehaviour
{
    [SerializeField] private bool isEnabled;
    private EntityManager _entityManager;
    private EntityQuery _magnetQuery;

    private void Start()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _magnetQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<MagnetComponent>()
            .Build(_entityManager);
    }

    private void FixedUpdate()
    {
        _magnetQuery.TryGetSingletonEntity<MagnetComponent>(out var magnetEntity);
        _magnetQuery.TryGetSingleton<MagnetComponent>(out var magnet);

        if (magnet.isEnabled != isEnabled)
        {
            magnet.isEnabled = isEnabled;
            _entityManager.SetComponentData(magnetEntity, magnet);
        }
    }

    public void MagnetStateToggle()
    {
        isEnabled = !isEnabled;
    }
}
