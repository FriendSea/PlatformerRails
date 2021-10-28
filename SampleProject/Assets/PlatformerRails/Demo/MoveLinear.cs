using UnityEngine;
using PlatformerRails;

[RequireComponent(typeof(MoverOnRails))]
public class MoveLinear : MonoBehaviour {
    [SerializeField]
    Vector3 Velocity;

    MoverOnRails Controller;
    void Start()
    {
        Controller = GetComponent<MoverOnRails>();
        Controller.Velocity = this.Velocity;
    }

    void FixedUpdate()
    {
        Controller.Velocity.x = -Controller.Position.x * 5f;
    }
}
