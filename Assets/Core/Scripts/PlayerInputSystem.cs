using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial class PlayerInputSystem : SystemBase
{
    private Camera mainCamera;

    protected override void OnCreate()
    {
        mainCamera = Camera.main;
    }

    protected override void OnUpdate()
    {
        if (mainCamera == null) return;

        var mousePosition = Input.mousePosition;
        var mouseClicked = Input.GetMouseButtonDown(0);

        foreach (var (launcher, localTransform) 
                 in SystemAPI.Query<RefRW<LauncherInputData>, RefRW<LocalTransform>>())
        {
            launcher.ValueRW.launch = mouseClicked;

            var ray = mainCamera.ScreenPointToRay(mousePosition);
            var plane = new Plane(Vector3.forward, 0); // XY plane (Z=0)

            if (plane.Raycast(ray, out var distance))
            {
                float3 worldMousePosition = ray.GetPoint(distance);
                worldMousePosition.z = 0; // stay in XY plane

                float3 launcherPos = localTransform.ValueRO.Position;
                launcherPos.z = 0;

                float2 dir = math.normalize(worldMousePosition.xy - launcherPos.xy);

                // Save direction for BallLaunchSystem
                launcher.ValueRW.launchDirection = new float3(dir.x, dir.y, 0);

                // Rotate launcher toward mouse
                float angle = math.atan2(dir.y, dir.x); // radians
                localTransform.ValueRW.Rotation = quaternion.RotateZ(angle);
            }
        }
    }
}
