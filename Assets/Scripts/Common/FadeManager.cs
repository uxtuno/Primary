using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// フェードイン、フェードアウトを管理する
/// </summary>
public class FadeManager : MonoBehaviour
{
	// 唯一のインスタンス
	private static FadeManager _instance;

	/// <summary>
	///     フェード中に実行させる処理
	/// </summary>
	public delegate void Processing();

	// フェードをかけるために使用するCanvas。子にImageが必要
	private static GameObject fadeCanvasPrefab; 
	private static Canvas fadeCanvas;
	private static Image fadeImage;

	private static readonly int defaultSortOrder = 5;

	private Coroutine fadeCoroutine; // フェードアウト、フェードインを実行するコルーチン

	public FadeManager()
	{
		IsFading = false;
	}

	public static FadeManager instance
	{
		get
		{
			if (_instance == null)
			{
				var SceneChangeSingleton = new GameObject("FadeManager");
				_instance = SceneChangeSingleton.AddComponent<FadeManager>();
				fadeCanvasPrefab = Resources.Load<GameObject>("Prefabs/UI/FadeCanvas");
				DontDestroyOnLoad(SceneChangeSingleton.gameObject);
			}

			return _instance;
		}
	}

	private List<string> scenePaths { get; set; } // 読み込み可能なシーンのパス

	public bool IsFading { get; private set; }

	/// <summary>
	///     フェードイン、フェードアウトでシーンを切り替える
	/// </summary>
	/// <param name='interval'>暗転にかかる時間(秒)</param>
	/// <param name="processing">フェードアウト完了時に実行する処理</param>
	public void Fade(float interval = 0.5f, Processing processing = null)
	{
		if (IsFading)
		{
			Debug.Log("フェード中です。");
			return;
		}

		fadeCoroutine = StartCoroutine(TransScene(interval, defaultSortOrder, Color.white, processing));
	}

	/// <summary>
	///     フェードイン、フェードアウトでシーンを切り替える
	/// </summary>
	/// <param name='interval'>暗転にかかる時間(秒)</param>
	/// <param name="sortOrder">フェードを掛ける面</param>
	/// <param name="processing">フェードアウト完了時に実行する処理</param>
	public void Fade(float interval = 0.5f, int sortOrder = 5, Processing processing = null)
	{
		if (IsFading)
		{
			Debug.Log("フェード中です。");
			return;
		}

		fadeCoroutine = StartCoroutine(TransScene(interval, sortOrder, Color.white, processing));
	}

	/// <summary>
	///     フェードイン、フェードアウトでシーンを切り替える
	/// </summary>
	/// <param name="startColor">色</param>
	/// <param name='interval'>暗転にかかる時間(秒)</param>
	/// <param name="processing">フェードアウト完了時に実行する処理</param>
	public void Fade(Color startColor, float interval = 0.5f, Processing processing = null)
	{
		Fade(startColor, interval, defaultSortOrder, processing);
	}

	/// <summary>
	///     フェードイン、フェードアウトでシーンを切り替える
	/// </summary>
	/// <param name="startColor">色</param>
	/// <param name='interval'>暗転にかかる時間(秒)</param>
	/// <param name="sortOrder">フェードを掛ける面</param>
	/// <param name="processing">フェードアウト完了時に実行する処理</param>
	public void Fade(Color startColor, float interval = 0.5f, int sortOrder = 5, Processing processing = null)
	{
		if (IsFading)
		{
			Debug.Log("フェード中です。");
			return;
		}

		fadeCoroutine = StartCoroutine(TransScene(interval, sortOrder, startColor, processing));
	}


	/// <summary>
	/// 実際にフェードアウトを実行
	/// </summary>
	/// <param name="interval">暗転にかかる時間(秒)</param>
	/// <param name="sortOrder">フェードを掛ける面</param>
	/// <param name="startColor">色</param>
	/// <param name="processing">フェードアウト完了時に実行する処理</param>
	/// <returns></returns>
	private IEnumerator TransScene(float interval, int sortOrder, Color startColor, Processing processing)
	{
		fadeCanvas = Instantiate(fadeCanvasPrefab).GetComponent<Canvas>();
		// シーン遷移が行われても問題なく動作するように、破棄しないようにする
		DontDestroyOnLoad(fadeCanvas.gameObject);
		fadeImage = fadeCanvas.GetComponentInChildren<Image>();
		fadeImage.color = startColor;

		//だんだん暗く
		IsFading = true;
		float time = 0;
		while (time <= interval)
		{
			//this.FadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
			var c = fadeImage.color;
			c.a = Mathf.Lerp(0f, 1f, time / interval);
			fadeImage.color = c;
			time += Time.deltaTime;
			yield return 0;
		}

		// フェード中に実行する処理
		if (processing != null)
		{
			processing();
		}

		//だんだん明るく
		time = 0;
		while (time <= interval)
		{
			//this.FadeAlpha = Mathf.Lerp(1.0f, 0.0f, time / interval);
			var c = fadeImage.color;
			c.a = Mathf.Lerp(1.0f, 0.0f, time / interval);
			fadeImage.color = c;

			time += Time.deltaTime;
			yield return 0;
		}

		Destroy(fadeCanvas.gameObject);
		IsFading = false;
	}

	/// <summary>
	/// フェードを強制停止
	/// </summary>
	public void FadeStop()
	{
		if (fadeCanvas != null)
		{
			Destroy(fadeCanvas.gameObject);
		}

		if (fadeCoroutine != null)
			StopCoroutine(fadeCoroutine);

		IsFading = false;
	}
}