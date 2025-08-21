using Unity.Entities;

namespace VibeLabs.MovingBrick
{
    public struct PathProgress : IComponentData
    {
        public float value;
        public bool movingForward;
    }
}
