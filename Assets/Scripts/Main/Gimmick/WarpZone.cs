using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// ワープゾーン。接触したものを対応するワープ先へ移動させる
/// </summary>
public class WarpZone : Gimmick, ISwitchEvent
{
	private bool isWarp; // ワープした瞬間
	private bool startOnAwake = true; // 開始時から起動しているか
	[SerializeField]
	private GameObject warpEffectPrefab = null; // 

	//移動先のオブジェクト
	[SerializeField]
	private GameObject destinationObject = null;
	private WarpZone destinationWarpZone;

	private bool isPossible = true; // ワープが使用可能か

	/// <summary>
	/// ワープが使用可能か
	/// </summary>
	public bool switchState
	{
		get
		{
			return isPossible;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		isPossible = startOnAwake;
		destinationWarpZone = destinationObject.GetComponent<WarpZone>();
	}

	void OnTriggerEnter(Collider other)
	{
		if (!isPossible)
		{
			return;
		}

		GraspItem grasp = other.GetComponentInParent<GraspItem>();
		if (grasp != null && grasp.isGrasp)
		{
			return;
		}

		if (other.tag == Tags.RideOn && !isWarp)
		{
			if (other.transform.parent.tag != Tags.Player)
			{
				Instantiate(warpEffectPrefab, other.transform.parent.position, Quaternion.identity);
				Instantiate(warpEffectPrefab, destinationObject.transform.position, Quaternion.identity);

				other.transform.parent.position = destinationObject.transform.position;
				other.transform.parent.rotation = destinationObject.transform.rotation;
				isWarp = true;

				if (destinationWarpZone != null)
				{
					destinationObject.GetComponent<WarpZone>().isWarp = true;
				}
			}
			else
			{
				// プレイヤーが入ったとき。フェードアウトの演出が入る
				FadeManager.instance.Fade("", 0.5f,
					delegate ()
					{
						Instantiate(warpEffectPrefab, other.transform.parent.position, Quaternion.identity);
						Instantiate(warpEffectPrefab, destinationObject.transform.position, Quaternion.identity);

						player.StopLaser();
						other.transform.parent.position = destinationObject.transform.position;
						other.transform.parent.rotation = destinationObject.transform.rotation;
						isWarp = true;
						if (destinationWarpZone != null)
						{
							destinationObject.GetComponent<WarpZone>().isWarp = true;
						}
					});
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (!isPossible)
		{
			return;
		}

		if (other.tag == Tags.RideOn && isWarp)
		{
			isWarp = false;
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.tag == Tags.RideOn)
		{
			// 重なったものがプレイヤー以外なら押し出す
			if (other.GetComponentInParent<Player>() == null)
			{
				Vector3 offset = (other.transform.position - transform.position).normalized;
				offset.y = 0.0f;
				other.transform.parent.position += offset * Time.deltaTime;
			}
		}
	}

	public void Switch()
	{
		isPossible = !isPossible;
	}
}
