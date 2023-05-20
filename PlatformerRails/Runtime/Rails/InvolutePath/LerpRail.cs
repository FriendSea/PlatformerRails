using UnityEngine;

namespace PlatformerRails
{
    public class LerpRail : IRail
    {
        Vector3 StartPoint;
        Vector3 EndPoint;
        Quaternion rotation;

        public System.Func<float, float> HeightFunc { private get; set; }
        public float Height(float RailZ)
        {
            return HeightFunc(RailZ);
        }

        public float Length { get { return (EndPoint - StartPoint).magnitude; } }

        public LerpRail(Vector3 startPoint, Vector3 endPoint, Vector3 UpVector)
        {
            StartPoint = startPoint;
            EndPoint = endPoint;
            rotation = Quaternion.LookRotation(EndPoint - StartPoint, UpVector);
        }

        public Vector3 Local2World(Vector3 LocalPosition)
        {
            Vector3 pos = StartPoint + (EndPoint - StartPoint) * LocalPosition.z / Length;
            LocalPosition.z = 0;
            return pos + rotation * LocalPosition;
        }

        public Quaternion Rotation(float RailZ)
        {
            return rotation;
        }

        public Vector3? World2Local(Vector3 WorldPosition)
        {
            return Quaternion.Inverse(rotation) * (WorldPosition - StartPoint);
        }

        public Vector3? World2Local(Vector3 WorldPosition, out IRail usedSubrail)
        {
            usedSubrail = this;
            return Quaternion.Inverse(rotation) * (WorldPosition - StartPoint);
        }
    }
}
