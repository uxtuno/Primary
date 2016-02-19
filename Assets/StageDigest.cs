using UnityEngine;
using System.Collections;
using System.Linq;

public class StageDigest : MonoBehaviour
{
	[SerializeField]
	private GameObject[] stagePrefabs;

	// Use this for initialization
	IEnumerator Start()
	{
		GameObject go = null;
		// 名前で探しちゃう
		var backGround = GameObject.Find("BackGround");
		if (backGround)
		{
			Destroy(go);
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
				FadeManager.instance.Fade("", 0.5f,
					() =>
					{
						if (go)
							Destroy(go);
						go = Instantiate(stagePrefabs[stageIndex]);
					});
				yield return new WaitForSeconds(8.0f);
			}
		}
	}
}
