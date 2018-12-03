using UnityEngine;

namespace PlatformerRails
{
    public class LineRail : RailBehaviour
    {
        [SerializeField]
        float length;

        public override float Length { get { return length; } }

        public override Vector3 Local2World(Vector3 LocalPosition)
        {
            return transform.position + transform.rotation * LocalPosition;
        }

        public override Vector3? World2Local(Vector3 WorldPosition)
        {
            return Quaternion.Inverse(transform.rotation) * (WorldPosition - transform.position);
        }

        public override Quaternion Rotation(float RailZ)
        {
            return transform.rotation;
        }

#if UNITY_EDITOR
        const float DrawWidth = 0.25f;
        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * Length);
            Gizmos.DrawLine(transform.position + transform.right * DrawWidth, transform.position + transform.forward * Length + transform.right * DrawWidth);
            Gizmos.DrawLine(transform.position - transform.right * DrawWidth, transform.position + transform.forward * Length - transform.right * DrawWidth);
        }
#endif
    }
}
