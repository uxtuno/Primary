using UnityEngine;
using System.Collections;

/// <summary>
/// 移動するオブジェクト
/// </summary>
public class MovableObject : MyMonoBehaviour {
	private MovableObjectController controller; // オブジェクトを制御する、親
	private ColorObjectBase colorObstacle;

	protected override void Awake()
	{
		base.Awake();
		controller = transform.parent.GetComponent<MovableObjectController>();
		if(controller == null)
		{
			Debug.LogError("親オブジェクトにMovableObjectControllerがアタッチされていません");
		}
	}

	protected override void Update()
	{
		if(colorObstacle != null && colorObstacle.isDisappearance)
		{
			// もし衝突していた物体が消えたなら、コントローラ側にそれを知らせる
			controller.CollisionDisappearance();
			colorObstacle = null;
		}
	}

	void OnCollisionEnter(Collision other)
	{
		controller.Collision();

		if(other.transform.GetComponent<Rigidbody>() == null)
		{
			return;
		}

		// 途中で消失する可能性のあるものにぶつかったときはそれを覚えておく
		if(other.transform.GetComponent<Rigidbody>().constraints == RigidbodyConstraints.FreezeAll)
		{
			colorObstacle = other.transform.GetComponent<ColorObjectBase>();
		}
	}

	void OnCollisionExit(Collision other)
	{
		controller.CollisionDisappearance();
	}
}
