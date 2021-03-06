﻿//#define DEBUG_SHOW_FPS

using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

/// <summary>
/// 突貫工事なので色々直したい
/// </summary>
public class GameManager : MyMonoBehaviour
{
	private static bool isCreated = false; // 複数生成防止

	private StageController _stageController = null;
	private static readonly string stageManagerTag = "StageManager";
	[SerializeField]
	private GameObject playerPrefab = null;
	[SerializeField]
	private GameObject checkIconPrefab = null; // 調べるアイコン
	[SerializeField]
	private GameObject grabIconPrefab = null; // 調べるアイコン
	private GameObject icon; // 調べるアイコン
							 //private List<Items> playerItemList; // プレイヤーのアイテム。シーン切り替え時の引継ぎ用

	/// <summary>
	/// 現在のステージ進行を管理するオブジェクト
	/// 現在未使用
	/// </summary>
	public StageController stageController
	{
		get
		{
			if (_stageController == null)
			{
				Debug.LogError("stageControllerが存在しません");
			}
			return _stageController;
		}

		private set { _stageController = value; }
	}

	protected override void Awake()
	{
		base.Awake();
		//Time.captureFramerate = 30;

		if (!isCreated)
		{
			DontDestroyOnLoad(gameObject);
			isCreated = true;
		}
		else
		{
			Destroy(gameObject);
			return;
		}

		OnLevelWasLoaded(Application.loadedLevel);
	}

	private Text text;

	[Conditional("DEBUG_SHOW_FPS")]
	private void InitShowFPS()
	{
		var go = new GameObject("FPS");
		var canvas = go.AddComponent<Canvas>();
		canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		var child = new GameObject("Text");
		child.transform.SetParent(go.transform);
		text = child.AddComponent<Text>();
		text.rectTransform.position = canvas.transform.position;
		text.font = Font.CreateDynamicFontFromOSFont("arial", 50);
		text.fontSize = 50;
		text.color = Color.blue;
		text.horizontalOverflow = HorizontalWrapMode.Overflow;
		text.verticalOverflow = VerticalWrapMode.Overflow;
	}

	public void OnLevelWasLoaded(int level)
	{
		InitShowFPS();
		GameObject go = GameObject.FindGameObjectWithTag(stageManagerTag);
		if (go != null)
		{
			stageController = go.GetComponent<StageController>();
		}

		GameObject respawnPoint = GameObject.Find("RespawnPoint");
		GameObject playerGameObject = GameObject.FindGameObjectWithTag(Tags.Player);
		Player player;
		if (playerGameObject != null)
		{
			player = playerGameObject.GetComponent<Player>();
		}

		Vector3 respawnPosition = Vector3.zero;
		Quaternion respawnRotation = Quaternion.identity;
		if (respawnPoint != null)
		{
			respawnPosition = respawnPoint.transform.position;
			respawnRotation = respawnPoint.transform.rotation;
			foreach (Transform child in respawnPoint.transform)
			{
				Destroy(child.gameObject);
			}
		}

		if (playerGameObject == null)
		{
			if (Application.loadedLevelName != SceneName.NormalClear &&
				Application.loadedLevelName != SceneName.ExtraClear &&
				Application.loadedLevelName != SceneName.Menu)
			{
				player = ((GameObject)Instantiate(playerPrefab, respawnPosition, respawnRotation)).GetComponent<Player>();
			}
		}
		else
		{
			// シーン上にプレイヤーが存在すれば
			player = playerGameObject.GetComponentInParent<Player>();

			if ((player != null) && (Application.loadedLevelName != Scenes.TutorialStage01.name))
			{
				player.AddItem(Items.Tablet);
			}
		}
	}

	private float oneSecondsCount;
	private int frameCount;
	void Update()
	{
		ShowFPS();
	}

	[Conditional("DEBUG_SHOW_FPS")]
	void ShowFPS()
	{
			text.text = (1.0f / Time.deltaTime).ToString();
	}

	public void HideIcon()
	{
		if (icon)
		{
			Destroy(icon);
		}
	}

	/// <summary>
	/// ステージを切り替える
	/// </summary>
	public void StageChange(string name)
	{
		//playerItemList = new List<Items>(player.GetItemList());
		SceneChangeSingleton.instance.LoadLevel(name);
	}
}
