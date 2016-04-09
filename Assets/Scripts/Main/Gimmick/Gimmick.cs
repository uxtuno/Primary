using UnityEngine;

/// <summary>
///     全てのギミックはこれを継承する
/// </summary>
public abstract class Gimmick : EventObject
{
	private GameObject pickIcon;

	private Vector3 pickIconDefaultScale; // 本来のスケールを記録

	[SerializeField, Tooltip("ギミック起動時に表示するアイコン")]
	private GameObject pickIconPrefab;

	[SerializeField, Tooltip("アイコンを表示する位置")]
	protected Transform pickIconShowPosition;

	protected override void Awake()
	{
		base.Awake();
		pickIconPrefab = Resources.Load<GameObject>("Prefabs/Gimmicks/PickIcon");
	}

	/// <summary>
	///     ギミック起動時に呼ぶ
	/// </summary>
	protected void ShowPickIcon()
	{
		if (pickIconShowPosition == null)
			ShowPickIcon(transform.position);
		else
			ShowPickIcon(pickIconShowPosition.position);
	}

	/// <summary>
	///     ギミック起動時に呼ぶ
	/// </summary>
	protected void ShowPickIcon(Vector3 position, Transform parent = null)
	{
		if (pickIcon != null)
		{
			Destroy(pickIcon);
		}

		pickIcon = Instantiate(pickIconPrefab, position, Quaternion.identity) as GameObject;
		pickIconDefaultScale = pickIcon.transform.lossyScale;
		if (parent != null)
		{
			pickIcon.transform.SetParent(parent);
			pickIcon.transform.rotation = Quaternion.identity;

			pickIcon.transform.localPosition = Vector3.zero;
			var localScale = pickIcon.transform.localScale;
			var lossyScale = pickIcon.transform.lossyScale;

			// 親のスケールに影響を受けないように計算
			pickIcon.transform.localScale = new Vector3(
				localScale.x * (pickIconDefaultScale.x / lossyScale.x),
				localScale.y * (pickIconDefaultScale.y / lossyScale.y),
				localScale.z * (pickIconDefaultScale.z / lossyScale.z));
		}
	}
}