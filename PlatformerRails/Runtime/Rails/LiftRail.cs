using UnityEngine;

namespace PlatformerRails
{
    public class LiftRail : RailBehaviour
    {
        [SerializeField]
        float length;
        [SerializeField]
        float Range = 0.55f;

        Vector3 position;
        Quaternion rotation;

        public override float Length { get { return length; } }

        public override Vector3 Local2World(Vector3 LocalPosition)
        {
            return position + rotation * LocalPosition;
        }

        public override Vector3? World2Local(Vector3 WorldPosition)
        {
            var result = Quaternion.Inverse(rotation) * (WorldPosition - position);
            if (result.y > Range) return null;
            return result;
        }

        public override Quaternion Rotation(float RailZ)
        {
            return rotation;
        }

        void Awake()
        {
            position = transform.position;
            rotation = transform.rotation;
        }

        void FixedUpdate()
        {
            position = transform.position;
            rotation = transform.rotation;
        }

#if UNITY_EDITOR
        const float DrawWidth = 0.25f;
        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * Length);
            Gizmos.DrawLine(transform.position + transform.right * DrawWidth, transform.position + transform.forward * Length + transform.right * DrawWidth);
            Gizmos.DrawLine(transform.position - transform.right * DrawWidth, transform.position + transform.forward * Length - transform.right * DrawWidth);
            Gizmos.DrawLine(transform.position + transform.up * Range, transform.position + transform.forward * Length + transform.up * Range);
        }
#endif
    }
}
