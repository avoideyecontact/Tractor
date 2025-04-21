using Unity.Entities;
using UnityEngine;

public partial class TractorInputSystem : SystemBase
{
    protected override void OnCreate()
    {
        RequireForUpdate<TractorInputComponent>();
    }

    protected override void OnUpdate()
    {
        var input = new TractorInputComponent
        {
            Horizontal = Input.GetAxis("Horizontal"),
            Vertical = Input.GetAxis("Vertical")
        };

        SystemAPI.SetSingleton(input);
    }
}
