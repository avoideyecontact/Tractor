using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

public struct DestroyableTag : IComponentData { }

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
//[UpdateBefore(typeof(PhysicsSimulationGroup))]
[BurstCompile]
public partial struct TriggerDetectionSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
        var ecb = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged);
        var simulation = SystemAPI.GetSingleton<SimulationSingleton>();

        state.Dependency = new TriggerJob
        {
            ecb = ecb,
            TriggerComponents = SystemAPI.GetComponentLookup<TriggerTag>(),
            OtherComponents = SystemAPI.GetComponentLookup<OtherComponent>(),
            //DestroyableComponents = SystemAPI.GetComponentLookup<DestroyableTag>(),
            //GoodComponents = SystemAPI.GetComponentLookup<GoodComponent>()
        }.Schedule(simulation, state.Dependency);
    }

    public struct TriggerTag : IComponentData { }
    public struct OtherComponent : IComponentData { }

    [BurstCompile]
    struct TriggerJob : ITriggerEventsJob
    {
        public EntityCommandBuffer ecb;
        public ComponentLookup<TriggerTag> TriggerComponents;
        public ComponentLookup<OtherComponent> OtherComponents;
        //public ComponentLookup<DestroyableTag> DestroyableComponents;
        //public ComponentLookup<GoodComponent> GoodComponents;

        [BurstCompile]
        public void Execute(TriggerEvent triggerEvent)
        {
            bool isEntityATrigger = TriggerComponents.HasComponent(triggerEvent.EntityA);
            bool isEntityBTrigger = TriggerComponents.HasComponent(triggerEvent.EntityB);

            if (isEntityATrigger != isEntityBTrigger)
            {
                Entity triggerEntity = isEntityATrigger ? triggerEvent.EntityA : triggerEvent.EntityB;
                Entity otherEntity = isEntityATrigger ? triggerEvent.EntityB : triggerEvent.EntityA;

                if (OtherComponents.HasComponent(otherEntity))
                {
                    ecb.DestroyEntity(otherEntity);
                    //if (DestroyableComponents.HasComponent(otherEntity))
                    //{
                    //    if (GoodComponents.HasComponent(otherEntity))
                    //    {
                    //        UnityEngine.Debug.Log("+1");
                    //    }
                    //}
                }
            }
        }
    }
}
