using UnityEngine;

namespace PlatformerRails
{
    public interface IRail
    {
        float Length { get; }
        Vector3 Local2World(Vector3 LocalPosition);
        Vector3? World2Local(Vector3 WorldPosition);
        Quaternion Rotation(float RailZ);
        float Height(float RailZ);
    }
}
