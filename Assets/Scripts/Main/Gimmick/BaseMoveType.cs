using System;
using UnityEngine;

// 足場の動作を担当する基底クラス、実際の動作は派生クラスによって決定される
public abstract class BaseMoveType : ISwitchEvent
{
	public enum MoveType
	{
		None,
		PingPong,
		Switching,
		Circulating,
		OneStep
	}

	protected MovableObjectController.ControlPoint[] controlPoints; // 制御点

	/// <summary>
	/// 現在の状態
	/// </summary>
	public MovableObjectController.State currentState
	{
		get { return _currentState; }
		protected set
		{
			_currentState = value;
			// 現在の状態の動作を行うメソッドを設定
			switch (currentState)
			{
				case MovableObjectController.State.Stop:
					currentStateMethod = Stop;
					break;
				case MovableObjectController.State.Move:
					currentStateMethod = Move;
					break;
				case MovableObjectController.State.Wait:
					currentStateMethod = Wait;
					break;
				case MovableObjectController.State.Collided:
					currentStateMethod = Collided;
					break;
				case MovableObjectController.State.Burying:
					currentStateMethod = Burying;
					break;
				case MovableObjectController.State.None:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}

	protected MovableObjectController.State oldState; // 変更前のState
	protected int currentControlPointIndex = 0;
	protected int nextControlPointIndex = 1;
	protected bool isReverse = false; // 反対方向へ移動中
	protected Transform transform; // 操作対象
	protected float normalPosition = 0.0f; // 現在位置を0~1に正規化した値
	protected float currentWaitSeconds; // 現在の制御点での待ち時間
	protected float waitCount; // 待ち時間カウント用
	protected Bezier bezier; // ベジェ曲線移動用
	protected Action currentStateMethod; // 現在の状態の動作を行うメソッド
	private MovableObjectController.State _currentState;

	public bool switchState { get; protected set; }

	/// <summary>
	/// インスタンスを生成
	/// </summary>
	/// <param name="target">操作対象</param>
	/// <param name="moveType">移動タイプによってインスタンスを生成するクラスを変更</param>
	/// <param name="controlPoints">制御点の配列</param>
	/// <returns></returns>
	public static BaseMoveType Create(Transform target, MoveType moveType, MovableObjectController.ControlPoint[] controlPoints)
	{
		switch (moveType)
		{
			case MoveType.Circulating:
				return new CirculatingMoveType(target, controlPoints);
			case MoveType.OneStep:
				return new OneStepMoveType(target, controlPoints);
			case MoveType.PingPong:
				return new PingPongMoveType(target, controlPoints);
			case MoveType.Switching:
				return new SwitchingMoveType(target, controlPoints);
			case MoveType.None:
				break;
			default:
				throw new ArgumentOutOfRangeException("moveType", moveType, null);
		}
		return null;
	}

	/// <summary>
	/// 初期化
	/// </summary>
	/// <param name="target">操作対象</param>
	/// <param name="controlPoints">制御点</param>
	protected virtual void Initialize(Transform target, MovableObjectController.ControlPoint[] controlPoints)
	{
		currentControlPointIndex = 0;
		nextControlPointIndex = 0;
		this.controlPoints = controlPoints;
		currentState = MovableObjectController.State.None;
		isReverse = false;
		transform = target;
		currentWaitSeconds = 0.0f;
		waitCount = 0.0f;
		ToMove();
		SetNextParams();
		switchState = true;
	}

	// 反対方向へ移動する為に必要なパラメータを設定する
	public void SetReverse(bool isReverse)
	{
		this.isReverse = isReverse;
		normalPosition = 1.0f - normalPosition;
		NextControlPoint();
	}

	/// <summary>
	/// 動作状態を切り替える
	/// </summary>
	/// <param name="newState">新しい動作状態</param>
	public void ChangeState(MovableObjectController.State newState)
	{
		oldState = currentState;
		switch (newState)
		{
			case MovableObjectController.State.Stop: // 停止
				ToStop();
				break;

			case MovableObjectController.State.Move: // 移動
				ToMove();
				break;

			case MovableObjectController.State.Wait: // 一時停止
				ToWait();
				break;

			case MovableObjectController.State.Collided: // 衝突
				ToCollided();
				break;

			case MovableObjectController.State.Burying: // 埋没
				ToBurying();
				break;
		}
	}

	#region - 各状態へ切り替える際の動作。特別な動作をしたい場合は派生クラスで実装

	/// <summary>
	/// 停止状態へ切り替え
	/// </summary>
	protected virtual void ToStop()
	{
		currentState = MovableObjectController.State.Stop;
	}

	/// <summary>
	/// 移動状態へ切り替え
	/// </summary>
	protected virtual void ToMove()
	{
		currentState = MovableObjectController.State.Move;
	}

	/// <summary>
	/// 一時停止状態へ切り替え
	/// </summary>
	protected virtual void ToWait()
	{
		currentWaitSeconds = GetCurrentControlPoint().wait;
		if (currentWaitSeconds == 0)
		{
			return;
		}

		waitCount = 0.0f;
		currentState = MovableObjectController.State.Wait;
	}

	/// <summary>
	/// 衝突状態へ切り替え
	/// </summary>
	protected virtual void ToCollided()
	{
		currentState = MovableObjectController.State.Collided;
	}

	/// <summary>
	/// 埋没状態へ切り替え
	/// </summary>
	protected virtual void ToBurying()
	{
		currentState = MovableObjectController.State.Burying;
	}

	#endregion

	/// <summary>
	/// 状態を更新する
	/// </summary>
	public void Update()
	{
		// 現在の状態に対応する動作を行う
		if (currentStateMethod != null)
			currentStateMethod();
	}

	#region - 各動作状態の実際の動作。特別な動作をしたい場合は派生クラスで実装

	/// <summary>
	/// 停止状態
	/// </summary>
	protected virtual void Stop()
	{
	}

	/// <summary>
	/// 移動状態
	/// </summary>
	protected virtual void Move()
	{
		if (nextControlPointIndex == -1)
		{
			return;
		}

		normalPosition += Time.deltaTime/GetCurrentControlPoint().travelTime;

		normalPosition = Mathf.Clamp01(normalPosition); // 0 ~ 1 に収める

		if (bezier == null)
		{
			transform.localPosition = Vector3.Lerp(GetCurrentControlPoint().position, GetNextControlPoint().position,
				normalPosition);
		}
		else
		{
			transform.localPosition = bezier.GetPointAtTime(normalPosition);
		}
		transform.localScale = Vector3.Lerp(GetCurrentControlPoint().scale, GetNextControlPoint().scale, normalPosition);
		transform.localRotation = Quaternion.Lerp(GetCurrentControlPoint().rotation, GetNextControlPoint().rotation,
			normalPosition);

		// 次の制御点へ到達。Clampしているため条件式はこれで問題ない
		if (normalPosition == 0.0f || normalPosition == 1.0f)
		{
			Reaching();
		}
	}

	/// <summary>
	/// 一時停止状態
	/// </summary>
	protected virtual void Wait()
	{
		if (waitCount >= currentWaitSeconds)
		{
			ChangeState(MovableObjectController.State.Move);
		}
		else
		{
			waitCount += Time.deltaTime;
		}
	}

	/// <summary>
	/// 衝突状態
	/// </summary>
	protected virtual void Collided()
	{
		// 衝突時、何もしない
	}

	/// <summary>
	/// 埋没状態
	/// </summary>
	protected virtual void Burying()
	{
		// 埋没時、何もしない
	}

	/// <summary>
	/// 衝突していたものが無くなった
	/// </summary>
	public virtual void CollisionDisappearance()
	{
		ChangeState(MovableObjectController.State.Move);
	}

	#endregion


	/// <summary>
	/// 次の制御点へ到達
	/// </summary>
	protected virtual void Reaching()
	{
		SetNextParams();
		ChangeState(MovableObjectController.State.Wait);
		normalPosition = 0.0f;
	}

	/// <summary>
	/// 次の制御点を返す
	/// </summary>
	/// <returns></returns>
	protected abstract void NextControlPoint();

	/// <summary>
	/// 次の制御点を計算し、パラメータに適切な値を設定する
	/// </summary>
	protected void SetNextParams()
	{
		NextControlPoint();
		SetNextBezierCurve();
	}

	/// <summary>
	/// ベジェ曲線計算用のパラメータを設定
	/// </summary>
	private void SetNextBezierCurve()
	{
		if (nextControlPointIndex == -1)
		{
			return;
		}

		Vector3 prevHandle;
		Vector3 nextHandle;

		if (!isReverse)
		{
			prevHandle = GetNextControlPoint().prevHandle;
			nextHandle = GetCurrentControlPoint().nextHandle;
		}
		else
		{
			prevHandle = GetNextControlPoint().nextHandle;
			nextHandle = GetCurrentControlPoint().prevHandle;
		}

		// 制御点がベジェ曲線かどうかを判定
		if (nextHandle != GetCurrentControlPoint().position || prevHandle != GetNextControlPoint().position)
		{
			bezier = new Bezier();
			bezier.p0 = GetCurrentControlPoint().position;
			bezier.p1 = nextHandle;
			bezier.p2 = prevHandle;
			bezier.p3 = GetNextControlPoint().position;
		}
		else
		{
			bezier = null;
		}
	}

	/// <summary>
	/// 現在の制御点を取得
	/// </summary>
	/// <returns></returns>
	public MovableObjectController.ControlPoint GetCurrentControlPoint()
	{
		return controlPoints[currentControlPointIndex];
	}

	/// <summary>
	/// 次の制御点を取得
	/// </summary>
	/// <returns></returns>
	public MovableObjectController.ControlPoint GetNextControlPoint()
	{
		return controlPoints[nextControlPointIndex];
	}

	/// <summary>
	/// スイッチの状態を切り替える
	/// スイッチに反応する派生クラスが多いことが分かっているので
	/// ここでISwichEventインターフェイスを実装しておく
	/// 無効にしたければ派生クラス側で何もしないメソッドとしてオーバーライドする
	/// </summary>
	public virtual void Switch()
	{
		if (switchState)
		{
			ChangeState(MovableObjectController.State.Stop);
		}
		else
		{
			ChangeState(oldState);
		}

		switchState = !switchState;
	}
}