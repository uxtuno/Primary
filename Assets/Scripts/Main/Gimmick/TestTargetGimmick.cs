using UnityEngine;
using System.Collections;

public class TestTargetGimmick : Gimmick {

	private bool isStarted = false;

	protected override void Awake()
	{
		base.Awake();
	
	}

	protected override void Update()
	{
		if (!isStarted)
		{
			return;
		}

		Debug.Log("実行中です");
	}

	/// <summary>
	/// ギミックを動作させる
	/// </summary>
	public void StartGimmick()
	{
		isStarted = true;
		Debug.Log("開始されてしまいました！");
	}

	/// <summary>
	/// ギミックを停止させる
	/// </summary>
	public void StopGimmick()
	{
        isStarted = false;
        Debug.Log("停止させられてしまいました！");
    }

	/// <summary>
	/// ギミックが動いているか
	/// </summary>
	/// <returns></returns>
	public bool IsRunning()
	{
		return isStarted;
	}
}
