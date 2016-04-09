using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// ゴール。プレイヤーはこれを調べると次のステージへ進む
/// </summary>
public class Goal : EventObject, ICheckEvent
{
    [SerializeField]
    private string nextSceneName; // 次のステージの名前()
    [SerializeField]
    private Item keyItem = null; // このアイテムをプレイヤーが持っていないと先へ進めない

    public bool isPossible
    {
        get
        {
            return player.ContainsItem(keyItem.item);
        }
    }

    void Start()
    {
        int currentLoadedLevel = -1;
		// ファイルから次のステージを読み込んでくる
        StageData stageData = Resources.Load<StageData>("StageData/StageData");
        currentLoadedLevel = stageData.param.FindIndex(p => p.Name == Application.loadedLevelName);
        nextSceneName = stageData.param[currentLoadedLevel + 1].Name;
    }

    /// <summary>
    /// 扉を開けてシーン遷移
    /// </summary>
    /// <returns></returns>
    IEnumerator MigrationScene()
    {
        // 使用したのでプレイヤーから削除する
        player.DeleteItem(keyItem.item);

        // プレイヤーを直接止める
        player.enabled = false;
        Animator goalAnim = transform.GetComponentInChildren<Animator>();
        goalAnim.SetTrigger("GoalIn");
        while (!goalAnim.GetCurrentAnimatorStateInfo(0).IsName("Take 001"))
        {
            yield return null;
            // 一秒待つ
            // yield return new WaitForSeconds(0.5f);
        }

        gameController.StageChange(nextSceneName);
        yield return null;
    }

	/// <summary>
	/// プレイヤーが調べたとき
	/// </summary>
    public void Check()
    {
        if (isPossible)
        {
            StartCoroutine(MigrationScene());
        }
    }

	/// <summary>
	/// アイコンを表示
	/// </summary>
	public void GetIconSprite()
    {
        if (isPossible)
        {
            ExamineIconManager.ShowIcon(ExamineIconManager.IconType.GotoNextStage);
        }
    }
}
