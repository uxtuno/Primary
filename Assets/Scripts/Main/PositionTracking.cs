using UnityEngine;

/// <summary>
/// 設定したオブジェクトを追尾させる
/// </summary>
public class PositionTracking : MyMonoBehaviour {
	[SerializeField]
	private Transform target = null; // 追尾対象
	private Vector3 defaultPosition; // 自身の初期位置
	private Vector3 targetDefaultPosition; // 追尾対象の初期位置

	void Start () {
		defaultPosition = transform.position;
		targetDefaultPosition = target.transform.position;
	}
	
	protected override void LateUpdate () {
		// 全ての移動が終わったこのタイミングで座標を反映
		transform.position = defaultPosition + (target.position - targetDefaultPosition);
	}
}
