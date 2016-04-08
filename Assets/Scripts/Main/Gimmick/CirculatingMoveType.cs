using UnityEngine;

/// <summary>
/// 循環移動
/// </summary>
public sealed class CirculatingMoveType : BaseMoveType
{
	public CirculatingMoveType(Transform target, MovableObjectController.ControlPoint[] controlPoints)
	{
		Initialize(target, controlPoints);
	}

	/// <summary>
	/// 次の制御点を返す
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
			nextControlPointIndex = 0;
		}
		else if (nextControlPointIndex < 0)
		{
			nextControlPointIndex = controlPoints.Length - 1;
		}
	}

	// ぶつかってる時は何もしない
	protected override void Collided()
	{
	}
}