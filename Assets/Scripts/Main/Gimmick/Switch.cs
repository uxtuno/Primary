using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

/// <summary>
/// スイッチ。一つ以上のものが乗っていた場合にオン
/// </summary>
public class Switch : Gimmick
{
	[SerializeField] private EventObject[] targets = null; // 操作対象
	private List<ISwitchEvent> switchTargets = new List<ISwitchEvent>(); // スイッチの操作対象として登録可能なもの

	private List<GameObject> hitList = new List<GameObject>();

	/// <summary>
	/// スイッチの状態
	/// </summary>
	public bool switchState { get; set; }

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
				Debug.Log("(index : " + index + ")スイッチの操作対象として登録出来ません");
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

		if (!switchState)
		{
			SwitchOn();
		}

		// 同じものが含まれていた場合登録しない
		if (hitList.Contains(other.gameObject))
		{
			return;
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
		hitList.Remove(other.gameObject);
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

	/// <summary>
	/// 指定GameObjectをスイッチの上に載っているものリストから外す
	/// この操作でスイッチの上の物が無くなるとこのスイッチはOffになる
	/// </summary>
	/// <param name="removeGameObject">リストから外す対象</param>
	public void RemoveHitObject(GameObject removeGameObject)
	{
		if (removeGameObject == null)
			return;

		hitList.RemoveAll(match => match == removeGameObject.gameObject);

		if (hitList.Count == 0 && switchState)
		{
			SwitchOff();
		}
	}
}