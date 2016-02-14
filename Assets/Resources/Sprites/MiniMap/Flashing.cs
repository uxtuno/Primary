using UnityEngine;
using System.Collections;

/// <summary>
/// オブジェクトを一定時間点滅後、破棄する
/// </summary>
public class Flashing : MyMonoBehaviour
{
	[SerializeField, Tooltip("生存時間")]
	private float lifeSeconds = 0.0f;
	[SerializeField, Tooltip("点滅間隔")]
	private float flashingIntervalSeconds = 0.5f;

	IEnumerator Start()
	{
		float count = 0.0f;
		while (count < lifeSeconds)
		{
			yield return new WaitForSeconds(flashingIntervalSeconds);
			IsShow = !IsShow;
			count += flashingIntervalSeconds;
		}
		Destroy(gameObject);
	}

	void OnValidate()
	{
		if (lifeSeconds < 0.0f)
			lifeSeconds = 0.0f;

		if (flashingIntervalSeconds < 0.0f)
			flashingIntervalSeconds = 0.0f;
	}
}
