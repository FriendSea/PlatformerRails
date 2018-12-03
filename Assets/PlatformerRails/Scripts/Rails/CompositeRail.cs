using UnityEngine;

namespace PlatformerRails
{
    /// <summary>
    /// 連続してない複数のレールをまとめて１つにする．
    /// </summary>
    public class CompositeRail : IRail
    {
        /// <summary>
        /// レールの両端に余白を追加する．余白部分ではW2Lがnullになる．
        /// </summary>
        class MarginRail : IRail
        {
            const float margin = 1f;
            public IRail Rail { get; set; }

            public float Length { get { return Rail.Length + margin * 2; } }

            public Vector3 Local2World(Vector3 LocalPosition)
            {
                return Rail.Local2World(LocalPosition - Vector3.forward * margin);
            }

            public Quaternion Rotation(float RailZ)
            {
                return Rail.Rotation(RailZ - margin);
            }

            public Vector3? World2Local(Vector3 WorldPosition)
            {
                Vector3? pos = Rail.World2Local(WorldPosition);
                if (pos == null) return null;
                if (pos.Value.z < 0) return null;
                if (pos.Value.z > Rail.Length) return null;
                return pos + Vector3.forward * margin;
            }

            public float Height(float RailZ)
            {
                return Rail.Height(RailZ - margin);
            }

            public MarginRail(IRail original)
            {
                Rail = original;
            }
        }

        ChainRail chainRail;

        public float Length { get { return chainRail.Length; } }

        public Vector3 Local2World(Vector3 LocalPosition)
        {
            return chainRail.Local2World(LocalPosition);
        }

        public Quaternion Rotation(float RailZ)
        {
            return chainRail.Rotation(RailZ);
        }

        public Vector3? World2Local(Vector3 WorldPosition)
        {
            return chainRail.World2Local(WorldPosition);
        }

        public CompositeRail()
        {
            chainRail = new ChainRail();
        }

        public void AddRail(IRail rail)
        {
            foreach (var r in chainRail.Segments)
                if ((r as MarginRail).Rail == rail) return;
            chainRail.AddRail(new MarginRail(rail));
        }

        public void RemoveRail(IRail rail)
        {
            IRail target = null;
            foreach (var r in chainRail.Segments)
                if ((r as MarginRail).Rail == rail) target = r;
            if (target == null) return;
            RemoveRail(target);
        }

        public void Clear()
        {
            chainRail.Clear();
        }

        public float Height(float RailZ)
        {
            return chainRail.Height(RailZ);
        }
    }
}
