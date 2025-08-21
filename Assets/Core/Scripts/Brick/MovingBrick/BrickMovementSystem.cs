using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace VibeLabs.MovingBrick
{
    [BurstCompile]
    public partial struct BrickMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            foreach (var (transform, progress, pathData) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PathProgress>, RefRO<MovingBrickPathData>>())
            {
                var currentProgress = progress.ValueRO.value;
                var newProgress = currentProgress;

                switch (pathData.ValueRO.PathType)
                {
                    case PathType.verticalLine:
                        
                        if (progress.ValueRO.movingForward)
                        {
                            newProgress += pathData.ValueRO.speed * deltaTime;
                            if (newProgress >= 1f)
                            {
                                newProgress = 1f;
                                progress.ValueRW.movingForward = false;
                            }
                        }
                        else
                        {
                            newProgress -= pathData.ValueRO.speed * deltaTime;
                            if (newProgress <= 0f)
                            {
                                newProgress = 0f;
                                progress.ValueRW.movingForward = true;
                            }
                        }
                        
                        var v_y = math.lerp(pathData.ValueRO.startPosition.y, 
                            pathData.ValueRO.startPosition.y + pathData.ValueRO.amplitudeY, 
                            newProgress);
                        transform.ValueRW.Position = new float3(pathData.ValueRO.startPosition.x, v_y, pathData.ValueRO.startPosition.z);

                        
                        
                        break;
                    
                    
                    
                    case PathType.horizontalLine:
                        if (progress.ValueRO.movingForward)
                        {
                            newProgress += pathData.ValueRO.speed * deltaTime;
                            if (newProgress >= 1f)
                            {
                                newProgress = 1f;
                                progress.ValueRW.movingForward = false;
                            }
                        }
                        else
                        {
                            newProgress -= pathData.ValueRO.speed * deltaTime;
                            if (newProgress <= 0f)
                            {
                                newProgress = 0f;
                                progress.ValueRW.movingForward = true;
                            }
                        }

                        var h_x = math.lerp(pathData.ValueRO.startPosition.x, 
                            pathData.ValueRO.startPosition.x + pathData.ValueRO.amplitudeX, 
                            newProgress);
                        transform.ValueRW.Position = new float3(h_x, pathData.ValueRO.startPosition.y, pathData.ValueRO.startPosition.z);
                        
                        break;
                    case PathType.Infinity:
                        newProgress = (currentProgress + pathData.ValueRO.speed * deltaTime) % 1f;
                        var x = pathData.ValueRO.startPosition.x + pathData.ValueRO.amplitudeX * math.sin(newProgress * pathData.ValueRO.frequencyX * math.PI * 2);
                        var y = pathData.ValueRO.startPosition.y + pathData.ValueRO.amplitudeY * math.sin(newProgress * pathData.ValueRO.frequencyY * math.PI * 2);
                        transform.ValueRW.Position = new float3(x, y, pathData.ValueRO.startPosition.z);
                        break;
                    
                    case PathType.M:
                        if (progress.ValueRO.movingForward)
                        {
                            newProgress += pathData.ValueRO.speed * deltaTime;
                            if (newProgress >= 1f)
                            {
                                newProgress = 1f;
                                progress.ValueRW.movingForward = false;
                            }
                        }
                        else
                        {
                            newProgress -= pathData.ValueRO.speed * deltaTime;
                            if (newProgress <= 0f)
                            {
                                newProgress = 0f;
                                progress.ValueRW.movingForward = true;
                            }
                        }

                        var m_x = pathData.ValueRO.startPosition.x + newProgress * pathData.ValueRO.amplitudeX;
                        var m_y = pathData.ValueRO.startPosition.y + pathData.ValueRO.amplitudeY * 0.5f * (1 - math.cos(newProgress * pathData.ValueRO.frequencyY * 2 * math.PI));
                        transform.ValueRW.Position = new float3(m_x, m_y, pathData.ValueRO.startPosition.z);
                        break;

                    case PathType.Z:
                        if (progress.ValueRO.movingForward)
                        {
                            newProgress += pathData.ValueRO.speed * deltaTime;
                            if (newProgress >= 1f)
                            {
                                newProgress = 1f;
                                progress.ValueRW.movingForward = false;
                            }
                        }
                        else
                        {
                            newProgress -= pathData.ValueRO.speed * deltaTime;
                            if (newProgress <= 0f)
                            {
                                newProgress = 0f;
                                progress.ValueRW.movingForward = true;
                            }
                        }

                        float3 z_pos;
                        if (newProgress < 1f / 3f)
                        {
                            // Top bar
                            var t = newProgress * 3f;
                            var p0 = pathData.ValueRO.startPosition + new float3(0, pathData.ValueRO.amplitudeY, 0);
                            var p1 = pathData.ValueRO.startPosition + new float3(pathData.ValueRO.amplitudeX, pathData.ValueRO.amplitudeY, 0);
                            z_pos = math.lerp(p0, p1, t);
                        }
                        else if (newProgress < 2f / 3f)
                        {
                            // Diagonal bar
                            var t = (newProgress - 1f / 3f) * 3f;
                            var p1 = pathData.ValueRO.startPosition + new float3(pathData.ValueRO.amplitudeX, pathData.ValueRO.amplitudeY, 0);
                            var p2 = pathData.ValueRO.startPosition;
                            z_pos = math.lerp(p1, p2, t);
                        }
                        else
                        {
                            // Bottom bar
                            var t = (newProgress - 2f / 3f) * 3f;
                            var p2 = pathData.ValueRO.startPosition;
                            var p3 = pathData.ValueRO.startPosition + new float3(pathData.ValueRO.amplitudeX, 0, 0);
                            z_pos = math.lerp(p2, p3, t);
                        }
                        transform.ValueRW.Position = z_pos;
                        break;
                }

                progress.ValueRW.value = newProgress;
            }
        }

        private static float3 GetCatmullRomPoint(float3 p0, float3 p1, float3 p2, float3 p3, float t)
        {
            return 0.5f * (
                (2 * p1) +
                (-p0 + p2) * t +
                (2 * p0 - 5 * p1 + 4 * p2 - p3) * t * t +
                (-p0 + 3 * p1 - 3 * p2 + p3) * t * t * t
            );
        }
    }
}
