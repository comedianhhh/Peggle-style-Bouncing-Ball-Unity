using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class LauncherAuthoring : MonoBehaviour
{
    public float launchForce = 100.0f;
    public Vector3 launchDirection = Vector3.forward;
    public GameObject ballPrefab;
    
    public class LauncherAuthoringBaker : Baker<LauncherAuthoring>
    {
        public override void Bake(LauncherAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new LauncherInputData
            {
                launchForce = authoring.launchForce,
                launchDirection = authoring.launchDirection
            });

            AddComponent(entity, new BallSpawner
            {
                BallPrefab = GetEntity(authoring.ballPrefab, TransformUsageFlags.Dynamic)
            });
        }
    }
}
