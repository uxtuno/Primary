using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// フィールドに固定されているレイガン
/// </summary>
[ExecuteInEditMode]
public class FixedRaygun : Gimmick, ISwitchEvent
{
	[SerializeField]
	private GameObject lookTarget = null; // 向きたいターゲット。実行時は無効
	private EraseLaser eraseLaser = null; // 照射するレーザー

	private bool _isIrradiation;

	[SerializeField]
	private bool playOnAwake = true; // 開始時から照射するか

	/// <summary>
	/// 照射状態を取得
	/// </summary>
	public bool isIrradiation
	{
		get
		{
			return _isIrradiation;
		}

		private set
		{
			_isIrradiation = value;
			eraseLaser.IsShow = value;
		}
	}

	/// <summary>
	/// スイッチとしての状態
	/// </summary>
	public bool switchState
	{
		get
		{
			return isIrradiation;
		}
	}

	protected override void Awake()
	{
		base.Awake();

		// 子としてeraseLaserを持ってるはず
		eraseLaser = GetComponentInChildren<EraseLaser>();

		if (eraseLaser == null)
		{
			Debug.Log("Raygunを子として持つ必要があります");
		}

		// 初期動作
		if (playOnAwake)
		{
			isIrradiation = true;
		}
		else
		{
			isIrradiation = false;
		}
	}

	protected override void Update()
	{
		if (Application.isPlaying)
		{
			// 照射している
			if (isIrradiation)
			{
				eraseLaser.Irradiation();
			}
		}
		else // 編集モードのみ
		{
			if (lookTarget != null)
			{
				transform.LookAt(lookTarget.transform);
			}
		}
	}

	/// <summary>
	/// 照射開始/停止
	/// </summary>
	public void Switch()
	{
		isIrradiation = !isIrradiation;
		ShowPickIcon();
	}
}
