using UnityEngine;
using System.Collections;

/// <summary>
/// ミニマップのアイコンを常にミニマップ用カメラの方向を向くようにする
/// </summary>
public class MiniMapIconBillboard : MonoBehaviour {
	private Camera targetCamera;
	private Quaternion defaultRotation;
	Vector3 toCamera;

	void Start()
	{
		targetCamera = GameObject.FindGameObjectWithTag(Tags.MiniMapCamera).GetComponent<Camera>();
		toCamera = targetCamera.transform.position - transform.position;
		defaultRotation = transform.rotation;
	}

	void Update()
	{
		transform.forward = Vector3.up;
	}

	void LateUpdate()
	{
		Vector3 angles = transform.eulerAngles;
		angles.y = targetCamera.transform.eulerAngles.y + 180.0f;
		transform.eulerAngles = angles;
    }
}
