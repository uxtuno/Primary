using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 存在するアイテム
/// </summary>
public enum Items
{
	None,
	Tablet,
	CardKey,
	RedRaygun,
	GreenRaygun,
	BlueRaygun,
}

public class Item : EventObject, ICheckEvent
{
	private bool _isAcquisition = true; // 取得可能フラグ
	[SerializeField]
	private Items _item = Items.None; // アイテムの種類

	/// <summary>
	/// 取得可能フラグ
	/// </summary>
	public bool isAcquisition
	{
		get { return _isAcquisition; }
		set { _isAcquisition = value; }
	}

	/// <summary>
	/// 利用可能フラグ
	/// isAcquisitionと実質同じ。interfaceメンバを実装
	/// </summary>
	public bool isPossible
	{
		get
		{
			return isAcquisition;
		}
	}

	public Items item
	{
		get { return _item; }
		set { _item = value; }
	}

	public void Check()
	{
		if (!_isAcquisition || !IsShow)
		{
			return;
		}
		IsShow = false;
		player.AddItem(item);
	}

	/// <summary>
	/// Eキーで調べるアイコンを表示
	/// </summary>
	public void ShowIcon()
	{
		// 取得不可能、もしくは取得済みならreturn
		if (!_isAcquisition || !IsShow)
		{
			return;
		}

		if (item == Items.CardKey)
		{
			ExamineIconManager.ShowIcon(ExamineIconManager.IconType.TakeCardKey);
		}
		else
		{
			ExamineIconManager.ShowIcon(ExamineIconManager.IconType.Check);
		}
	}
}
