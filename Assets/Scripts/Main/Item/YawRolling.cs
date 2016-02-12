using UnityEngine;
using System.Collections;

/// <summary>
/// 横方向の回転
/// </summary>
public class YawRolling : MyMonoBehaviour
{
	private static readonly float axisAngle = 23.43f + 270.0f;
	private Vector3 RotationAxis = new Vector3(Mathf.Cos(axisAngle * Mathf.Deg2Rad), Mathf.Sin(axisAngle * Mathf.Deg2Rad));
	private float speed = 50.0f;

	void Start()
	{
		RotationAxis = transform.forward;
	}

	protected override void Update()
	{
		transform.RotateAround(transform.position, RotationAxis, Time.deltaTime * speed);
	}
}
