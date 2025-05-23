using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.InputSystem;

public class TractorControls : MonoBehaviour
{
    PlayerInput playerInput;
    private InputAction upAction;
    private InputAction downAction;
    private InputAction leftAction;
    private InputAction rightAction;

    private Vector2 tractorInput;

    private EntityManager _entityManager;
    private EntityQuery _inputQuery;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        upAction = playerInput.actions.FindAction("UP");
        downAction = playerInput.actions.FindAction("DOWN");
        leftAction = playerInput.actions.FindAction("LEFT");
        rightAction = playerInput.actions.FindAction("RIGHT");

        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        _inputQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<TractorInputComponent>()
            .Build(_entityManager);
    }

    private void OnEnable()
    {
        upAction.performed += UpPressed;
        downAction.performed += DownPressed;
        leftAction.performed += LeftPressed;
        rightAction.performed += RightPressed;
    }
    private void OnDisable()
    {
        upAction.performed -= UpPressed;
        downAction.performed -= DownPressed;
        leftAction.performed -= LeftPressed;
        rightAction.performed -= RightPressed;
    }

    private void FixedUpdate()
    {
        _inputQuery.TryGetSingletonEntity<TractorInputComponent>(out var inputEntity);
        _inputQuery.TryGetSingleton<TractorInputComponent>(out var input);

        input.Horizontal = tractorInput.x;
        input.Vertical = tractorInput.y;

        _entityManager.SetComponentData(inputEntity, input);
    }

    private void UpPressed(InputAction.CallbackContext context)
    {
        tractorInput.y = context.ReadValue<float>();
    }
    private void DownPressed(InputAction.CallbackContext context)
    {
        tractorInput.y = -context.ReadValue<float>();
    }
    private void LeftPressed(InputAction.CallbackContext context)
    {
        tractorInput.x = -context.ReadValue<float>();
    }
    private void RightPressed(InputAction.CallbackContext context)
    {
        tractorInput.x = context.ReadValue<float>();
    }
}
