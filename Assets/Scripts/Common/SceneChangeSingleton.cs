using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SceneChangeSingleton : MonoBehaviour
{

	// 唯一のインスタンス
	private static SceneChangeSingleton _instance;
	public static SceneChangeSingleton instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject SceneChangeSingleton = new GameObject("SceneChangeSingleton");
				_instance = SceneChangeSingleton.AddComponent<SceneChangeSingleton>();
				DontDestroyOnLoad(SceneChangeSingleton.gameObject);
			}

			return _instance;
		}
	}

	List<string> scenePaths { get; set; }   // 読み込み可能なシーンのパス
	private readonly int Layer = 0; // 全てのオブジェクトを覆い隠せるようにレイヤーを最前面に
	private Texture2D BlackTexture;
	private float FadeAlpha = 0.0f; // フェード中の透明度
	private bool isFading = false; // フェード中かどうか
	public bool IsFading
	{
		get
		{
			return isFading;
		}
	}

	/// <summary>
	/// フェードイン、フェードアウトでシーンを切り替える
	/// </summary>
	/// <param name='scene'>シーン名</param>
	/// <param name='interval'>暗転にかかる時間(秒)</param>
	public void LoadLevel(string scene, float interval = 0.5f)
	{
		if (IsFading)
		{
			Debug.Log("フェード中にシーンを切り替えようとしました");
			return;
		}

		// 別のフェードが動いていたら止める(競合するので)
		if (FadeManager.instance.IsFading)
			FadeManager.instance.FadeStop();
		StartCoroutine(TransScene(scene, interval));
	}

	/// <summary>
	/// シーンを追加して読み込む
	/// </summary>
	/// <param name="scenes">シーン名の配列</param>
	/// <param name='interval'>暗転にかかる時間(秒)</param>
	public void LoadLevel(string[] scenes, float interval = 0.5f)
	{
		if (IsFading)
		{
			Debug.Log("フェード中にシーンを切り替えようとしました");
			return;
		}

		StartCoroutine(TransScene(scenes, interval));
	}

	public void Awake()
	{
		scenePaths = new List<string>();

#if UNITY_EDITOR
		// BuildSettingsで登録されているシーンのパスを格納する
		foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
		{
			scenePaths.Add(scene.path);
		}
#endif

		//ここで黒テクスチャ作る
		BlackTexture = new Texture2D(32, 32, TextureFormat.RGB24, false);
		BlackTexture.SetPixel(0, 0, Color.white);
		BlackTexture.Apply();
	}

	public void OnGUI()
	{
		if (!this.IsFading)
			return;

		GUI.depth = Layer;

		//透明度を更新して黒テクスチャを描画
		GUI.color = new Color(0.0f, 0.0f, 0.0f, this.FadeAlpha);
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), this.BlackTexture);
	}


	// シーン遷移用コルーチン
	IEnumerator TransScene(string scene, float interval)
	{
		//だんだん暗く
		this.isFading = true;
		float time = 0;
		while (time <= interval)
		{
			this.FadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
			time += Time.deltaTime;
			yield return 0;
		}

		//if (SceneSearch(scene))
		{
			//シーン切替
			Application.LoadLevel(scene);
		}
		//else
		//{
		//    Debug.LogError(scene + "シーンに遷移出来ませんでしました。\nシーンが存在するか、またBuildSettingsに登録されているか確認してください。");
		//}

		//だんだん明るく
		time = 0;
		while (time <= interval)
		{
			this.FadeAlpha = Mathf.Lerp(1.0f, 0.0f, time / interval);
			time += Time.deltaTime;
			yield return 0;
		}

		this.isFading = false;
	}

	// シーンマージ遷移用コルーチン
	IEnumerator TransScene(string[] scenes, float interval)
	{
		//だんだん暗く
		this.isFading = true;
		float time = 0;
		while (time <= interval)
		{
			this.FadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
			time += Time.deltaTime;
			yield return 0;
		}

		//scenesにindexを追加
		var scenesWithIndex = scenes.Select((name, index) => new { index, name });

		//シーン切替
		foreach (var scene in scenesWithIndex)
		{
			if (scene.index == 0)
			{
				Application.LoadLevel(scene.name);
			}
			else if (scene.index > 0)
			{
				Application.LoadLevelAdditive(scene.name);
			}
			else
			{
				Debug.LogError("scenesのデータが異常です" + scene.name + scene.index);
			}
		}

		//だんだん明るく
		time = 0;
		while (time <= interval)
		{
			this.FadeAlpha = Mathf.Lerp(1.0f, 0.0f, time / interval);
			time += Time.deltaTime;
			yield return 0;
		}

		this.isFading = false;
	}

	/// <summary>
	/// シーンが存在するかどうかを調べる
	/// </summary>
	/// <param name="scene">検索対象のシーン名</param>
	bool SceneSearch(string scene)
	{
		bool ret = false;
		foreach (string scenePath in scenePaths)
		{
			if (scene != "" && scenePath.IndexOf(scene + ".unity") >= 0)
			{
				ret = true;
				break;
			}
		}

		return ret;
	}
}
