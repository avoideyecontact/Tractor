using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Extensions;
using Unity.Physics;
using UnityEngine;
using Unity.Transforms;

[BurstCompile]
public partial struct TractorMovementSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityManager entityManager = state.EntityManager;
        NativeArray<Entity> entities = entityManager.GetAllEntities(Allocator.Temp);

        foreach (Entity entity in entities)
        {
            if (entityManager.HasComponent<TractorMovementComponent>(entity))
            {
                var tractor = entityManager.GetComponentData<TractorMovementComponent>(entity);

                var velocity = entityManager.GetComponentData<PhysicsVelocity>(entity);
                var mass = entityManager.GetComponentData<PhysicsMass>(entity);
                var transform = entityManager.GetComponentData<LocalTransform>(entity);


                // Angle correction
                Quaternion quaternion = transform.Rotation;
                float3 euler = quaternion.eulerAngles;
                transform.Rotation = Quaternion.Euler(0f, euler.y, 0f);

                // Pos correction
                transform.Position.y = tractor.basePosY;

                // Movement
                float3 direction = math.forward();
                direction = math.mul(transform.Rotation, direction);
                float3 linearImpulse = math.normalize(direction) * tractor.movementForce * Input.GetAxis("Vertical");
                velocity.ApplyLinearImpulse(mass, linearImpulse * SystemAPI.Time.DeltaTime);

                // Rotation
                float3 angularIpulse = new float3(0, Input.GetAxis("Horizontal"), 0) * tractor.movementForce;                
                velocity.ApplyAngularImpulse(mass, angularIpulse * SystemAPI.Time.DeltaTime);


                entityManager.SetComponentData<PhysicsVelocity>(entity, velocity);
                entityManager.SetComponentData<LocalTransform>(entity, transform);
            }
        }
    }
}
