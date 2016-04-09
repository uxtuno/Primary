using UnityEngine;
using System.Collections;
using System.Linq;

public class StageDigest : MonoBehaviour
{
	[SerializeField]
	private GameObject[] stagePrefabs;
	GameObject stage = null;

	// Use this for initialization
	IEnumerator Start()
	{
		// 名前で探しちゃう
		var backGround = GameObject.Find("BackGround");
		if (backGround)
		{
			Destroy(stage);
		}

		while (true)
		{
			int[] stageOrder = Enumerable.Repeat(-1, stagePrefabs.Length).ToArray();
			// あらかじめステージの順番を決めておく
			for (int i = 0; i < stagePrefabs.Length; ++i)
			{
				// 一巡するまで重複を無くすために
				// 重複があれば次のステージを選択
				int r = Random.Range(0, stagePrefabs.Length);
				while (stageOrder.Contains(r))
					r = (r + 1) % stagePrefabs.Length;
				stageOrder[i] = r;
			}

			for (int i = 0; i < stagePrefabs.Length; ++i)
			{
				int stageIndex = stageOrder[i];
				FadeManager.instance.Fade(new Color(0.0f, 0.0f, 0.0f), 1.5f,
					() =>
					{
						if (stage)
							Destroy(stage);
						stage = Instantiate(stagePrefabs[stageIndex]);
					});
				yield return new WaitForSeconds(8.0f);
			}
		}
	}

	/// <summary>
	/// シーンを切り替えるときには呼ばなければならない
	/// </summary>
	public void Stop()
	{
		FadeManager.instance.FadeStop();
	}
}
