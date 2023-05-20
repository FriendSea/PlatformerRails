using UnityEngine;

namespace PlatformerRails
{
    public class RailManager : IRail
    {
        static RailManager _instance;
        public static RailManager instance
        {
            get
            {
                if (_instance == null) _instance = new RailManager();
                return _instance;
            }
        }

        CompositeRail compositeRail;

        public RailManager()
        {
            compositeRail = new CompositeRail();
        }

        public void Clear()
        {
            compositeRail.Clear();
        }

        public float Length { get { return compositeRail.Length; } }

        public Vector3 Local2World(Vector3 LocalPosition)
        {
            return compositeRail.Local2World(LocalPosition);
        }

        public Quaternion Rotation(float RailZ)
        {
            return compositeRail.Rotation(RailZ);
        }

        public Vector3? World2Local(Vector3 WorldPosition)
        {
            return compositeRail.World2Local(WorldPosition);
        }

        public Vector3? World2Local(Vector3 WorldPosition, out IRail usedSubrail)
        {
            return compositeRail.World2Local(WorldPosition, out usedSubrail);
        }

        public float Height(float RailZ)
        {
            return compositeRail.Height(RailZ);
        }

        public static void AddRail(IRail newrail)
        {
            instance.compositeRail.AddRail(newrail);
        }

        public static void RemoveRail(IRail rail)
        {
            instance.compositeRail.RemoveRail(rail);
        }
    }
}
