using UnityEngine;
using System.Collections;

/// <summary>
/// ミニマップのアイコンを常にミニマップ用カメラの方向を向くようにする
/// </summary>
public class MiniMapIconBillboard : MonoBehaviour
{
	private Camera targetCamera;
	//Vector3 toCamera;

	void Start()
	{
		var cameraGo = GameObject.FindGameObjectWithTag(Tags.MiniMapCamera);
		if (cameraGo)
		{
			targetCamera = cameraGo.GetComponent<Camera>();
			//toCamera = targetCamera.transform.position - transform.position;
		}
	}

	void Update()
	{
		transform.forward = Vector3.up;
	}

	void LateUpdate()
	{
		if (!targetCamera)
			return;

		Vector3 angles = transform.eulerAngles;
		angles.y = targetCamera.transform.eulerAngles.y + 180.0f;
		transform.eulerAngles = angles;
	}
}
