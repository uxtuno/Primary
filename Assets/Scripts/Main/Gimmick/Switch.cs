using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// スイッチ。一つ以上のものが乗っていた場合にオン
/// </summary>
public class Switch : Gimmick
{
	[SerializeField]
	private EventObject[] targets = null; // 操作対象
	private List<ISwitchEvent> switchTargets = new List<ISwitchEvent>(); // スイッチの操作対象として登録可能なもの

	private List<GameObject> hitList = new List<GameObject>();
	private bool _switchState = false; // switchStateのバッキングフィールド

	/// <summary>
	/// スイッチの状態
	/// </summary>
	public bool switchState
	{
		get { return _switchState; }
		set { _switchState = value; }
	}

	protected override void Awake()
	{
		base.Awake();
		if (useSounds.Count == 0)
		{
			useSounds.Add(SoundCollector.SoundName.Switch);
		}
	}

	void Start()
	{
		int index = 0;
		// 操作対象として登録可能なものを絞り込む
		foreach (EventObject target in targets)
		{
			index++;
			if (target is ISwitchEvent)
			{
				switchTargets.Add(target as ISwitchEvent);
			}
			else
			{
				Debug.Log("(index : " + index.ToString() + ")スイッチの操作対象として登録出来ません");
			}
		}
		// 誤って使用しないように
		targets = null;
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag != Tags.RideOn)
		{
			return;
		}

		if (other.GetComponentInParent<ColorObjectBase>() != null)
		{
			if (other.GetComponentInParent<ColorObjectBase>().isDisappearance)
			{
				// カラーブロックのisTriggerがtrueになったときに入るのでリストから削除
				hitList.RemoveAll((match) => match == other.gameObject);
				if (hitList.Count == 0 && switchState)
				{
					SwitchOff();
					switchState = false;
				}
				return;
			}
		}

		if (!switchState)
		{
			SwitchOn();
			switchState = true;
		}

		hitList.Add(other.gameObject);
	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag != Tags.RideOn)
		{
			return;
		}
		// スイッチから外れたオブジェクトをリストから消去
		hitList.RemoveAll((match) => match == other.gameObject);
		if (switchState && hitList.Count == 0)
		{
			SwitchOff();
			switchState = false;
		}
	}

	/// <summary>
	/// ターゲットを全てオン
	/// </summary>
	private void SwitchOn()
	{
		switchState = true;
		foreach (ISwitchEvent target in switchTargets)
		{
			target.Switch();
		}

		// タイトル画面では音がならないように
		if (Application.loadedLevelName == SceneName.Menu)
			return;

		SoundPlayerSingleton.instance.PlaySE(gameObject, soundCollector[useSounds[0]], false, true, 0.5f, 0.0f, true);
	}

	/// <summary>
	/// ターゲットを全てオフ
	/// </summary>
	private void SwitchOff()
	{
		switchState = false;
		foreach (ISwitchEvent target in switchTargets)
		{
			target.Switch();
		}
	}


}
