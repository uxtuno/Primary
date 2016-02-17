using UnityEngine;
using System.Collections;

public class MenuManager : MyMonoBehaviour
{
	private enum State
	{
		None,
		Title,
		TitleScroll,
		TopMenu,
		SelectStageMenu,
	}

	[SerializeField]
	private GameObject primaryMenuPrefab = null;
	private MenuParentScript primaryMenu = null;
	[SerializeField]
	private GameObject secondaryMenuPrefab = null;
	private MenuParentScript secondaryMenu = null;
	[SerializeField]
	private MovableObjectController titleController = null;

	private State currentState = State.None;

	void Start()
	{
		//PopPrimaryMenu();
		currentState = State.Title;
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}

	protected override void Update()
	{
		switch (currentState)
		{
			case State.Title:
				if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
				{
					titleController.Switch();
					currentState = State.TitleScroll;
				}
				break;

			case State.TitleScroll:
				if (titleController.currentState == MovableObjectController.State.Stop)
				{
					currentState = State.TopMenu;
					PopPrimaryMenu();
				}
				break;
		}
	}

	/// <summary>
	/// トッポメニュー用コールバック
	/// </summary>
	/// <param name="itemName"></param>
	/// <returns></returns>
	bool OnPrimaryMenuSelected(string itemName)
	{
		Debug.Log(itemName);
		switch (itemName)
		{
			case "NewGame":
				SceneChangeSingleton.instance.LoadLevel(Scenes.TutorialStage01.name);
				break;

			case "StageSelect":
				secondaryMenu = Instantiate(secondaryMenuPrefab).GetComponent<MenuParentScript>();
				if (secondaryMenu != null)
				{
					secondaryMenu.MenuSelected += new MenuSelectEventHandrer(OnSecondaryMenuSelected);
				}
				break;

			case "Exit":
				Application.Quit();
				break;

			case MenuParentScript.CanselMessage:
				return true;
		}

		return false;
	}

	/// <summary>
	/// ステージセレクト用コールバック
	/// </summary>
	/// <param name="itemName"></param>
	/// <returns></returns>
	bool OnSecondaryMenuSelected(string itemName)
	{
		switch (itemName)
		{
			case MenuParentScript.CanselMessage:
				PopPrimaryMenu();
				break;

			default:
				// データからステージ名を読み込んでくる
				StageData stageData = Resources.Load<StageData>("StageData/StageData");
				SceneChangeSingleton.instance.LoadLevel(stageData.param[int.Parse(itemName)].Name);
				break;
		}

		return false;
	}

	void PopPrimaryMenu()
	{
		primaryMenu = ((GameObject)Instantiate(primaryMenuPrefab)).GetComponent<MenuParentScript>();

		if (primaryMenu != null)
		{
			primaryMenu.MenuSelected += new MenuSelectEventHandrer(OnPrimaryMenuSelected);
		}
	}
}
