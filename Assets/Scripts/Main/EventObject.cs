using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 何らかの動作をさせる
/// </summary>
public interface IActionEvent
{
	/// <summary>
	/// 動作を起こす
	/// </summary>
    void Action();
}

/// <summary>
/// 調べられる
/// </summary>
public interface ICheckEvent
{
	/// <summary>
	/// 調べる
	/// </summary>
    void Check();

	/// <summary>
	/// 現在調べられる状態か
	/// </summary>
	bool isPossible
	{
		get;
	}

	/// <summary>
	/// アイコンを表示する
	/// </summary>
	void GetIconSprite();
}

/// <summary>
/// 切り替え可能
/// </summary>
public interface ISwitchEvent
{
	/// <summary>
	/// スイッチの状態を切り替える
	/// </summary>
    void Switch();

	/// <summary>
	/// スイッチの状態
	/// オンかオフか
	/// </summary>
	bool switchState
    {
        get;
    }
}

/// <summary>
/// ステージのイベントに関連するオブジェクト
/// ギミックやメッセージ表示など
/// </summary>
public class EventObject : MyMonoBehaviour
{
	[SerializeField]
	protected List<SoundCollector.SoundName> useSounds;
	protected SoundCollector soundCollector = null;

	protected override void Awake()
	{
		base.Awake();
		soundCollector = FindObjectOfType<SoundCollector>();
	}
}
