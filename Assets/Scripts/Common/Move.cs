using UnityEngine;

/// <summary>
/// 指定した軸の方向へ移動する
/// シンプルな移動
/// </summary>
public class Move : MonoBehaviour
{
	/// <summary>
	/// 移動方向軸
	/// </summary>
	public enum Axis
	{
		forward,
		up,
		right
	}

	[SerializeField]
	private Axis direction = Axis.forward; // 移動方向

	private Vector3 directionVec; // 移動ベクトル

	[SerializeField]
	private float speed = 1.0f; // 移動速度

	private void Start()
	{
		// 列挙体から対応するベクトルに変換
		switch (direction)
		{
			case Axis.forward:
				directionVec = Vector3.forward;
				break;
			case Axis.up:
				directionVec = Vector3.up;
				break;
			case Axis.right:
				directionVec = Vector3.right;
				break;
		}
	}

	private void Update()
	{
		transform.Translate(directionVec * Time.deltaTime * speed);
	}
}