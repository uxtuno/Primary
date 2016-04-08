using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// プレイヤー前方を調べる動作を担当
/// イベント発生や、箱を掴むなど
/// </summary>
[RequireComponent(typeof(SphereCollider))]
public class Examine : MyMonoBehaviour
{
	private GraspItem _grabbedObject = null;
	private GraspItem _grabPossibleObject = null;
	private List<GraspItem> hitList = new List<GraspItem>();
	private float grabbedMoveSpeed = 10.0f; // 手に持ったオブジェクトが手に追従する速度
	private float grabbedLimitLeaving = 1.5f; // 手に持ったオブジェクトが離れられる限界
	private float radius;

	/// <summary>
	/// 掴むことが出来る物
	/// 何も掴めそうなものが無い場合null
	/// </summary>
	public GraspItem grabPossibleObject
	{
		get
		{
			return _grabPossibleObject;
		}
	}

	/// <summary>
	/// 現在掴んでいるもの
	/// </summary>
	public GraspItem grabbedObject
	{
		get
		{
			return _grabbedObject;
		}

		protected set
		{
			_grabbedObject = value;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		radius = GetComponent<SphereCollider>().radius;
	}

	void Start()
	{
		GetComponent<SphereCollider>().isTrigger = true;
	}

	protected override void Update()
	{
		base.Update();
		if (AdvancedWriteMessageSingleton.instance.isWrite)
		{
			return;
		}

		Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
		float minDistance = Mathf.Infinity;
		Component[] iconOwnerCheckComponents = null;
		bool isGrabbed = false;
		foreach (Collider hitCollider in colliders)
		{
			float distance = (hitCollider.transform.position - transform.position).sqrMagnitude;
			Component[] checkComponents = hitCollider.gameObject.GetComponents(typeof(ICheckEvent));
			if (distance < minDistance && checkComponents.Length != 0)
			{
				if (hitCollider.GetComponent<GraspItem>() != null && !hitCollider.GetComponent<GraspItem>().enabled)
				{
					continue;
				}

				if (isGrabbed)
				{
					if (hitCollider.GetComponent<GraspItem>() != null)
					{
						minDistance = distance;
						iconOwnerCheckComponents = checkComponents;
					}
				}
				else
				{
					if (hitCollider.GetComponent<GraspItem>() != null)
					{
						isGrabbed = true;
					}

					minDistance = distance;
					iconOwnerCheckComponents = checkComponents;
				}
			}
		}

		if (iconOwnerCheckComponents != null && iconOwnerCheckComponents.Length != 0)
		{
			if (grabbedObject == null)
			{
				foreach (ICheckEvent checkEvent in iconOwnerCheckComponents)
				{
					checkEvent.GetIconSprite();
				}

				if ((Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(1)) && !player.isIrradiation)
				{
					foreach (ICheckEvent component in iconOwnerCheckComponents)
					{
						component.Check();
					}
				}
			}
			else
			{
				ExamineIconManager.HideIcon();
			}
		}
		else
		{
			ExamineIconManager.HideIcon();
		}

		if (hitList.Count != 0)
		{
			if (Input.GetMouseButtonDown(1) && !Input.GetMouseButton(0))
			{
				minDistance = Mathf.Infinity;
				GraspItem grabItem = null;
				foreach (GraspItem item in hitList)
				{
					float distance = (item.transform.position - transform.position).magnitude;
					if (minDistance > distance && !item.collider.isTrigger)
					{
						grabItem = item.GetComponent<GraspItem>();
						minDistance = distance;
					}
				}

				if (grabItem != null)
				{
					grabItem.Grab(transform.parent.gameObject);
					grabbedObject = grabItem;
					grabbedObject.transform.parent = transform.parent.parent;
				}
			}
		}

		if (grabbedObject != null)
		{
			bool isLetGo = false; // 持ってるものを離す
			if ((transform.position - grabbedObject.transform.position).sqrMagnitude > grabbedLimitLeaving)
			{
				isLetGo = true;
			}
			else
			{
				grabbedObject.rigidbody.velocity = (transform.position - grabbedObject.transform.position) * grabbedMoveSpeed;
			}

			if (Input.GetMouseButtonUp(1))
			{
				isLetGo = true;
			}
			else if (grabbedObject.GetComponent<ColorObjectBase>() != null && grabbedObject.GetComponent<ColorObjectBase>().isDisappearance)
			{
				isLetGo = true;
			}

			if (isLetGo)
			{
				LetGo();
			}
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (grabbedObject != null)
		{
			return;
		}

		// hitListに何か含まれていればreturn。(拾えるもの優先)
		foreach (GraspItem listItem in hitList)
		{
			ColorObjectBase colorObject = listItem.GetComponent<ColorObjectBase>();
			if (colorObject != null)
			{
				if (!colorObject.isDisappearance)
				{
					return;
				}
			}
			else
			{
				return;
			}
		}

		// 目の前の物を調べる
		// 調べられるもの全てを配列として取得
		Component[] checkComponents = other.GetComponents(typeof(ICheckEvent));
		if (checkComponents == null)
		{
			return;
		}

		ExamineIconManager.HideIcon();
		if ((Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(1)) && !player.isIrradiation)
		{
			foreach (ICheckEvent component in checkComponents)
			{
				component.Check();
			}
		}
	}


	void OnTriggerEnter(Collider other)
	{
		if (grabbedObject != null)
		{
			return;
		}

		// hitListに何か含まれていればreturn。(拾えるもの優先)
		foreach (GraspItem listItem in hitList)
		{
			ColorObjectBase colorObject = listItem.GetComponent<ColorObjectBase>();
			if (colorObject != null)
			{
				if (!colorObject.isDisappearance)
				{
					return;
				}
			}
			else
			{
				return;
			}
		}

		if (other.GetComponent<EventObject>() != null)
		{
			//other.GetComponent<EventObject>().ViewIcon();
		}

		// 掴めるものか
		if (other.GetComponent<GraspItem>() == null)
		{
			return;
		}

		// すでにhitListに含まれていればreturn
		if (hitList.Exists((item) => item == other.GetComponent<GraspItem>()))
		{
			return;
		}

		hitList.Add(other.GetComponent<GraspItem>());
	}

	void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<EventObject>() != null)
		{
			//other.GetComponent<EventObject>().HideIcon();
		}

		// hitListに含まれていなければreturn
		if (other.GetComponent<GraspItem>() == null)
		{
			return;
		}

		hitList.Remove(other.GetComponent<GraspItem>());
	}

	/// <summary>
	/// 掴んでいるものを放す
	/// </summary>
	public void LetGo()
	{
		if (grabbedObject == null)
		{
			return;
		}

		grabbedObject.LetGo();
		if (player.rideScaffolds == null)
		{
			grabbedObject.InitParent();
		}
		else
		{
			grabbedObject.transform.parent = player.rideScaffolds.scaffoldingBody;
		}

		grabbedObject = null;
	}
}
