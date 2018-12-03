using UnityEngine;

public class MoveLift : MonoBehaviour {
	[SerializeField]
	Vector3 Axis;
	[SerializeField]
	float Speed;

	void FixedUpdate()
	{
		transform.localPosition = Axis * Mathf.Sin(Time.fixedTime * Speed);
	}
}
