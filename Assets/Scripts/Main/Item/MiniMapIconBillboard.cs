using UnityEngine;
using System.Collections;

/// <summary>
/// ミニマップのアイコンを常にミニマップ用カメラの方向を向くようにする
/// </summary>
public class MiniMapIconBillboard : MonoBehaviour {
	private Camera targetCamera;
	private Quaternion defaultRotation;

	void Start()
	{
		targetCamera = GameObject.FindGameObjectWithTag(Tags.MiniMapCamera).GetComponent<Camera>();
		defaultRotation = transform.rotation;
	}

	void Update()
	{
		transform.forward = Vector3.up; ;
	}

	void LateUpdate()
	{
		transform.rotation = defaultRotation;
	}
}
