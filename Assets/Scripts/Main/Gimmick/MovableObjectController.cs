using System;
using UnityEngine;

public class MovableObjectController : Gimmick, ISwitchEvent, IActionEvent
{
	public enum MoveType
	{
		None,
		PingPong,
		Switching,
		Circulating,
		OneStep,
	}

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
		public float wait;   // 待機時間
		public float travelTime; // 次の制御点へ移動するのにかかる時間
								 //[HideInInspector]
		public Vector3 prevHandle; // 前のハンドル位置
								   //[HideInInspector]
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
	///// </summary>
	public enum State
	{
		None,       // 状態無し
		Stop,       // 停止
		Move,       // 移動中
		Wait,       // 一時停止
		Collided,   // 衝突中
		Burying,    // 埋没中
	}

	#region - インスペクタ指定用
	[SerializeField]
	private MoveType moveType = MoveType.PingPong; // 移動タイプ
	[SerializeField]
	private bool playOnAwake = true; // 開始時に動いているかどうか
	[SerializeField]
	private ControlPoint[] controlPoints = null; // 制御点
	#endregion

	private Transform scaffoldsTransform;   // 足場
	private BaseMoveType movableObject;

	//private int currentControlPointIndex = 0;   // 現在の制御点。次の制御点に達した時に切り替わる
	//private int nextControlPointIndex = 0;   // 次の制御点

	//private State _currentState = State.None;
	//private float currentWaitTime;  // 現在の制御点での待ち時間
	//private float waitCounter;      // 待ち時間のカウント用
	//private bool isReturn; // 折り返し移動中
	//private bool isCurrentBezier = false; // 現在の制御点がベジェ曲線モードか
	private static readonly int n = 50;
	private Bezier bezier = new Bezier(n); // ベジェ曲線制御用

	//private static readonly float allowableDistance = 0.5f; // 足場を離れたと判断するための距離

	/// <summary>
	/// 現在の足場の状態
	/// </summary>
	public State currentState
	{
		get { return MovableObject.currentState; }
	}

	private static readonly float defaultWait = 3.0f;   // デフォルトの停止時間
	private static readonly float defaultSpeed = 3.0f;  // デフォルトの速度
	//private float elapsedTime = 0.0f; // 現在の制御点から次の制御点へ移動する際の経過時間(0~1)

	private bool _switchState = false; // switchStateのバッキングフィールド

	/// <summary>
	/// スイッチの状態
	/// </summary>
	public bool switchState
	{
		get { return _switchState; }
		set { _switchState = value; }
	}

	/// <summary>
	/// 制御対象オブジェクトを取得
	/// </summary>
	public BaseMoveType MovableObject
	{
		get
		{
			return movableObject;
		}
	}

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
				if (controlPoints[controlPointIndex] == null) // 続けて書く場合、前回のループで生成したものがそのまま使えるはずなのでnullの時のみ生成
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

	void OnDrawGizmos()
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
		Quaternion initRotation = transform.GetChild(0).rotation;
		scaffoldsTransform = transform.GetChild(0);
		scaffoldsTransform.gameObject.AddComponent<MovableObject>();
		movableObject = BaseMoveType.Create(scaffoldsTransform, moveType, controlPoints);

		foreach (Transform child in transform)
		{
			if (scaffoldsTransform != child)
			{
				Destroy(child.gameObject);
			}
		}

		scaffoldsTransform.parent = transform;

		if (useSounds.Count == 0)
		{
			useSounds.Add(SoundCollector.SoundName.Driving5);
		}

		//currentControlPointIndex = 0;
		//nextControlPointIndex = 0;

		// シーン開始時の状態
		if (playOnAwake)
		{
			// 開始時から起動している
			//SoundPlayerSingleton.instance.PlaySE(scaffoldsTransform.gameObject, soundCollector[useSounds[0]], true);
			switchState = true;
		}
		else
		{
			// デフォルトがオンなのでオフにするために呼ぶ
			MovableObject.Switch();
			switchState = false;
		}
	}

	void FixedUpdate()
	{
		MovableObject.Update();
	}

	/// <summary>
	/// 衝突した
	/// </summary>
	public void Collision()
	{
		MovableObject.ChangeState(State.Collided);
	}

	/// <summary>
	/// 衝突していたものが消えた
	/// </summary>
	public void CollisionDisappearance()
	{
		MovableObject.CollisionDisappearance();
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
		MovableObject.Switch();

		ShowPickIcon(transform.TransformPoint(controlPoints[0].position));
	}

	public void Action()
	{
		if (moveType != MoveType.OneStep)
		{
			return;
		}
		(MovableObject as IActionEvent).Action();
	}
}
