using UnityEngine;

/// <summary>
/// Action()が呼ばれた時に一区間だけ移動する
/// </summary>
public sealed class OneStepMoveType : BaseMoveType, IActionEvent
{
	public OneStepMoveType(Transform target, MovableObjectController.ControlPoint[] controlPoints)
	{
		Initialize(target, controlPoints);
	}

	/// <summary>
	/// 初期化
	/// </summary>
	/// <param name="target">操作対象</param>
	/// <param name="controlPoints">制御点</param>
	protected override void Initialize(Transform target, MovableObjectController.ControlPoint[] controlPoints)
	{
		base.Initialize(target, controlPoints);
		ChangeState(MovableObjectController.State.Stop);
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

	/// <summary>
	/// 衝突状態
	/// </summary>
	protected override void Collided()
	{
	}

	/// <summary>
	/// 次の制御点へ到達
	/// </summary>
	protected override void Reaching()
	{
		SetNextParams();
		ChangeState(MovableObjectController.State.Stop);
	}

	/// <summary>
	/// スイッチの状態を切り替える
	/// </summary>
	public override void Switch()
	{
	}

	/// <summary>
	/// 動作を起こす
	/// </summary>
	public void Action()
	{
		normalPosition = 0.0f;
		ChangeState(MovableObjectController.State.Move);
	}
}