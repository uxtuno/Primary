using UnityEngine;
using System.Collections;

/// <summary>
/// 単純な回転
/// </summary>
public class RotateAxis : MonoBehaviour {
	public enum Axis
	{
		forward,
		up,
		right,
	}

	[SerializeField]
	private Axis axis = Axis.forward;
	[SerializeField]
	private float speed = 1.0f;
	Vector3 axisVec;

	void Start()
	{
		switch (axis)
		{
			case Axis.forward:
				axisVec = Vector3.forward;
				break;
			case Axis.up:
				axisVec = Vector3.up;
				break;
			case Axis.right:
				axisVec = Vector3.right;
				break;
		}
	}

	void Update()
	{
		transform.Rotate(axisVec, Time.deltaTime * speed);
	}
}
