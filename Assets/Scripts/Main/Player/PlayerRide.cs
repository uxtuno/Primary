using UnityEngine;
using System.Collections;

public class PlayerRide : MyMonoBehaviour {

	void OnTriggerEnter(Collider other)
	{
		// プレイヤーと自身とぶつかっても無視
		if(other.GetComponentInParent<Player>() != null)
		{
			return;
		}

		if(other.isTrigger)
		{
			return;
		}

		Scaffolds scaffolds = other.transform.GetComponentInParent<Scaffolds>();
		if(scaffolds == null && player.rideScaffolds != null && !player.isRide)
		{
			player.rideScaffolds = null;
			player.InitParent();
		}
	}
}
