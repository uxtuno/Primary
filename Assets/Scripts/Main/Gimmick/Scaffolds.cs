using UnityEngine;

/// <summary>
///     足場
///     判定のため、足場の表面にIsTriggerのコライダが必要
///		本体はこのスクリプトがアタッチされているオブジェクトの親
/// </summary>
public class Scaffolds : MyMonoBehaviour
{
	private static readonly float overCapacityTime = 0.1f; // 埋まってないと判定するための時間
	private float overCapacityTimeCount = 0.0f;

	private ColorObjectBase collisionColorObject = null; // 現在衝突しているカラーオブジェクト

	private MovableObjectController controller; // 足場を操作するスクリプト
	private bool isNotOverChange = false; // 埋まってない状態になろうとしている
	private bool isOver = false; // 埋まっている
	private bool isStop = false; // 停止している

	/// <summary>
	/// 足場の本体
	/// </summary>
	public Transform scaffoldingBody { get; protected set; }

	protected override void Awake()
	{
		base.Awake();
		//rideOnTags = new string[] { Tags.Player };

		// RigidBodyの初期状態を記録する
		if (rigidbody != null)
		{
			rigidbody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
		}

		// 本体はこのスクリプトがアタッチされているオブジェクトの親
		scaffoldingBody = transform.parent;
	}

	private void Start()
	{
		// この足場を操作しているスクリプトを取得
		controller = GetComponentInParent<MovableObjectController>();
	}

	protected override void Update()
	{
		if (collisionColorObject == null)
			return;

		// ぶつかっていたカラーオブジェクトが消失したので
		// コントローラへ通知する
		if (collisionColorObject.isDisappearance)
		{
			controller.CollisionDisappearance();
			collisionColorObject = null;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<ColorBlock>() != null)
		{
			// カラーブロックが消失状態で重なっている
			isOver = true;
			overCapacityTimeCount = 0.0f;
		}

		if (other.tag != Tags.RideOn)
			return;
		
		// ぶつかったオブジェクトが掴めるものならオブジェクトの構造が違うので特殊処理
		var graspItem = other.transform.GetComponentInParent<GraspItem>();
		if (graspItem != null)
		{
			if (!graspItem.isGrasp)
			{
				other.transform.parent.parent = scaffoldingBody;
			}
		}
		else if (other.GetComponentInParent<Player>() != null)
		{
			// プレイヤーが乗ったのでプレイヤーに対する設定
			player.transform.parent = scaffoldingBody;
			player.rideScaffolds = this;
			player.isRide = true;
		}
		else
		{
			other.transform.parent.parent = scaffoldingBody;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (isOver)
		{
			// ぶつかっていたものが離れた、判定開始
			isNotOverChange = true;
		}

		if (other.tag != Tags.RideOn)
		{
			return;
		}

		var graspItem = other.transform.GetComponentInParent<GraspItem>();
		if (graspItem != null)
		{
			if (graspItem.isGrasp)
			{
			}
		}
		else if (other.GetComponentInParent<Player>() != null)
		{
			player.isRide = false;
		}
		else if (other.transform.parent == scaffoldingBody)
		{
			other.transform.parent = null;
		}
	}

	private void OnCollisionEnter(Collision other)
	{
		controller = GetComponentInParent<MovableObjectController>();
		if (other.transform.tag == Tags.RideOn || controller == null)
		{
			return;
		}

		var rigidBody = other.transform.GetComponent<Rigidbody>();

		if (isOver)
		{
			// カラーブロックが消失状態で重なっている間に再生された
			// つまり埋まった
			isStop = true;
			if (rigidBody == null || (other.transform.GetComponent<GraspItem>() == null))
			{
				// 埋まった時の処理
				controller.Burying();
				collisionColorObject = other.transform.GetComponent<ColorObjectBase>();
			}
		}
		else
		{
			// 何かにぶつかったので停止
			if (rigidBody == null || (other.transform.GetComponent<GraspItem>() == null))
			{
				if (other.transform == transform)
				{
					return;
				}
				controller.Collision();
				collisionColorObject = other.transform.GetComponent<ColorObjectBase>();
			}
		}
	}

	private void OnCollisionExit(Collision other)
	{
		controller = GetComponentInParent<MovableObjectController>();
		if (other.transform.tag == Tags.RideOn || controller == null)
		{
			return;
		}

		// ぶつかっていたものが離れたので停止解除
		if (isStop)
		{
			isStop = false;
		}
		// controller.CollisionDisappearance();
	}

	private void FixedUpdate()
	{
		if (!isNotOverChange)
			return;

		if (overCapacityTimeCount > overCapacityTime)
		{
			isOver = false;
			isNotOverChange = false;
			overCapacityTimeCount = 0.0f;
		}
		else
		{
			overCapacityTimeCount += Time.deltaTime;
		}
	}
}