using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace VibeLabs.MovingBrick
{
    public struct MovingBrickPathData : IComponentData
    {
        public PathType PathType;
        public float speed;
        public float amplitudeX, amplitudeY;
        public float frequencyX, frequencyY;
        public float3 startPosition;
        public FixedList128Bytes<float3> waypoints;
    }
}
