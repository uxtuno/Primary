using UnityEngine;
using System.Collections;

/// <summary>
/// カウントを実行させる
/// </summary>
public class Counter
{
	private readonly float targetTime_s; // 目標時間
	private int count; // 目標時間までをカウント

	public Counter(float targetTime_s)
	{
		this.targetTime_s = targetTime_s;
	}

	public bool isCounting
	{
		get { return count > 0; }
	}

	/// <summary>
	/// StartCoroutineで呼び出すことでカウント開始
	/// </summary>
	/// <returns></returns>
	public IEnumerator StartCounter()
	{
		count = 0;

		AddCount();
		while (count * 0.001f < targetTime_s)
		{
			yield return new WaitForFixedUpdate();
			AddCount();
		}
		count = 0;
	}

	private void AddCount()
	{
		// 浮動小数点の誤差軽減のためintに変換して計算
		count += (int)(Time.deltaTime * 1000 + 0.5f);
	}
}
