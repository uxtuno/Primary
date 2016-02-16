using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 何らかの動作をさせる
/// </summary>
public interface IActionEvent
{
    void Action();
}

/// <summary>
/// 調べられる
/// </summary>
public interface ICheckEvent
{
    void Check();
	bool isPossible
	{
		get;
	}

	void ShowIcon();
}

/// <summary>
/// 切り替え可能
/// </summary>
public interface ISwitchEvent
{
    void Switch();
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

	[SerializeField]
	private bool _flag = false;
	public bool flag
	{
		get { return _flag; }
		protected set { _flag = value; }
	}
}
