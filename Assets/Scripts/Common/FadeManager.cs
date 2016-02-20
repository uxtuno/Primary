using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FadeManager : MonoBehaviour
{
	private static GameObject fadeCanvasPrefab;
	private static Canvas fadeCanvas;
	private static Image fadeImage;
	private static readonly int defaultSortOrder = 5;

	/// <summary>
	/// フェード中に実行させる処理
	/// </summary>
	public delegate void Processing();

	// 唯一のインスタンス
	private static FadeManager _instance;
	public static FadeManager instance
	{
		get
		{
			if (_instance == null)
			{
				GameObject SceneChangeSingleton = new GameObject("FadeManager");
				_instance = SceneChangeSingleton.AddComponent<FadeManager>();
				fadeCanvasPrefab = Resources.Load<GameObject>("Prefabs/UI/FadeCanvas");
                DontDestroyOnLoad(SceneChangeSingleton.gameObject);
			}

			return _instance;
		}
	}

	List<string> scenePaths { get; set; }   // 読み込み可能なシーンのパス
	private Texture2D overTexture;
	//private float FadeAlpha = 0.0f; // フェード中の透明度
	private bool isFading = false; // フェード中かどうか
	public bool IsFading
	{
		get
		{
			return isFading;
		}
	}

	public void Awake()
	{
		//ここでテクスチャ作る
		overTexture = Texture2D.whiteTexture;
		overTexture.Apply();
	}

	Coroutine fadeCoroutine;

	/// <summary>
	/// フェードイン、フェードアウトでシーンを切り替える
	/// </summary>
	/// <param name='scene'>シーン名</param>
	/// <param name='interval'>暗転にかかる時間(秒)</param>
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
	/// フェードイン、フェードアウトでシーンを切り替える
	/// </summary>
	/// <param name='scene'>シーン名</param>
	/// <param name='interval'>暗転にかかる時間(秒)</param>
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
	/// フェードイン、フェードアウトでシーンを切り替える
	/// </summary>
	/// <param name='scene'>シーン名</param>
	/// <param name='interval'>暗転にかかる時間(秒)</param>
	public void Fade(Color startColor, float interval = 0.5f, Processing processing = null)
	{
		Fade(startColor, interval, defaultSortOrder, processing);
	}

	/// <summary>
	/// フェードイン、フェードアウトでシーンを切り替える
	/// </summary>
	/// <param name='scene'>シーン名</param>
	/// <param name='interval'>暗転にかかる時間(秒)</param>
	public void Fade(Color startColor, float interval = 0.5f, int sortOrder = 5, Processing processing = null)
	{
		if (IsFading)
		{
			Debug.Log("フェード中です。");
			return;
		}

		fadeCoroutine = StartCoroutine(TransScene(interval, sortOrder, startColor, processing));
	}


	// シーン遷移用コルーチン
	IEnumerator TransScene(float interval, int sortOrder, Color startColor, Processing processing)
	{
		fadeCanvas = Instantiate(fadeCanvasPrefab).GetComponent<Canvas>();
		DontDestroyOnLoad(fadeCanvas.gameObject);
		fadeImage = fadeCanvas.GetComponentInChildren<Image>();
		fadeImage.color = startColor;

		//だんだん暗く
		this.isFading = true;
		float time = 0;
		while (time <= interval)
		{
			//this.FadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
			Color c = fadeImage.color;
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
			Color c = fadeImage.color;
			c.a = Mathf.Lerp(1.0f, 0.0f, time / interval);
			fadeImage.color = c;

			time += Time.deltaTime;
			yield return 0;
		}

		Destroy(fadeCanvas.gameObject);
		this.isFading = false;
	}

	public void FadeStop()
	{
		Destroy(fadeCanvas.gameObject);
		StopCoroutine(fadeCoroutine);
		isFading = false;
	}
}
