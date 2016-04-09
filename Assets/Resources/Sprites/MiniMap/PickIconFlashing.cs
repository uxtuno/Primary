using System.Collections;
using UnityEngine;

/// <summary>
///     オブジェクトを一定時間点滅後、破棄する
/// </summary>
public class PickIconFlashing : MyMonoBehaviour
{
	[SerializeField, Tooltip("点滅間隔")]
	private float flashingIntervalSeconds = 0.5f;
	[SerializeField, Tooltip("生存時間")]
	private float lifeSeconds = 0.0f;

	private static readonly float showTimeRate = 0.64f;
	private static readonly float hideTimeRate = 0.36f;

	private IEnumerator Start()
	{
		var count = 0.0f;
		IsShow = true;
		while (count < lifeSeconds)
		{
			// 表示中
			yield return new WaitForSeconds(flashingIntervalSeconds * showTimeRate);
			IsShow = !IsShow;
			// 非表示中
			yield return new WaitForSeconds(flashingIntervalSeconds * hideTimeRate);
			IsShow = !IsShow;
			count += flashingIntervalSeconds;
		}
		Destroy(gameObject);
	}

	/// <summary>
	/// インスペクタ範囲チェック
	/// </summary>
	private void OnValidate()
	{
		if (lifeSeconds < 0.0f)
			lifeSeconds = 0.0f;

		if (flashingIntervalSeconds < 0.0f)
			flashingIntervalSeconds = 0.0f;
	}
}