using UnityEngine;

namespace PlatformerRails
{
    public abstract class RailBehaviour : MonoBehaviour, IRail
    {
        public abstract float Length { get; }
        public virtual float Height(float RailZ) { return 0; }
        public abstract Vector3 Local2World(Vector3 LocalPosition);
        public abstract Quaternion Rotation(float RailZ);
        public abstract Vector3? World2Local(Vector3 WorldPosition);
        public abstract Vector3? World2Local(Vector3 WorldPosition, out IRail usedSubrail);

        void OnEnable()
        {
            RailManager.AddRail(this);
        }

        void OnDisable()
        {
            RailManager.RemoveRail(this);
        }
    }
}
