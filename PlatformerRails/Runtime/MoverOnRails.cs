using System.Collections;
using UnityEngine;

namespace PlatformerRails
{
    [RequireComponent(typeof(Rigidbody))]
    public class MoverOnRails : MonoBehaviour, IMover
    {
        [SerializeField]
        RailBehaviour railBehaviour;
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }

        IRail rail;
        Rigidbody rigidbody;
        IRail currentSingleRail;

        public event System.Action<IRail> RailChangeEvent;

        public IRail Rail => rail != null ? rail : railBehaviour;

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
            Vector3 worldPosition = rail.Local2World(Position);
            rigidbody.MovePosition(worldPosition);
        }

        void LateFixedUpdate()
        {
            UpdateLocalPosition();
        }

        void UpdateLocalPosition()
        {
            IRail usedRail;
            var w2l = rail.World2Local(transform.position, out usedRail);
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
            CheckUsedRail(usedRail);
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

        private void CheckUsedRail(IRail usedRail)
        {
            if (usedRail != currentSingleRail)
            {
                RailChangeEvent?.Invoke(usedRail);
                currentSingleRail = usedRail;
            }
        }
    }
}
