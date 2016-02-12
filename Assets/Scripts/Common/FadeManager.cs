﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FadeManager : MonoBehaviour {
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
				DontDestroyOnLoad(SceneChangeSingleton.gameObject);
			}

			return _instance;
		}
	}

	List<string> scenePaths { get; set; }   // 読み込み可能なシーンのパス
	private readonly int Layer = 0; // 全てのオブジェクトを覆い隠せるようにレイヤーを最前面に
	private Texture2D overTexture;
	private float FadeAlpha = 0.0f; // フェード中の透明度
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

	public void OnGUI()
	{
		if (!this.IsFading)
			return;

		GUI.depth = Layer;

		//透明度を更新してテクスチャを描画
		GUI.color = new Color(1.0f, 1.0f, 1.0f, FadeAlpha);
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), overTexture);
	}

	/// <summary>
	/// フェードイン、フェードアウトでシーンを切り替える
	/// </summary>
	/// <param name='scene'>シーン名</param>
	/// <param name='interval'>暗転にかかる時間(秒)</param>
	public void Fade(string scene, float interval = 0.5f, Processing processing = null)
	{
		if (IsFading)
		{
			Debug.Log("フェード中です。");
			return;
		}

		StartCoroutine(TransScene(scene, interval, processing));
	}

	// シーン遷移用コルーチン
	IEnumerator TransScene(string scene, float interval, Processing processing)
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

		// フェード中に実行する処理
		if(processing != null)
		{
			processing();
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
}
