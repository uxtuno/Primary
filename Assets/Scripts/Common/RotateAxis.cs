using UnityEngine;

/// <summary>
///     単純な回転
/// </summary>
public class RotateAxis : MonoBehaviour
{
	/// <summary>
	/// 回転軸
	/// </summary>
	public enum Axis
	{
		forward,
		up,
		right
	}

	[SerializeField]
	private Axis axis = Axis.forward; // 回転軸
	private Vector3 axisVec; // 軸ベクトル

	[SerializeField]
	private float speed = 1.0f; // 回転速度

	private void Start()
	{
		// 回転軸をVactor3として格納
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

	private void Update()
	{
		transform.Rotate(axisVec, Time.deltaTime * speed);
	}
}