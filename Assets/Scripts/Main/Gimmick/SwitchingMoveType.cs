using UnityEngine;

/// <summary>
/// スイッチの状態によって移動方向が変わる
/// </summary>
public sealed class SwitchingMoveType : BaseMoveType
{
	public SwitchingMoveType(Transform target, MovableObjectController.ControlPoint[] controlPoints)
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
			nextControlPointIndex = currentControlPointIndex;
		}
		else if (nextControlPointIndex < 0)
		{
			nextControlPointIndex = currentControlPointIndex;
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
		if (nextControlPointIndex == currentControlPointIndex)
		{
			ChangeState(MovableObjectController.State.Stop);
			// 折り返し処理用に終点まで到達した時はlinePositionを戻さない
		}
		else
		{
			ChangeState(MovableObjectController.State.Wait);
			normalPosition = 0.0f;
		}
	}

	/// <summary>
	/// スイッチの状態を切り替える
	/// </summary>
	public override void Switch()
	{
		switchState = !switchState;
		normalPosition = 1.0f - normalPosition; // 折り返した時の位置を再計算
		isReverse = !isReverse;
		SetNextParams();
		//Debug.Log(currentState);

		// SwitchingのStateがStopの場合は終点に居るという事
		// 終点の時にスイッチが押された場合強制的に移動開始させる
		// Waitの場合は一定時間待機後、自動で移動開始するので問題ない
		if (currentState == MovableObjectController.State.Stop)
		{
			ChangeState(MovableObjectController.State.Move);
		}
		else if (currentState == MovableObjectController.State.Wait)
		{
			SetNextParams();
			normalPosition = 0.0f;
		}
		else if (currentState == MovableObjectController.State.Collided)
		{
			ChangeState(MovableObjectController.State.Move);
		}
	}
}