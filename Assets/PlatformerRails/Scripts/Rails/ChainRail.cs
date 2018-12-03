using System.Collections.Generic;
using UnityEngine;

namespace PlatformerRails
{
    /// <summary>
    /// 連続してる複数のレールをまとめて１つにする．
    /// </summary>
    public class ChainRail : IRail
    {
        List<IRail> Childs = new List<IRail>();
        public IEnumerable<IRail> Segments { get { return Childs; } }

        public bool isValid { get { return Childs.Count > 0; } }

        public float Length { get; private set; }

        public void AddRail(IRail newrail)
        {
            Childs.Add(newrail);
            Length += newrail.Length;
        }

        public void RemoveRail(IRail targetrail)
        {
            if (!Childs.Contains(targetrail)) return;
            Childs.Remove(targetrail);
            Length -= targetrail.Length;
        }

        public void Clear()
        {
            Childs.Clear();
            Length = 0;
        }

        IRail Way2Segment(float way, out float localWay)
        {
            foreach (var r in Childs)
            {
                if (r.Length < way)
                {
                    way -= r.Length;
                    continue;
                }
                localWay = way;
                return r;
            }

            localWay = way + Childs[Childs.Count - 1].Length;
            return Childs[Childs.Count - 1];
        }

        public Vector3 Local2World(Vector3 LocalPosition)
        {
            float localway;
            IRail segment = Way2Segment(LocalPosition.z, out localway);
            LocalPosition.z = localway;
            return segment.Local2World(LocalPosition);
        }

        struct W2Lresult
        {
            public IRail rail;
            public Vector3 pos;
            public float height;
            public float dist;
            public float RailDist { get { return Mathf.Abs(pos.x) + Mathf.Abs(pos.y - height); } }
            public bool IsHighThanRail { get { return pos.y >= height; } }
        }

        List<W2Lresult> results = new List<W2Lresult>();
        public Vector3? World2Local(Vector3 WorldPosition)
        {
            float distance = 0;
            results.Clear();
            foreach (var rail in Childs)
            {
                distance += rail.Length;
                var pos = rail.World2Local(WorldPosition);

                if (pos == null) continue;
                if (pos.Value.z < 0 || pos.Value.z > rail.Length) continue;

                W2Lresult res = new W2Lresult
                {
                    rail = rail,
                    pos = pos.Value,
                    height = rail.Height(pos.Value.z),
                    dist = distance - rail.Length
                };
                results.Add(res);
            }
            if (results.Count <= 0) return null;
            results.Sort((a, b) =>
            {
                if (a.IsHighThanRail && !b.IsHighThanRail) return -1;
                if (!a.IsHighThanRail && b.IsHighThanRail) return 1;
                return (int)Mathf.Sign(a.RailDist - b.RailDist);
            });
            return results[0].pos + Vector3.forward * results[0].dist;
        }

        public Quaternion Rotation(float RailZ)
        {
            float localway;
            IRail segment = Way2Segment(RailZ, out localway);
            return segment.Rotation(localway);
        }

        public float Height(float RailZ)
        {
            float localway;
            IRail segment = Way2Segment(RailZ, out localway);
            return segment.Height(localway);
        }
    }
}
