using UnityEngine;
using System.Collections;

/// <summary>
/// アイテムの回転
/// </summary>
public class ItemRolling : MyMonoBehaviour
{
	// 地球の自転を意識した23.43度
	private static readonly float axisAngle = 23.43f + 270.0f;
	private Vector3 RotationAxis = new Vector3(Mathf.Cos(axisAngle * Mathf.Deg2Rad), Mathf.Sin(axisAngle * Mathf.Deg2Rad));
	private float speed = 100.0f;

	protected override void Update()
	{
		transform.RotateAround(transform.position, RotationAxis, Time.deltaTime * speed);
	}
}
