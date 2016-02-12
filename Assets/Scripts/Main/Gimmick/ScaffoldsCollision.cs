using UnityEngine;
using System.Collections;

/// <summary>
/// 足場
/// 判定のため、足場の表面にIsTriggerのコライダが必要
/// </summary>
public class ScaffoldsCollision : MyMonoBehaviour
{
	private bool isOver = false;
	private bool isNotOverChange = false;
	private bool isStop = false;
	private static readonly float overCapacityTime = 0.1f;
	private float overCapacityTimeCount = 0.0f;
	private ColorObjectBase collisionColorObject = null; // 現在衝突しているカラーオブジェクト
														 /// <summary>
														 /// 足場の本体
														 /// 移動するときはこれを操作する
														 /// </summary>
	private Transform _scaffoldingBody; // 本体

	public Transform scaffoldingBody
	{
		get { return _scaffoldingBody; }
		protected set { _scaffoldingBody = value; }
	}

	private MovableObjectController controller; // 足場を操作するスクリプト

	protected override void Awake()
	{
		base.Awake();
		//rideOnTags = new string[] { Tags.Player };
		if (rigidbody != null)
		{
			rigidbody.constraints = (RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation);
		}

		scaffoldingBody = transform.parent;
	}

	void Start()
	{
		controller = GetComponentInParent<MovableObjectController>();
	}

	protected override void Update()
	{
		if (collisionColorObject != null)
		{
			if (collisionColorObject.isDisappearance)
			{
				controller.CollisionDisappearance();
				collisionColorObject = null;
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<ColorBlock>() != null)
		{
			isOver = true;
			overCapacityTimeCount = 0.0f;
		}

		if (other.tag != Tags.RideOn)
		{
			return;
		}

		GraspItem graspItem = other.transform.GetComponentInParent<GraspItem>();
		if (graspItem != null)
		{
			if (!graspItem.isGrasp)
			{
				other.transform.parent.parent = scaffoldingBody;
			}
		}
		else if (other.GetComponentInParent<Player>() != null)
		{
			player.transform.parent = scaffoldingBody;
			//player.rideScaffolds = this;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (isOver)
		{
			isNotOverChange = true;
		}

		if (other.tag != Tags.RideOn)
		{
			return;
		}

		GraspItem graspItem = other.transform.GetComponentInParent<GraspItem>();
		if (graspItem != null)
		{
			Debug.Log(graspItem.isGrasp);
			if (graspItem.isGrasp)
			{
				return;
			}
		}

		if (other.transform.parent == scaffoldingBody)
		{
			other.transform.parent = null;
		}

	}

	void OnCollisionEnter(Collision other)
	{
		controller = GetComponentInParent<MovableObjectController>();
		if (other.transform.tag == Tags.RideOn || controller == null)
		{
			return;
		}

		Rigidbody rigidBody = other.transform.GetComponent<Rigidbody>();

		if (isOver)
		{
			isStop = true;
			if (rigidBody == null || (other.transform.GetComponent<GraspItem>() == null))
			{
				controller.Burying();
				collisionColorObject = other.transform.GetComponent<ColorObjectBase>();
			}
		}
		else
		{
			if (rigidBody == null || (other.transform.GetComponent<GraspItem>() == null))
			{
				controller.Collision();
				collisionColorObject = other.transform.GetComponent<ColorObjectBase>();
			}
		}
	}

	void OnCollisionExit(Collision other)
	{
		controller = GetComponentInParent<MovableObjectController>();
		if (other.transform.tag == Tags.RideOn || controller == null)
		{
			return;
		}

		if (isStop)
		{
			isStop = false;
		}
		// controller.CollisionDisappearance();
	}

	void FixedUpdate()
	{
		if (isNotOverChange)
		{
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
}
