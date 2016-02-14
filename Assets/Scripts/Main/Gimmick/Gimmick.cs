using UnityEngine;
using System.Collections;

/// <summary>
/// ギミックが外部に公開する情報
/// 独自の情報を持たせたければこれを継承して作成
/// </summary>
public abstract class GimmickState
{
	public bool frag;
}

/// <summary>
/// 全てのギミックはこれを継承する
/// </summary>
public abstract class Gimmick : EventObject
{
	[SerializeField, Tooltip("ギミック起動時にアイコンを表示する位置")]
	private Transform pickIconPosition;
	private GameObject pickIconPrefab;
	private GameObject pickIcon;

	protected override void Awake()
	{
		base.Awake();
		pickIconPrefab = Resources.Load<GameObject>("Prefabs/Gimmicks/PickIcon");
	}

	/// <summary>
	/// ギミック起動時に呼ぶ
	/// </summary>
	/// <param name="pickIconPrefab"></param>
	protected void ShowPickIcon()
	{
		ShowPickIcon(pickIconPosition.position);
	}

	/// <summary>
	/// ギミック起動時に呼ぶ
	/// </summary>
	/// <param name="pickIconPrefab"></param>
	protected void ShowPickIcon(Vector3 position)
	{
		if (pickIcon != null)
		{
			Destroy(pickIcon);
		}

		pickIcon = Instantiate(pickIconPrefab, position, Quaternion.identity) as GameObject;
	}
}
