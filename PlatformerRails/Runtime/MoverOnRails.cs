using System.Collections;
using UnityEngine;
using UnityEngine.LowLevel;
using System.Linq;

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

        public bool IsOnRail { get; private set; }
        public event System.Action OnLocalPositionUpdated;

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
            LateFixedUpdate.OnLateFixedUpdate += UpdateLocalPosition;
            UpdateLocalPosition();
        }

        void OnDisable()
        {
            LateFixedUpdate.OnLateFixedUpdate -= UpdateLocalPosition;
            Velocity = Vector3.zero;
        }

        void FixedUpdate()
        {
            Position += Velocity * Time.fixedDeltaTime;
            rigidbody.MovePosition(rail.Local2World(Position));
        }

        void UpdateLocalPosition()
        {
            var w2l = rail.World2Local(transform.position);
            IsOnRail = w2l != null;
            if (!IsOnRail) return;
            Position = w2l.Value;
            rigidbody.velocity = Vector3.zero;

            var newrot = rail.Rotation(Position.z);
            /*
            if (Quaternion.Angle(transform.rotation, newrot) > 30f)
                Velocity = Quaternion.Inverse(newrot) * transform.rotation * Velocity;
            */
            transform.rotation = newrot;

            OnLocalPositionUpdated?.Invoke();
        }
    }

    static class LateFixedUpdate
	{
        static bool initialized;
        static event System.Action _onLateFixedUpdate;
        public static event System.Action OnLateFixedUpdate
		{
			add
			{
				if (!initialized)
				{
				    var loops = PlayerLoop.GetCurrentPlayerLoop();
                    for(int i=0;i<loops.subSystemList.Length;i++)
					{
                        var isFixedUpdate = false;
                        foreach(var item in loops.subSystemList[i].subSystemList)
                            isFixedUpdate |= item.type == typeof(UnityEngine.PlayerLoop.FixedUpdate.PhysicsFixedUpdate);
						if (isFixedUpdate)
						{
                            loops.subSystemList[i].subSystemList =
                                loops.subSystemList[i].subSystemList.Append(new PlayerLoopSystem()
                                {
                                    type = typeof(LateFixedUpdate),
                                    updateDelegate = () => _onLateFixedUpdate?.Invoke()
                                }).ToArray();
                            PlayerLoop.SetPlayerLoop(loops);
                            break;
						}
					}
				}
                _onLateFixedUpdate += value;
                initialized = true;
			}
			remove
			{
                _onLateFixedUpdate -= value;
			}
		}
	}
}
