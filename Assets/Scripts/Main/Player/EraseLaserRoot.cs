using UnityEngine;
using System.Collections;

public class EraseLaserRoot : MyMonoBehaviour {
	private int hitCount = 0;

	void OnTriggerEnter(Collider other)
	{
		if(hitCount == 0)
		{
			player.isLaserPossible = false;
		}
		hitCount++;
	}

	void OnTriggerExit(Collider other)
	{
		if (hitCount > 0)
		{
			;

			if (--hitCount == 0)
			{
				player.isLaserPossible = true;
			}
		}
	}
}
