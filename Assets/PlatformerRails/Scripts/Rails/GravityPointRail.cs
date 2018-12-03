using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PlatformerRails
{
    public class GravityPointRail : RailBehaviour
    {
        [SerializeField]
        float Radius;
        [SerializeField]
        float Range;

        public override float Length { get { return (Radius + Range / 2) * 2 * Mathf.PI; } }

        public override Vector3 Local2World(Vector3 LocalPosition)
        {
            float angle = LocalPosition.z / Length * 360f;
            Quaternion rot = transform.rotation * Quaternion.AngleAxis(angle, Vector3.right);
            LocalPosition.z = 0;
            return transform.position + rot * (LocalPosition + Vector3.up * Radius);
        }

        public override Quaternion Rotation(float RailZ)
        {
            float angle = RailZ / Length * 360f;
            return transform.rotation * Quaternion.AngleAxis(angle, Vector3.right);
        }

        public override Vector3? World2Local(Vector3 WorldPosition)
        {
            Vector3 ret = Vector3.zero;
            Vector3 localPos = Quaternion.Inverse(transform.rotation) * (WorldPosition - transform.position);
            ret.x = localPos.x;
            localPos.x = 0;
            ret.y = localPos.magnitude - Radius;
            float angle = Vector3.Angle(Vector3.up, localPos);
            if (localPos.z < 0) angle = 360 - angle;
            ret.z = angle / 360f * Length;

            if (ret.y > Range) return null;
            return ret;
        }

#if UNITY_EDITOR
        const float DrawWidth = 0.25f;
        void OnDrawGizmos()
        {
            Handles.color = Color.green;
            Handles.DrawWireDisc(transform.position, transform.right, Radius);
            Handles.DrawWireDisc(transform.position + transform.right * DrawWidth, transform.right, Radius);
            Handles.DrawWireDisc(transform.position - transform.right * DrawWidth, transform.right, Radius);
            Handles.DrawWireDisc(transform.position, transform.right, Radius + Range);
        }
#endif
    }
}

