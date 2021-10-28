using UnityEngine;

public class ModelRotator : MonoBehaviour {
	[SerializeField]
	float EaseSpeed = 5f;

	void Awake()
	{
		beforeRotation = transform.rotation;
	}

	Quaternion beforeRotation;
	void Update () {
		transform.rotation = beforeRotation = Quaternion.Lerp(beforeRotation, transform.parent.rotation, EaseSpeed * Time.deltaTime);
	}
}
