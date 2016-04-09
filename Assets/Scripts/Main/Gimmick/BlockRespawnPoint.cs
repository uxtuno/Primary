using UnityEngine;

// Todo : 現状、ゲーム内で使用するかはわからないので動作確認が不完全。

/// <summary>
/// ブロックの出現を管理するスクリプト
/// これをアタッチしなくてもリスポーンはする
/// 任意のタイミングで出現させたい時に使用
/// </summary>
public class BlockRespawnPoint : Gimmick, IActionEvent
{
	[SerializeField]
	private ColorObjectBase colorObject = null; // 対応するカラーオブジェクト
	[SerializeField]
	private bool playOnAwake = true; // 開始と同時に出現させる

	protected override void Awake()
	{
		base.Awake();

		if (playOnAwake)
		{
			Pop();
		}
		else
		{
			colorObject.IsShow = false;
		}
	}

	/// <summary>
	/// ブロックを出現させる
	/// </summary>
	private void Pop()
	{
		colorObject.IsShow = true;
		colorObject.transform.position = transform.position;
	}

    public void Action()
    {
        Pop();
    }
}
