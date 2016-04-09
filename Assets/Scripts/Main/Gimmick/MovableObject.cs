using UnityEngine;

/// <summary>
///     移動するオブジェクト
/// </summary>
public class MovableObject : MyMonoBehaviour
{
	private ColorObjectBase colorObstacle;
	private MovableObjectController controller; // オブジェクトを制御する、親

	protected override void Awake()
	{
		base.Awake();
		controller = transform.parent.GetComponent<MovableObjectController>();
		if (controller == null)
		{
			Debug.LogError("親オブジェクトにMovableObjectControllerがアタッチされていません");
		}
	}

	protected override void Update()
	{
		if (colorObstacle == null || !colorObstacle.isDisappearance) return;

		// もし衝突していた物体が消えたなら、コントローラ側にそれを知らせる
		controller.CollisionDisappearance();
		colorObstacle = null;
	}

	private void OnCollisionEnter(Collision other)
	{
		controller.Collision();

		if (other.transform.GetComponent<Rigidbody>() == null)
		{
			return;
		}

		// 途中で消失する可能性のあるものにぶつかったときはそれを覚えておく
		if (other.transform.GetComponent<Rigidbody>().constraints == RigidbodyConstraints.FreezeAll)
		{
			colorObstacle = other.transform.GetComponent<ColorObjectBase>();
		}
	}

	private void OnCollisionExit(Collision other)
	{
		controller.CollisionDisappearance();
	}
}