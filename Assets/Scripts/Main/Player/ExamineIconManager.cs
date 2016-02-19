using UnityEngine;
using System.Collections;


/// <summary>
/// プレイヤーの前のものに応じたアイコン表示
/// </summary>
public static class ExamineIconManager
{
	public enum IconType
	{
		None,
		Check, // 調べる時のアイコン
		Hold, // 持ち運べるもののアイコン
		GotoNextStage, // ゴールのアイコン
		TakeCardKey, // カードキーを調べる時のアイコン
	}

	static private IconType _currentIconType = IconType.None; // 現在のアイコンタイプ
	//static private GUITexture icon; // 現在表示中のアイコン

	// アイコンのプレハブ
	static private GameObject centerIconPrefab;
	private static GameObject centerIcon;

	private static Sprite checkIconSprite;
	private static Sprite goToNextIconSprite;
	private static Sprite holdIconSprite;
	private static Sprite takeCardKeyIconSprite;

	static ExamineIconManager()
	{
		// 使用するアイコンはここで読み込み
		centerIconPrefab = Resources.Load<GameObject>("Prefabs/CheckIcon");

		checkIconSprite = Resources.Load<Sprite>("Sprites/Icons/CheckIcon");
		goToNextIconSprite = Resources.Load<Sprite>("Sprites/Icons/GoToNextIcon");
		holdIconSprite = Resources.Load<Sprite>("Sprites/Icons/HoldIcon");
		takeCardKeyIconSprite = Resources.Load<Sprite>("Sprites/Icons/TakeCardKeyIcon");
	}

	/// <summary>
	/// 現在のアイコンタイプを取得
	/// </summary>
	static public IconType currentIconType
	{
		get { return _currentIconType; }
		private set { _currentIconType = value; }
	}

	/// <summary>
	/// 中央アイコンを表示
	/// </summary>
	/// <param name="iconType"></param>
	static public void ShowIcon(IconType iconType)
	{
		// 現在表示中のアイコンでなければ
		//if (currentIconType != iconType)
		{
			if(centerIcon == null)
			{
				centerIcon = GameObject.Instantiate(centerIconPrefab);
			}

			currentIconType = iconType;
			switch (iconType)
			{
				case IconType.Check:
					centerIcon.GetComponentInChildren<GUITexture>().texture = checkIconSprite.texture;
					break;

				case IconType.Hold:
					centerIcon.GetComponentInChildren<GUITexture>().texture = holdIconSprite.texture;
					break;

				case IconType.GotoNextStage:
					centerIcon.GetComponentInChildren<GUITexture>().texture = goToNextIconSprite.texture;
					break;

				case IconType.TakeCardKey:
					centerIcon.GetComponentInChildren<GUITexture>().texture = takeCardKeyIconSprite.texture;
					break;
			}
		}
	}

	/// <summary>
	/// 表示中の中央アイコンを消去
	/// 表示中でなければ何もしない
	/// </summary>
	static public void HideIcon()
	{
		if(centerIcon != null)
		{
			centerIcon.GetComponentInChildren<GUITexture>().texture = null;
			currentIconType = IconType.None;
		}
	}

	/// <summary>
	/// アイコンの表示状態を変更
	/// </summary>
	/// <param name="visible">表示フラグ</param>
	static public void SetVisible(bool visible)
	{
		if (centerIcon != null)
		{
			centerIcon.GetComponentInChildren<GUITexture>().enabled = visible;
		}
	}
}
