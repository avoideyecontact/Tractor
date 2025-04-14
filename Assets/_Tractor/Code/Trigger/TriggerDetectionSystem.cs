using Unity.Burst;
using Unity.Entities;
using Unity.Physics;


[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
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
            GoodComponents = SystemAPI.GetComponentLookup<GoodTag>(),
        }.Schedule(simulation, state.Dependency);
    }

    public struct TriggerTag : IComponentData { }
    public struct OtherComponent : IComponentData { }

    [BurstCompile]
    struct TriggerJob : ITriggerEventsJob
    {
        public EntityCommandBuffer ecb;
        public ComponentLookup<TriggerTag> TriggerComponents;
        public ComponentLookup<GoodTag> GoodComponents;

        [BurstCompile]
        public void Execute(TriggerEvent triggerEvent)
        {
            bool isEntityATrigger = TriggerComponents.HasComponent(triggerEvent.EntityA);
            bool isEntityBTrigger = TriggerComponents.HasComponent(triggerEvent.EntityB);

            if (isEntityATrigger != isEntityBTrigger)
            {
                Entity triggerEntity = isEntityATrigger ? triggerEvent.EntityA : triggerEvent.EntityB;
                Entity otherEntity = isEntityATrigger ? triggerEvent.EntityB : triggerEvent.EntityA;

                if (GoodComponents.HasComponent(otherEntity))
                {
                    ecb.DestroyEntity(otherEntity);
                }
            }
        }
    }
}
