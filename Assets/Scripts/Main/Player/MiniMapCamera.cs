using UnityEngine;

public class MiniMapCamera : MyMonoBehaviour {
	private Vector3 defaultPosition; // 自身の初期位置
	private Quaternion defaultRotation; // 自身の初期アングル
	private Vector3 playerDefaultPosition; // 追尾対象の初期位置

	void Start()
	{
		defaultPosition = transform.position;
		defaultRotation = transform.rotation;
		playerDefaultPosition = player.transform.position;

	}

	protected override void LateUpdate()
	{
		// 全ての移動が終わったこのタイミングで座標を反映
		transform.position = defaultPosition + (player.transform.position - playerDefaultPosition);
		transform.rotation = defaultRotation;
	}
}
