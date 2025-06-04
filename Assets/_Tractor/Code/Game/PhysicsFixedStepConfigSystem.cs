using Unity.Entities;

public partial class PhysicsFixedStepConfigSystem : SystemBase
{
    protected override void OnCreate()
    {
        var fixedSimGroup = World.GetExistingSystemManaged<FixedStepSimulationSystemGroup>();
        fixedSimGroup.Timestep = 1f / 30f;
    }

    protected override void OnUpdate() { }
}
