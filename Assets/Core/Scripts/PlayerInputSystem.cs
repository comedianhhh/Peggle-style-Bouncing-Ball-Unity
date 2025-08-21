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
        Debug.unityLogger.Log(mousePosition);
        var mouseClicked = Input.GetMouseButtonDown(0);

        foreach (var (launcher, localToWorld) in SystemAPI.Query<RefRW<LauncherInputData>, RefRO<LocalToWorld>>())
        {
            launcher.ValueRW.launch = mouseClicked;
            
            var ray = mainCamera.ScreenPointToRay(mousePosition);
            var plane = new Plane(Vector3.forward, 0); 

            if (plane.Raycast(ray, out var distance))
            {
                float3 worldMousePosition = ray.GetPoint(distance);
                var launcherPosition = localToWorld.ValueRO.Position;
                
                var direction = math.normalize(worldMousePosition - launcherPosition);
                launcher.ValueRW.launchDirection = direction;
            }
        }
    }
}
