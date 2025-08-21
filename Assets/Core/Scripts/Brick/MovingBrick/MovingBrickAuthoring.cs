using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace VibeLabs.MovingBrick
{
    public class MovingBrickAuthoring : MonoBehaviour
    {
        public PathType pathType;
        public float speed = 1f;
        public float amplitudeX = 2f;
        public float amplitudeY = 1f;
        public float frequencyX = 2f;
        public float frequencyY = 1f;


        
        public class MovingBrickBaker : Baker<MovingBrickAuthoring>
        {
            public override void Bake(MovingBrickAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);

                var pathData = new MovingBrickPathData
                {
                    PathType = authoring.pathType,
                    speed = authoring.speed,
                    amplitudeX = authoring.amplitudeX,
                    amplitudeY = authoring.amplitudeY,
                    frequencyX = authoring.frequencyX,
                    frequencyY = authoring.frequencyY,
                    startPosition = (float3)authoring.transform.position
                };
                
                AddComponent(entity, pathData);
                AddComponent(entity, new PathProgress { value = 0f, movingForward = true });
            }
        }
        
        
    }
}
