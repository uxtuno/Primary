using UnityEngine;
using System.Collections;

public class PauseMenuController : MyMonoBehaviour
{
	/// <summary>
	/// ポーズメニューへ至るまで、また戻るまでの状態
	/// </summary>
	private enum PauseState
	{
		None,
		Go,
		Run,
		Back
	}

	private enum TabletMoveState    // タブレットの移動状態
	{
		before,     // 移動前
		midstream,  // 移動中
		after       // 移動後
	}

	private GameObject canvas { get; set; }
	private bool isInitialized { get; set; }    // Initialize()が実行されたかどうか
	private bool isPause { get; set; }      // ポーズ状態かどうか
	private PauseState pauseState { get; set; }
	[SerializeField]
	private GameObject pauseMenuPrefab = null;
	private MenuParentScript menu { get; set; }
	[SerializeField]
	private GameObject tabletPCPrefab = null;
	private Transform tabletScreen { get; set; }
	private GameObject tabletPC { get; set; }
	private Transform tabletPCAxis { get; set; }
	private static Transform initTabletPCAxis { get; set; }
	private Transform tabletTransform { get; set; }
	private TabletMoveState tabletMoveState { get; set; }
	private static readonly Color StartupScreenColor = new Color(1f / 255 * 55, 1f / 255 * 96, 1f / 255 * 255);
	private static readonly Color EndScreenColor = Color.black;


	protected override void Awake()
	{
		base.Awake();
		OnLevelWasLoaded(Application.loadedLevel);
	}

	private void Initialize()
	{
		canvas = GameObject.Find("Canvas");
		isPause = false;
		pauseState = PauseState.None;

		if (tabletPCPrefab == null)
		{
			tabletPCPrefab = Resources.Load<GameObject>("Prefabs/TabletPC");
		}

		tabletPC = GameObject.Find("TabletPCPoint") as GameObject;

		var tabletPCAxisGo = GameObject.Find("TabletPCAxis");
		if(tabletPCAxisGo)
		{
			tabletPCAxis = tabletPCAxisGo.transform as Transform;
		}
		initTabletPCAxis = tabletPCAxis;

		if(tabletPC != null)
		{
			tabletTransform = tabletPC.transform;
		}
		tabletMoveState = TabletMoveState.before;
		isInitialized = true;
	}

	protected override void Update()
	{
		if (Application.loadedLevelName == Scenes.Menu.name || AdvancedWriteMessageSingleton.instance.isWrite)
		{
			return;
		}

		if(!player.ContainsItem(Items.Tablet))
		{
			return;
		}

		if (!isInitialized)
		{
			Initialize();
		}

		base.Update();
		ChangeState();

		switch (pauseState)
		{
			case PauseState.None:
				break;

			case PauseState.Go:
				ToGo();
				break;

			case PauseState.Run:
				ToRun();
				break;

			case PauseState.Back:
				ToBack();
				break;
		}
	}

	/// <summary>
	/// ポーズに移る際、また戻る際の状態の切り替え
	/// </summary>
	private void ChangeState()
	{
		if (Input.GetKeyDown(KeyCode.Pause) || Input.GetKeyDown(KeyCode.Escape))
		{
			switch (pauseState)
			{
				case PauseState.None:
					pauseState = PauseState.Go;
					break;

				case PauseState.Go:
					break;

				case PauseState.Run:
					pauseState = PauseState.Back;
					break;

				case PauseState.Back:
					break;
			}
		}
	}

	private void ToGo()
	{
		isPause = true;
		Pauser.Pause();

		Vector3 targetEulerAngles = new Vector3(90.0f, 0.0f, 0.0f);

		if (tabletMoveState == TabletMoveState.before && Application.loadedLevelName != Scenes.EndScene.name)
		{
			tabletPC = Instantiate(tabletPCPrefab, tabletTransform.position, tabletTransform.rotation) as GameObject;
			tabletPC.transform.parent = tabletTransform;

			StartCoroutine(TabletMovement(tabletPCAxis, targetEulerAngles, true));
			player.ShowMiniMap(false);
			ExamineIconManager.SetVisible(false);
		}

		if (tabletMoveState == TabletMoveState.after)
		{
			pauseState = PauseState.Run;
			tabletMoveState = TabletMoveState.before;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}
	}

	private void ToRun()
	{
		if (pauseMenuPrefab != null && menu == null)
		{
			if (tabletScreen = tabletPC.transform.FindChild("Screen") as Transform)
			{
				tabletScreen.GetComponent<MeshRenderer>().material.color = StartupScreenColor;
			}

			menu = Instantiate(pauseMenuPrefab).GetComponent<MenuParentScript>();
			menu.MenuSelected += new MenuSelectEventHandrer(OnMenuSelected);
		}
	}

	private void ToBack()
	{
		if (menu != null)
		{

			Destroy(menu.gameObject);
		}

		if (tabletScreen && tabletScreen.GetComponent<MeshRenderer>().material.color != EndScreenColor)
		{
			tabletScreen.GetComponent<MeshRenderer>().material.color = EndScreenColor;
		}

		Vector3 targetEulerAngles = new Vector3(360.0f, 0.0f, 0.0f);

		if (tabletPCAxis.localEulerAngles.x != 0 && tabletMoveState == TabletMoveState.before)
		{

			StartCoroutine(TabletMovement(tabletPCAxis, targetEulerAngles));
		}

		if (tabletMoveState == TabletMoveState.after)
		{
			if (tabletPCAxis.localEulerAngles != targetEulerAngles)
			{
				tabletPCAxis.localEulerAngles = targetEulerAngles;
			}

			if (tabletPC != null)
			{
				Destroy(tabletPC);
			}

			pauseState = PauseState.None;
			tabletMoveState = TabletMoveState.before;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
			Pauser.Resume();
			isPause = false;
			player.ShowMiniMap(true);
			ExamineIconManager.SetVisible(true);
		}
	}

	private IEnumerator ToBackLoadLevel(string levelName)
	{
		while (isPause)
		{
			ToBack();
			yield return null;
		}

		SceneChangeSingleton.instance.LoadLevel(levelName);
	}

	void OnLevelWasLoaded(int level)
	{
		Pauser.SceneChangeInitialize();
		isInitialized = false;
		if (Application.loadedLevelName == Scenes.Menu.name || AdvancedWriteMessageSingleton.instance.isWrite)
		{
			return;
		}
		if(!player.ContainsItem(Items.Tablet))
		{
			return;
		}

		Initialize();
	}

	bool OnMenuSelected(string itemName)
	{
		switch (itemName)
		{
			case "Back":
			case MenuParentScript.CanselMessage:
				pauseState = PauseState.Back;
				break;

			case "EndGame":
				StartCoroutine(ToBackLoadLevel(Scenes.Menu.name));
				break;

			case "ReStart":
				StartCoroutine(ToBackLoadLevel(Application.loadedLevelName));
				break;
		}

		return false;
	}

	/// <summary>
	/// タブレットのTransformを指定角まで移動させる
	/// </summary>
	/// <param name="target">移動対象のオブジェクト</param>
	/// <param name="endRotation">回転先のオイラー角</param>
	/// <param name="isReverse">回転方向(trueにすると負の方向に回転)</param>///
	private IEnumerator TabletMovement(Transform target, Vector3 endRotation, bool isReverse = false)
	{
		tabletMoveState = TabletMoveState.midstream;

		Vector3 startRotation = target.localEulerAngles;
		Quaternion endQuaternion = new Quaternion();

		endQuaternion.eulerAngles = endRotation;

		int direction = 1;

		if (isReverse)
		{
			direction = -1;
		}

		float startTime = Time.time;

		while ((Mathf.Acos(Vector3.Dot(((target.localRotation * Vector3.forward) * direction), endQuaternion * Vector3.forward)) * 180.0f / Mathf.PI) > 0.1f)
		{
			float diff = Time.time - startTime;
			target.localEulerAngles = Vector3.Slerp(startRotation, endRotation, diff / 0.5f) * direction;
			yield return new WaitForSeconds(0.002f);
		}

		tabletMoveState = TabletMoveState.after;
	}
}
