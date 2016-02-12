using UnityEngine;

public class Respawn : MonoBehaviour
{
	[SerializeField]
	private GameObject respawnPoint; // リスポーン位置

	void Awake()
	{
		if (respawnPoint == null)
		{
			//Debug.Log("respawnPointが" + respawnPoint + "です。");
			respawnPoint = Instantiate(new GameObject("obj"));
			respawnPoint.transform.position = Vector3.zero;
		}
	}

	/// <summary>
	/// ここで何かしらのペナルティを課す
	/// </summary>
	void Penalty()
	{
	}

	void OnTriggerExit(Collider other)
	{
		// プレイヤーをリスポーン
		Player player = other.GetComponentInParent<Player>();
		if (player != null)
		{
			Penalty();
			player.Respawn();
			player.transform.position = respawnPoint.transform.position;
		}

		// リスポーンオブジェクトをリスポーン
		if (other.GetComponentInParent<RespawnObject>() != null)
		{
			other.GetComponentInParent<RespawnObject>().Respawn();
		}
	}
}