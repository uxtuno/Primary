using UnityEngine;

/// <summary>
///     最後まで到達すると来た道を戻るような移動
/// </summary>
public sealed class PingPongMoveType : BaseMoveType
{
	public PingPongMoveType(Transform target, MovableObjectController.ControlPoint[] controlPoints)
	{
		Initialize(target, controlPoints);
	}

	/// <summary>
	///     次の制御点を返す
	/// </summary>
	/// <returns></returns>
	protected override void NextControlPoint()
	{
		currentControlPointIndex = nextControlPointIndex;

		int increasingDirection; // 制御点の増加方向(プラスか、マイナスか)
		if (!isReverse)
		{
			increasingDirection = 1;
		}
		else
		{
			increasingDirection = -1;
		}

		// 次の制御点を求めておく
		nextControlPointIndex += increasingDirection;
		if (nextControlPointIndex >= controlPoints.Length)
		{
			nextControlPointIndex = currentControlPointIndex - 1;
			isReverse = true;
		}
		else if (nextControlPointIndex < 0)
		{
			nextControlPointIndex = currentControlPointIndex + 1;
			isReverse = false;
		}
	}

	// 衝突時、一旦停止後向きを変えて移動
	protected override void Collided()
	{
		normalPosition = 1.0f - normalPosition;
		isReverse = !isReverse;
		SetNextParams();
		ToWait();
	}

	public override void CollisionDisappearance()
	{
		// 何もしない
	}
}