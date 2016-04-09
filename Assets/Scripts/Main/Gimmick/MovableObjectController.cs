using UnityEngine;

/// <summary>
/// 移動オブジェクトの制御
/// </summary>
public class MovableObjectController : Gimmick, ISwitchEvent, IActionEvent
{
	/// <summary>
	/// 制御点
	/// </summary>
	[System.Serializable]
	public class ControlPoint
	{
		[HideInInspector]
		public Vector3 position; // 位置
		public Quaternion rotation; // 回転
		[HideInInspector]
		public Vector3 scale = Vector3.one; // この制御点へ到達した時点でのスケール
		public float wait; // 待機時間
		public float travelTime; // 次の制御点へ移動するのにかかる時間
		public Vector3 prevHandle; // 前のハンドル位置
		public Vector3 nextHandle; // 次のハンドル位置

		public ControlPoint(Vector3 position, Vector3 scale, Quaternion rotation)
		{
			Initialize(position, scale, rotation);
		}

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="position">座標</param>
		/// <param name="scale">サイズ</param>
		/// <param name="rotation">角度情報</param>
		public void Initialize(Vector3 position, Vector3 scale, Quaternion rotation)
		{
			this.position = position;
			this.rotation = rotation;
			prevHandle = position;
			nextHandle = position;
			this.scale = scale;

			wait = defaultWait;
			travelTime = defaultSpeed;
		}
	}

	/// <summary>
	/// 現在の移動状態
	/// </summary>
	public enum State
	{
		None, // 状態無し
		Stop, // 停止
		Move, // 移動中
		Wait, // 一時停止
		Collided, // 衝突中
		Burying, // 埋没中
	}

	#region - インスペクタ指定用

	[SerializeField]
	private BaseMoveType.MoveType moveType = BaseMoveType.MoveType.PingPong; // 移動タイプ
	[SerializeField]
	private bool playOnAwake = true; // 開始時に動いているかどうか
	[SerializeField]
	private ControlPoint[] controlPoints = null; // 制御点

	#endregion

	private Transform controlObjectTransform; // 足場

	/// <summary>
	/// 現在の足場の状態
	/// </summary>
	public State currentState
	{
		get { return movableObject.currentState; }
	}

	private static readonly float defaultWait = 3.0f; // デフォルトの停止時間
	private static readonly float defaultSpeed = 3.0f; // デフォルトの速度

	/// <summary>
	/// スイッチの状態
	/// </summary>
	public bool switchState { get; set; }

	/// <summary>
	/// 制御対象オブジェクトを取得
	/// </summary>
	public BaseMoveType movableObject { get; private set; }

	// 自動でインスペクタの値を設定する
	[ContextMenu("SetAuto")]
	public void AutoSetContext()
	{
		ControlPoint[] oldControlPoints = controlPoints;

		// 生成する制御点の数をカウント
		int createCount = 0;
		int index = 0;
		while (index < transform.childCount)
		{
			if (transform.GetChild(index).GetComponent<CurveNode>() != null)
			{
				createCount++;
				index += 3;
			}
			else
			{
				createCount++;
				index++;
			}
		}

		// 生成
		controlPoints = new ControlPoint[createCount];

		// 生成後、正しい値で初期化
		index = 0;
		int controlPointIndex = 0;
		while (index < transform.childCount)
		{
			Transform child = transform.GetChild(index);

			if (child.GetComponent<CurveNode>() != null)
			{
				Transform begin = child;
				Transform end = transform.GetChild(index + 3);
				if (controlPoints[controlPointIndex] == null) // 前回のループで生成したものがそのまま使えるはずなのでnullの時のみ生成
				{
					controlPoints[controlPointIndex] = new ControlPoint(begin.localPosition, begin.localScale, begin.localRotation);
					controlPoints[controlPointIndex].nextHandle = transform.GetChild(index + 1).localPosition;
					if (oldControlPoints.Length >= controlPointIndex)
					{
						controlPoints[controlPointIndex].wait = oldControlPoints[controlPointIndex].wait;
						controlPoints[controlPointIndex].travelTime = oldControlPoints[controlPointIndex].travelTime;
					}
				}
				else
				{
					controlPoints[controlPointIndex].nextHandle = transform.GetChild(index + 1).localPosition;
				}

				controlPoints[controlPointIndex + 1] = new ControlPoint(end.localPosition, end.localScale, end.localRotation);
				controlPoints[controlPointIndex + 1].prevHandle = transform.GetChild(index + 2).localPosition;
				if (oldControlPoints.Length >= controlPointIndex)
				{
					controlPoints[controlPointIndex].wait = oldControlPoints[controlPointIndex].wait;
					controlPoints[controlPointIndex].travelTime = oldControlPoints[controlPointIndex].travelTime;
				}
				controlPointIndex++;
				index += 3;
			}
			else
			{
				if (controlPoints[controlPointIndex] == null)
				{
					controlPoints[controlPointIndex] = new ControlPoint(child.localPosition, child.localScale, child.localRotation);
					if (oldControlPoints.Length >= controlPointIndex)
					{
						controlPoints[controlPointIndex].wait = oldControlPoints[controlPointIndex].wait;
						controlPoints[controlPointIndex].travelTime = oldControlPoints[controlPointIndex].travelTime;
					}
				}
				controlPointIndex++;
				index++;
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (transform.childCount == 0)
		{
			return;
		}

		Gizmos.color = Color.red;

		int index = 0;
		const float radius = 0.2f;

		const int n = 50; // 頂点数
		Bezier bezier = new Bezier(n);

		if (Application.isPlaying)
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(controlPoints[index].position + transform.position, radius);
			while (index < controlPoints.Length - 1)
			{
				bezier.p0 = controlPoints[index + 0].position + transform.position;
				bezier.p1 = controlPoints[index + 0].nextHandle + transform.position;
				bezier.p2 = controlPoints[index + 1].prevHandle + transform.position;
				bezier.p3 = controlPoints[index + 1].position + transform.position;

				// ベジェ曲線による移動ルートを書く
				if (bezier.CheckBezier())
				{
					Vector3[] bezierPositions = bezier.GetAllPoint();

					Gizmos.color = Color.red;
					for (int i = 0; i < bezierPositions.Length - 1; i++)
					{
						Gizmos.DrawLine(bezierPositions[i], bezierPositions[i + 1]);
					}
				}
				else
				{
					// 直線による移動ルートを書く
					Gizmos.color = Color.red;
					Gizmos.DrawLine(bezier.p0, bezier.p3);
				}
				index++;
			}
		}
		else
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawSphere(transform.GetChild(0).position, radius);
			while (index < transform.childCount - 1)
			{
				// ベジェ曲線による移動ルートを書く
				if (transform.GetChild(index + 0).GetComponent<CurveNode>() != null)
				{
					bezier.p0 = transform.GetChild(index + 0).position;
					bezier.p1 = transform.GetChild(index + 1).position;
					bezier.p2 = transform.GetChild(index + 2).position;
					bezier.p3 = transform.GetChild(index + 3).position;
					Vector3[] bezierPositions = bezier.GetAllPoint();

					Gizmos.color = Color.red;
					for (int i = 0; i < bezierPositions.Length - 1; i++)
					{
						Gizmos.DrawLine(bezierPositions[i], bezierPositions[i + 1]);
					}

					Gizmos.color = Color.blue;
					Gizmos.DrawLine(bezier.p0, bezier.p1);
					Gizmos.DrawLine(bezier.p3, bezier.p2);
					index += 3;
				}
				else
				{
					Gizmos.color = Color.red;
					Gizmos.DrawLine(transform.GetChild(index + 0).position, transform.GetChild(index + 1).position);
					index++;
				}
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		// 操作するオブジェクトを取得
		controlObjectTransform = transform.GetChild(0);
		controlObjectTransform.gameObject.AddComponent<MovableObject>();
		movableObject = BaseMoveType.Create(controlObjectTransform, moveType, controlPoints);

		// 実行時、操作するオブジェクト以外の子を削除
		foreach (Transform child in transform)
		{
			if (controlObjectTransform != child)
			{
				Destroy(child.gameObject);
			}
		}

		controlObjectTransform.parent = transform;

		if (useSounds.Count == 0)
		{
			useSounds.Add(SoundCollector.SoundName.Driving5);
		}

		// シーン開始時の状態
		if (playOnAwake)
		{
			// 開始時から起動している
			switchState = true;
		}
		else
		{
			// デフォルトがオンなのでオフにするために呼ぶ
			movableObject.Switch();
			switchState = false;
		}
	}

	void FixedUpdate()
	{
		if (movableObject != null)
		{
			movableObject.Update();
		}
	}

	/// <summary>
	/// 衝突した
	/// </summary>
	public void Collision()
	{
		movableObject.ChangeState(State.Collided);
	}

	/// <summary>
	/// 衝突していたものが消えた
	/// </summary>
	public void CollisionDisappearance()
	{
		movableObject.CollisionDisappearance();
	}

	/// <summary>
	/// 埋まった
	/// </summary>
	public void Burying()
	{
	}

	public void Switch()
	{
		switchState = !switchState;
		movableObject.Switch();

		if (pickIconShowPosition == null)
			ShowPickIcon(transform.TransformPoint(controlPoints[0].position), controlObjectTransform);
		else
			ShowPickIcon(pickIconShowPosition.position);
	}

	public void Action()
	{
		if (moveType != BaseMoveType.MoveType.OneStep)
		{
			return;
		}
		var actionEvent = movableObject as IActionEvent;
		if (actionEvent != null) actionEvent.Action();
	}
}