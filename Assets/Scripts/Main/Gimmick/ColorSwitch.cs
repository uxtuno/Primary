using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     光線に反応するスイッチ
/// </summary>
public class ColorSwitch : ColorObjectBase
{
	private List<ISwitchEvent> switchTargets = new List<ISwitchEvent>(); // スイッチの操作対象として登録可能なもの

	[SerializeField]
	private EventObject[] targets = null; // 操作対象

	public ColorSwitch()
	{
		switchState = false;
	}

	/// <summary>
	///     スイッチの状態
	/// </summary>
	public bool switchState { get; set; }

	private void Start()
	{
		// 操作対象として登録可能なものを絞り込む
		foreach (var target in targets)
		{
			if (target is ISwitchEvent)
			{
				switchTargets.Add(target as ISwitchEvent);
			}
			else
			{
				Debug.Log("スイッチの操作対象として登録出来ません");
			}
		}
	}

	protected override void LateUpdate()
	{
		// 自分の色と同じ色の光線が当たっていればスイッチオン
		if (objectColor.state == IrradiationColor.state)
		{
			// 状態がオフならオンに切り替え
			if (!switchState)
			{
				SwitchOn();
			}
		}
		else
		{
			// 状態がオンならオフに切り替え
			if (switchState)
			{
				SwitchOff();
			}
		}

		IrradiationColor = ColorState.NONE;
	}

	/// <summary>
	///     スイッチオン
	/// </summary>
	private void SwitchOn()
	{
		switchState = true;
		foreach (var target in switchTargets)
		{
			target.Switch();
		}
	}

	/// <summary>
	///     スイッチオフ
	/// </summary>
	private void SwitchOff()
	{
		switchState = false;
		foreach (var target in switchTargets)
		{
			target.Switch();
		}
	}
}