using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// メッセージイベント
/// なんらかの条件によってメッセージ字幕を表示します
/// </summary>
public class MessageEvent : EventObject, ICheckEvent
{
	/// <summary>
	/// イベント発生条件
	/// </summary>
	public enum Conditions
	{
		Collision, // プレイヤーが
		Checked, // プレイヤーが調べたとき
	}

	[SerializeField]
	private int messageIndex = -1; // 調べたときに表示するメッセージのインデックス
	[SerializeField]
	private bool oneTime = true; // 一度だけ表示
	[SerializeField]
	private Conditions conditions = Conditions.Checked; // イベント発生条件 
	[SerializeField]
	private Item sameTimeItem = null; // 同時に取得するアイテム

	private bool isCall; // メッセージ呼び出し中

	private bool _isPossible = true; // isPossibleの実体

	/// <summary>
	/// 調べられるフラグ
	/// </summary>
	public bool isPossible
	{
		get { return _isPossible; }
		private set { _isPossible = value; }
	}

	protected override void Awake()
	{
		base.Awake();

		isCall = false;
	}

	protected override void Update()
	{
		if (isCall)
		{
			if (!AdvancedWriteMessageSingleton.instance.isWrite)
			{
				isCall = false;
				if (oneTime)
					isPossible = false;
				if (sameTimeItem != null)
				{
					sameTimeItem.Check();
				}
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == Tags.Player && conditions == Conditions.Collision)
		{
			CallMessage();
		}
	}

	/// <summary>
	/// メッセージを表示
	/// </summary>
	public void Check()
	{
		// 発生条件が接触の場合、調べられないように
		if (conditions == Conditions.Collision)
		{
			return;
		}

		CallMessage();
	}

	/// <summary>
	/// 調べる時のアイコン
	/// </summary>
	public void GetIconSprite()
	{
		if (isPossible)
		{
			ExamineIconManager.ShowIcon(ExamineIconManager.IconType.Check);
		}
	}

	/// <summary>
	/// メッセージ表示開始
	/// </summary>
	private void CallMessage()
	{
		if (isCall)
		{
			return;
		}

		if (isPossible)
		{
			if (messageIndex != -1)
			{
				isCall = true;

				AdvancedWriteMessageSingleton.instance.Write(messageIndex);
				ExamineIconManager.HideIcon();
			}
		}
	}
}
