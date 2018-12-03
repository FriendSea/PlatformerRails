using System.Collections;
using UnityEngine;

namespace PlatformerRails
{
    [RequireComponent(typeof(Rigidbody))]
    public class MoverOnRails : MonoBehaviour
    {
        [SerializeField]
        RailBehaviour railBehaviour;
        public Vector3 Position { get; set; }
        public Vector3 Velocity;

        IRail rail;
        Rigidbody rigidbody;

        void Reset()
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.freezeRotation = true;
        }

        void Awake()
        {
            if (railBehaviour == null)
                rail = RailManager.instance;
            else
                rail = railBehaviour;
            rigidbody = GetComponent<Rigidbody>();
        }

        void OnEnable()
        {
            StartCoroutine(RunLateFixedUpdate());
            UpdateLocalPosition();
        }

        void OnDisable()
        {
            StopCoroutine(RunLateFixedUpdate());
            Velocity = Vector3.zero;
        }

        void FixedUpdate()
        {
            Position += Velocity * Time.fixedDeltaTime;
            rigidbody.MovePosition(rail.Local2World(Position));
        }

        void LateFixedUpdate()
        {
            UpdateLocalPosition();
        }

        void UpdateLocalPosition()
        {
            var w2l = rail.World2Local(transform.position);
            if (w2l == null)
            {
                Destroy(gameObject);
                return;
            }
            Position = w2l.Value;
            rigidbody.velocity = Vector3.zero;

            var newrot = rail.Rotation(Position.z);
            if (Quaternion.Angle(transform.rotation, newrot) > 30f)
                Velocity = Quaternion.Inverse(newrot) * transform.rotation * Velocity;
            transform.rotation = newrot;
        }

        IEnumerator RunLateFixedUpdate()
        {
            var wait = new WaitForFixedUpdate();
            while (true)
            {
                yield return wait;
                LateFixedUpdate();
            }
        }
    }
}
