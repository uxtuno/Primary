using UnityEngine;
using System.Collections;

public class CubeCursor : MyMonoBehaviour
{
	[SerializeField]
	private Vector3 axis = Vector3.zero;

	protected override void Awake()
	{
		base.Awake();
	}

	public void FixedUpdate()
	{
		transform.Rotate(axis, 1);
	}
}
