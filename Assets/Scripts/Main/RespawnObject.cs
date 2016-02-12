using UnityEngine;
using System.Collections;

/// <summary>
/// リスポーンするオブジェクト
/// </summary>
public class RespawnObject : MyMonoBehaviour {
	[SerializeField]
	private BlockRespawnPoint objectRespawnPoint = null; // リスポーンを管理するオブジェクト

	private Vector3 respawnPoint;
	private Quaternion respawnRotation;

	protected override void Awake()
	{
		base.Awake();

		respawnPoint = transform.position;
		respawnRotation = transform.rotation;
	}

	/// <summary>
	/// 再出現させる。
	/// </summary>
	public void Respawn()
	{
		if (objectRespawnPoint != null)
		{
			// これが設定されていれば任せる
			(objectRespawnPoint as ICheckEvent).Check();
		}
		else
		{
			transform.position = respawnPoint;
			transform.rotation = respawnRotation;
			if(transform.GetComponent<Rigidbody>())
			{
				transform.GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
			InitParent();
		}
	}
}
