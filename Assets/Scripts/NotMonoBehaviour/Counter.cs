using UnityEngine;
using System.Collections;

/// <summary>
/// カウントを実行させる
/// </summary>
public class Counter
{
	private readonly float targetTime_s; // 目標時間
	private float count; // 目標時間までをカウント

	public Counter(float targetTime_s)
	{
		this.targetTime_s = targetTime_s;
	}

	public bool isCounting
	{
		get { return Mathf.Abs(count) >= float.Epsilon; }
	}

	/// <summary>
	/// StartCoroutineで呼び出すことでカウント開始
	/// </summary>
	/// <param name="targetTime_s">カウント時間</param>
	/// <returns></returns>
	public IEnumerator StartCounter(float targetTime_s)
	{
		count = 0.0f;
		while(count < targetTime_s)
		{
			count += Time.deltaTime;
			yield return null;
		}

		count = 0.0f;
	}

	/// <summary>
	/// StartCoroutineで呼び出すことでカウント開始
	/// </summary>
	/// <returns></returns>
	public IEnumerator StartCounter()
	{
		count = 0.0f;
		while (count < targetTime_s)
		{
			count += Time.deltaTime;
			yield return null;
		}

		count = 0.0f;
	}
}
