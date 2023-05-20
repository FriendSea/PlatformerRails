using UnityEngine;

namespace PlatformerRails
{
    public class ConveyorRail : RailBehaviour
    {
        [SerializeField]
        float length;
        [SerializeField]
        float Range = 0.55f;
        [SerializeField]
        float Speed;

        public override float Length { get { return length; } }

        public override Vector3 Local2World(Vector3 LocalPosition)
        {
            return transform.position + transform.rotation * LocalPosition + transform.forward * Speed * Time.fixedDeltaTime;
        }

        public override Vector3? World2Local(Vector3 WorldPosition)
        {
            var result = Quaternion.Inverse(transform.rotation) * (WorldPosition - transform.position);
            if (result.y > Range) return null;
            return result;
        }

        public override Vector3? World2Local(Vector3 WorldPosition, out IRail usedSubrail)
        {
            var result = Quaternion.Inverse(transform.rotation) * (WorldPosition - transform.position);
            usedSubrail = this;
            if (result.y > Range) return null;
            return result;
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
            Gizmos.DrawLine(transform.position + transform.up * Range, transform.position + transform.forward * Length + transform.up * Range);
            Gizmos.color = Color.red;
            for (float pos = 0; pos < length; pos += 1)
            {
                Gizmos.DrawLine(transform.position + transform.forward * pos + transform.right * DrawWidth, transform.position + transform.forward * (pos + Speed * DrawWidth));
                Gizmos.DrawLine(transform.position + transform.forward * pos - transform.right * DrawWidth, transform.position + transform.forward * (pos + Speed * DrawWidth));
            }
        }
#endif
    }
}
