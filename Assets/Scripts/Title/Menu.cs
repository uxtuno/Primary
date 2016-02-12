using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
public class Menu : MyMonoBehaviour
{
	private enum TitleState
	{
		Title,
		Menu,
		SelectStage
	}

	private enum MenuState
	{
		Exit = -1,
		SelectStage,
		NewGame
	}

	private enum SelastStageState
	{
		TutorialStage1,
		TutorialStage2,
		Stage01,
		Stage02,
		Stage03,
		Stage04,
		Stage05
	}

	private struct Cursor<T>
	{
		public GameObject obj { get; set; }
		public T state { get; set; }
	}

	private TitleState titleState { get; set; }

	private GameObject titleCamera { get; set; }

	private GameObject menuCamera { get; set; }
	private Cursor<MenuState> menuCursor;
	private Dictionary<MenuState, Vector3> menuCursorPosition { get; set; }
	private int pos { get; set; }
	private int oldPos { get; set; }
	private bool isOnce { get; set; }

	[SerializeField]
	private MenuParentScript Menuitem;

	private GameObject selectStageCamera { get; set; }
	private Cursor<SelastStageState> selectStageCursor;
	private Dictionary<SelastStageState, Vector3> ssCursorPosition { get; set; }


	protected override void Awake()
	{
		return;
		base.Awake();

		titleState = TitleState.Title;

		titleCamera = GameObject.Find("TitleCamera");

		menuCamera = GameObject.Find("MenuCamera");
		menuCursor.obj = GameObject.Find("MenuCubeCursor");
		menuCursor.state = MenuState.NewGame;

		Vector3 newGameVec = GameObject.Find("NewGameText").transform.position;
		//Vector3 selectStageVec = GameObject.Find("SelectStageText").transform.position;
		Vector3 exitVec = GameObject.Find("ExitText").transform.position;
		Vector3 menuCursorVec = menuCursor.obj.transform.position;

		newGameVec = new Vector3(menuCursorVec.x, newGameVec.y + menuCursorVec.y, menuCursorVec.z);
		//selectStageVec = new Vector3(menuCursorVec.x, selectStageVec.y + menuCursorVec.y, menuCursorVec.z);
		exitVec = new Vector3(menuCursorVec.x, exitVec.y + menuCursorVec.y, menuCursorVec.z);

		menuCursorPosition = new Dictionary<MenuState, Vector3>();
		menuCursorPosition.Add(MenuState.NewGame, newGameVec);
		//menuCursorPosition.Add(MenuState.SelectStage, selectStageVec);
		menuCursorPosition.Add(MenuState.Exit, exitVec);
		isOnce = false;

		selectStageCamera = GameObject.Find("SelectStageCamera");
		selectStageCursor.obj = GameObject.Find("SelectStageCubeCursor");
		selectStageCursor.state = SelastStageState.TutorialStage1;

		Vector3 TutorialStage1Vec = GameObject.Find("TutorialStage1Text").transform.position;
		Vector3 TutorialStage2Vec = GameObject.Find("TutorialStage2Text").transform.position;
		Vector3 Stage1Vec = GameObject.Find("Stage1Text").transform.position;
		Vector3 Stage2Vec = GameObject.Find("Stage2Text").transform.position;
		Vector3 Stage3Vec = GameObject.Find("Stage3Text").transform.position;
		Vector3 Stage4Vec = GameObject.Find("Stage4Text").transform.position;
		Vector3 Stage5Vec = GameObject.Find("Stage5Text").transform.position;
		Vector3 SelectStageCursorVec = selectStageCursor.obj.transform.position;

		const float marginX = -8.0f;
		const float marginY = -10.0f;
		TutorialStage1Vec = new Vector3(TutorialStage1Vec.x + marginX, TutorialStage1Vec.y + marginY, SelectStageCursorVec.z);
		TutorialStage2Vec = new Vector3(TutorialStage2Vec.x + marginX, TutorialStage2Vec.y + marginY, SelectStageCursorVec.z);
		Stage1Vec = new Vector3(Stage1Vec.x + marginX, Stage1Vec.y + marginY, SelectStageCursorVec.z);
		Stage2Vec = new Vector3(Stage2Vec.x + marginX, Stage2Vec.y + marginY, SelectStageCursorVec.z);
		Stage3Vec = new Vector3(Stage3Vec.x + marginX, Stage3Vec.y + marginY, SelectStageCursorVec.z);
		Stage4Vec = new Vector3(Stage4Vec.x + marginX, Stage4Vec.y + marginY, SelectStageCursorVec.z);
		Stage5Vec = new Vector3(Stage5Vec.x + marginX, Stage5Vec.y + marginY, SelectStageCursorVec.z);

		ssCursorPosition = new Dictionary<SelastStageState, Vector3>();
		ssCursorPosition.Add(SelastStageState.TutorialStage1, TutorialStage1Vec);
		ssCursorPosition.Add(SelastStageState.TutorialStage2, TutorialStage2Vec);
		ssCursorPosition.Add(SelastStageState.Stage01, Stage1Vec);
		ssCursorPosition.Add(SelastStageState.Stage02, Stage2Vec);
		ssCursorPosition.Add(SelastStageState.Stage03, Stage3Vec);
		ssCursorPosition.Add(SelastStageState.Stage04, Stage4Vec);
		ssCursorPosition.Add(SelastStageState.Stage05, Stage5Vec);
	}

	protected override void Update()
	{
		base.Update();
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

		switch (titleState)
		{
			case TitleState.Title:
				TitleMove();
				break;

			case TitleState.Menu:
				MenuMove();
				break;

			case TitleState.SelectStage:
				SelectStageMove();
				break;
		}

	}

	/// <summary>
	/// タイトルのMove
	/// </summary>
	private void TitleMove()
	{
		if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
		{
			StartCoroutine(ObjectMovement(titleCamera, new Vector3(0.0f, -105.0f, 0.0f), TitleState.Menu));
		}
	}

	/// <summary>
	/// メニューのMove
	/// </summary>
	private void MenuMove()
	{
		if (!isOnce)
		{
			StartCoroutine(ObjectMovement(menuCamera, Vector3.zero, titleState));
			isOnce = true;
		}

		pos = (int)Input.GetAxisRaw("Vertical");

		if (pos != 0 && oldPos == 0)
		{
			//MenuCursorTransition(pos);
		}

		oldPos = pos;

		menuCursor.obj.transform.position = menuCursorPosition[menuCursor.state];

		if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
		{
			switch (menuCursor.state)
			{
				case MenuState.NewGame:
					SceneChangeSingleton.instance.LoadLevel("TutorialStage01");
					break;

				case MenuState.SelectStage:
					StartCoroutine(ObjectMovement(menuCamera, new Vector3(130.0f, 0.0f, -10.0f), TitleState.SelectStage));
					Debug.Log("SelectStage");
					break;

				case MenuState.Exit:
					Application.Quit();
					Debug.Log("Exit");
					break;
			}
		}
	}

	/// <summary>
	/// セレクトステージのMove
	/// </summary>
	private void SelectStageMove()
	{
		StartCoroutine(ObjectMovement(selectStageCamera, Vector3.zero, titleState));

		pos = (int)Input.GetAxisRaw("Vertical");

		if (pos != 0 && oldPos == 0)
		{
			SelectStageCursorTransition(pos);
		}

		oldPos = pos;

		selectStageCursor.obj.transform.position = ssCursorPosition[selectStageCursor.state];

		if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0))
		{
			SceneChangeSingleton.instance.LoadLevel(selectStageCursor.state.ToString());

		}
	}

	/// <summary>
	/// GameObjectを指定位置まで移動させる
	/// </summary>
	/// <param name="target">移動対象のオブジェクト</param>
	/// <param name="endPosition">移動先</param>
	/// <param name="state">コルーチン終了時のステート</param>
	private IEnumerator ObjectMovement(GameObject target, Vector3 endPosition, TitleState state)
	{
		Vector3 startPosition = target.transform.position;
		float startTime = Time.time;
		while (target.transform.position != endPosition)
		{
			float diff = Time.time - startTime;
			Debug.Log(diff);
			target.transform.position = Vector3.Lerp(startPosition, endPosition, diff / 1);
			yield return new WaitForSeconds(0.002f);
		}
		Debug.Log(target.transform.position == endPosition);
		titleState = state;
		StopCoroutine(ObjectMovement(target, endPosition, state));
	}

	/// <summary>
	/// メニューカーソルの遷移
	/// </summary>
	/// <param name="value"></param>
	private void MenuCursorTransition(int value)
	{
		if (value == 1)
		{
			switch (menuCursor.state)
			{
				case MenuState.NewGame:
					break;

				case MenuState.SelectStage:
					menuCursor.state = MenuState.NewGame;
					break;

				case MenuState.Exit:
					menuCursor.state = MenuState.SelectStage;
					break;
			}
		}
		else if (value == -1)
		{
			switch (menuCursor.state)
			{
				case MenuState.NewGame:
					menuCursor.state = MenuState.SelectStage;
					break;

				case MenuState.SelectStage:
					menuCursor.state = MenuState.Exit;
					break;

				case MenuState.Exit:
					break;
			}
		}
	}

	/// <summary>
	/// セレクトステージカーソルの遷移
	/// </summary>
	/// <param name="value"></param>
	private void SelectStageCursorTransition(int value)
	{
		if (value == 1)
		{
			switch (selectStageCursor.state)
			{
				case SelastStageState.TutorialStage1:
					break;

				case SelastStageState.TutorialStage2:
					selectStageCursor.state = SelastStageState.TutorialStage1;
					break;

				case SelastStageState.Stage01:
					selectStageCursor.state = SelastStageState.TutorialStage2;
					break;

				case SelastStageState.Stage02:
					selectStageCursor.state = SelastStageState.Stage01;
					break;

				case SelastStageState.Stage03:
					selectStageCursor.state = SelastStageState.Stage02;
					break;

				case SelastStageState.Stage04:
					selectStageCursor.state = SelastStageState.Stage03;
					break;

				case SelastStageState.Stage05:
					selectStageCursor.state = SelastStageState.Stage04;
					break;
			}
		}
		else if (value == -1)
		{
			switch (selectStageCursor.state)
			{
				case SelastStageState.TutorialStage1:
					selectStageCursor.state = SelastStageState.TutorialStage2;
					break;

				case SelastStageState.TutorialStage2:
					selectStageCursor.state = SelastStageState.Stage01;
					break;

				case SelastStageState.Stage01:
					selectStageCursor.state = SelastStageState.Stage02;
					break;

				case SelastStageState.Stage02:
					selectStageCursor.state = SelastStageState.Stage03;
					break;

				case SelastStageState.Stage03:
					selectStageCursor.state = SelastStageState.Stage04;
					break;

				case SelastStageState.Stage04:
					selectStageCursor.state = SelastStageState.Stage05;
					break;

				case SelastStageState.Stage05:
					break;
			}
		}
	}
}
*/