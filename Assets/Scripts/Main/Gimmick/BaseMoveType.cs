using UnityEngine;
using System.Collections;
using System;

// 足場の動作を担当する基底クラス、実際の動作は派生クラスによって決定される
public abstract class BaseMoveType : ISwitchEvent
{
	protected MovableObjectController.ControlPoint[] controlPoints; // 制御点
	protected MovableObjectController.State _currentState; // 
	public MovableObjectController.State currentState
	{
		get { return _currentState; }
		private set { _currentState = value; }
	}
	protected MovableObjectController.State oldState; // 変更前のState
	protected int currentControlPointIndex = 0;
	protected int nextControlPointIndex = 1;
	protected bool isReverse = false; // 反対方向へ移動中
	protected Transform transform; // 操作対象
	protected float linePosition = 0.0f; // 移動線上の位置(0.0f <= x <= 1.0f)
	protected float currentWaitTime; // 現在の制御点での待ち時間
	protected float waitTimeCount; // 待ち時間カウント用
	protected Bezier bezier; // ベジェ曲線移動用

	private bool _switchState = false;
	public bool switchState
	{
		get
		{
			return _switchState;
		}
		protected set
		{
			_switchState = value;
		}
	}

	// 
	public static BaseMoveType Create(Transform target, MovableObjectController.MoveType moveType, MovableObjectController.ControlPoint[] controlPoints)
	{
		switch (moveType)
		{
			case MovableObjectController.MoveType.Circulating:
				return new CirculatingMoveType(target, controlPoints);
			case MovableObjectController.MoveType.OneStep:
				return new OneStepMoveType(target, controlPoints);
			case MovableObjectController.MoveType.PingPong:
				return new PingPongMoveType(target, controlPoints);
			case MovableObjectController.MoveType.Switching:
				return new SwitchingMoveType(target, controlPoints);
		}
		return null;
	}

	/// <summary>
	/// 初期化
	/// </summary>
	/// <param name="target">操作対象</param>
	/// <param name="controlPoints">制御点</param>
	public virtual void Initialize(Transform target, MovableObjectController.ControlPoint[] controlPoints)
	{
		currentControlPointIndex = 0;
		nextControlPointIndex = 0;
		this.controlPoints = controlPoints;
		currentState = MovableObjectController.State.None;
		isReverse = false;
		this.transform = target;
		currentWaitTime = 0.0f;
		waitTimeCount = 0.0f;
		ToMove();
		SetNextParams();
		switchState = true;
	}

	// 反対方向へ移動する為に必要なパラメータを設定する
	public void SetReverse(bool isReverse)
	{
		this.isReverse = isReverse;
		linePosition = 1.0f - linePosition;
		isReverse = !isReverse;
		ConputeNextControlPoint();
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
			case MovableObjectController.State.Stop:  // 停止
				ToStop();
				break;

			case MovableObjectController.State.Move:  // 移動
				ToMove();
				break;

			case MovableObjectController.State.Wait:  // 一時停止
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

	// 各状態へ切り替える際の動作
	// 派生クラス側でオーバーライドして定義
	protected virtual void ToStop() { currentState = MovableObjectController.State.Stop; }
	protected virtual void ToMove()
	{
		currentState = MovableObjectController.State.Move;
	}
	protected virtual void ToWait()
	{
		currentWaitTime = GetCurrentControlPoint().wait;
		if (currentWaitTime == 0)
		{
			return;
		}

		waitTimeCount = 0.0f;
		currentState = MovableObjectController.State.Wait;
	}
	protected virtual void ToCollided() { currentState = MovableObjectController.State.Collided; }
	protected virtual void ToBurying() { currentState = MovableObjectController.State.Burying; }

	/// <summary>
	/// 状態を更新する
	/// </summary>
	public void Update()
	{
		switch (currentState)
		{
			case MovableObjectController.State.Stop:  // 停止中
				Stop();
				break;

			case MovableObjectController.State.Move:  // 移動中
				Move();
				break;

			case MovableObjectController.State.Wait:  // 一時停止中
				Wait();
				break;

			case MovableObjectController.State.Collided: // 衝突中
				Collided();
				break;

			case MovableObjectController.State.Burying: // 埋没中
				Burying();
				break;
		}
	}

	// 各動作状態の実際の動作
	// 特別な動作をしたい場合は派生クラスで実装
	protected virtual void Stop() { }
	protected virtual void Move()
	{
		if (nextControlPointIndex == -1)
		{
			return;
		}

		linePosition += Time.deltaTime / GetCurrentControlPoint().travelTime;

		linePosition = Mathf.Clamp01(linePosition); // 0 ~ 1 に収める

		if (bezier == null)
		{
			transform.localPosition = Vector3.Lerp(GetCurrentControlPoint().position, GetNextControlPoint().position, linePosition);
		}
		else
		{
			transform.localPosition = bezier.GetPointAtTime(linePosition);
		}
		transform.localScale = Vector3.Lerp(GetCurrentControlPoint().scale, GetNextControlPoint().scale, linePosition);
		transform.localRotation = Quaternion.Lerp(GetCurrentControlPoint().rotation, GetNextControlPoint().rotation, linePosition);

		// 次の制御点へ到達。Clampしているため条件式はこれで問題ない
		if (linePosition == 0.0f || linePosition == 1.0f)
		{
			Reaching();
		}
	}
	protected virtual void Wait()
	{
		if (waitTimeCount >= currentWaitTime)
		{
			ChangeState(MovableObjectController.State.Move);
		}
		else
		{
			waitTimeCount += Time.deltaTime;
		}
	}
	protected virtual void Collided()
	{
	}
	protected virtual void Burying() { }

	/// <summary>
	/// 衝突していたものが無くなった
	/// </summary>
	public virtual void CollisionDisappearance()
	{
		ChangeState(MovableObjectController.State.Move);
	}

	/// <summary>
	/// 次の制御点へ到達
	/// </summary>
	protected virtual void Reaching()
	{
		SetNextParams();
		ChangeState(MovableObjectController.State.Wait);
		linePosition = 0.0f;
	}

	/// <summary>
	/// 次の制御点を計算
	/// </summary>
	/// <returns></returns>
	public abstract void ConputeNextControlPoint();

	/// <summary>
	/// 次の制御点を計算し、パラメータに適切な値を設定する
	/// </summary>
	protected void SetNextParams()
	{
		ConputeNextControlPoint();
		ConputeNextBezierCurve();
	}

	/// <summary>
	/// ベジェ曲線計算用のパラメータを設定
	/// </summary>
	private void ConputeNextBezierCurve()
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
	public MovableObjectController.ControlPoint GetCurrentControlPoint() { return controlPoints[currentControlPointIndex]; }

	/// <summary>
	/// 次の制御点を取得
	/// </summary>
	/// <returns></returns>
	public MovableObjectController.ControlPoint GetNextControlPoint() { return controlPoints[nextControlPointIndex]; }

	// スイッチが押された時の動作。デフォルトでは、「停止/移動」を切り替える動作
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

/// <summary>
/// 最後まで到達すると来た道を戻るような移動
/// </summary>
public class PingPongMoveType : BaseMoveType, ISwitchEvent
{
	public PingPongMoveType(Transform target, MovableObjectController.ControlPoint[] controlPoints)
	{
		Initialize(target, controlPoints);
	}

	public override void ConputeNextControlPoint()
	{
		int returnIndex = nextControlPointIndex; // 戻り値用
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
		linePosition = 1.0f - linePosition;
		isReverse = !isReverse;
		SetNextParams();
		ToWait();
	}

	public override void CollisionDisappearance()
	{
		// 何もしない
	}
}

/// <summary>
/// 循環移動
/// </summary>
public class CirculatingMoveType : BaseMoveType
{
	public CirculatingMoveType(Transform target, MovableObjectController.ControlPoint[] controlPoints)
	{
		Initialize(target, controlPoints);
	}

	public override void ConputeNextControlPoint()
	{
		int returnIndex = nextControlPointIndex; // 戻り値用
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

/// <summary>
/// スイッチの状態によって移動方向が変わる
/// </summary>
public class SwitchingMoveType : BaseMoveType
{
	public SwitchingMoveType(Transform target, MovableObjectController.ControlPoint[] controlPoints)
	{
		Initialize(target, controlPoints);
	}

	public override void Initialize(Transform target, MovableObjectController.ControlPoint[] controlPoints)
	{
		base.Initialize(target, controlPoints);
	}

	public override void ConputeNextControlPoint()
	{
		int returnIndex = nextControlPointIndex; // 戻り値用
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

	protected override void Collided()
	{
	}

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
			linePosition = 0.0f;
		}
	}

	public override void Switch()
	{
		switchState = !switchState;
		linePosition = 1.0f - linePosition; // 折り返した時の位置を再計算
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
			linePosition = 0.0f;
		}
		else if(currentState == MovableObjectController.State.Collided)
		{
			ChangeState(MovableObjectController.State.Move);
		}
	}
}

/// <summary>
/// Action()が呼ばれた時に一区間だけ移動する
/// </summary>
public class OneStepMoveType : BaseMoveType, IActionEvent
{
	public OneStepMoveType(Transform target, MovableObjectController.ControlPoint[] controlPoints)
	{
		Initialize(target, controlPoints);
	}

	public override void Initialize(Transform target, MovableObjectController.ControlPoint[] controlPoints)
	{
		base.Initialize(target, controlPoints);
		ChangeState(MovableObjectController.State.Stop);
	}

	public override void ConputeNextControlPoint()
	{
		int returnIndex = nextControlPointIndex; // 戻り値用
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

	protected override void Collided()
	{
	}

	protected override void Reaching()
	{
		SetNextParams();
		ChangeState(MovableObjectController.State.Stop);
	}

	public override void Switch()
	{
	}

	public void Action()
	{
		linePosition = 0.0f;
		ChangeState(MovableObjectController.State.Move);
	}
}
