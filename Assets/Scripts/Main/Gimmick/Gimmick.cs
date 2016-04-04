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
	[SerializeField, Tooltip("ギミック起動時に表示するアイコン")]
	private GameObject pickIconPrefab;
	[SerializeField, Tooltip("アイコンを表示する位置")]
	protected Transform pickIconShowPosition;

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
		if (pickIconShowPosition == null)
			ShowPickIcon(transform.position);
		else
			ShowPickIcon(pickIconShowPosition.position);
	}

	private Vector3 pickIconDefaultScale; // 本来のスケールを記録

	/// <summary>
	/// ギミック起動時に呼ぶ
	/// </summary>
	/// <param name="pickIconPrefab"></param>
	protected void ShowPickIcon(Vector3 position, Transform parent = null)
	{
		if (pickIcon != null)
		{
			Destroy(pickIcon);
		}

		pickIcon = Instantiate(pickIconPrefab, position, Quaternion.identity) as GameObject;
		pickIconDefaultScale = pickIcon.transform.lossyScale;
		if(parent != null)
		{
			pickIcon.transform.SetParent(parent);
			pickIcon.transform.rotation = Quaternion.identity;
			
			pickIcon.transform.localPosition = Vector3.zero;
			Vector3 localScale = pickIcon.transform.localScale;
			Vector3 lossyScale = pickIcon.transform.lossyScale;

			// 親のスケールに影響を受けないように計算
			pickIcon.transform.localScale = new Vector3(
				localScale.x * (pickIconDefaultScale.x / lossyScale.x),
				localScale.y * (pickIconDefaultScale.y / lossyScale.y),
				localScale.z * (pickIconDefaultScale.z / lossyScale.z));
		}
	}
}
