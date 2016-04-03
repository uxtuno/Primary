using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// レーザーによって消去できる色のついたオブジェクトの基底クラス
/// </summary>
public abstract class ColorObjectBase : Gimmick
{
	[SerializeField]
	private ObjectColor _objectColor;  // オブジェクトの色

	private bool _isDisappearance = false; // 消失状態を表すフラグ

	/// <summary>
	/// 消失状態を切り替える(true : 消失)
	/// </summary>
	public bool isDisappearance
	{
		get { return _isDisappearance; }
		set
		{
			_isDisappearance = value;
			if (!(collider is MeshCollider))
			{
				collider.isTrigger = value;
			}
		}
	}

	/// <summary>
	/// オブジェクトの色
	/// </summary>
	public ObjectColor objectColor
	{
		get { return _objectColor; }
	}

	protected static readonly float eraseTime = 1.0f;   // 消えるまでの時間(秒)
	protected float defaultAlpha = 0.0f;   // 透明度の初期値
	protected ObjectColor IrradiationColor = ColorState.NONE;   // 現在のフレームで照射されているレーザーの色
	private float currentFrame; // 現在のフレーム
	protected bool isPlayback = false; // 再生中
	protected float endurance = 1.0f; // 現在の耐久値。1～0に正規化された値

	// 子に含まれるItemを保持する
	protected List<Item> items = new List<Item>();

	// todo : エフェクトを全て手作業で設定しなければならないので作業量的に問題がある
	// ほぼ同じエフェクトを使うことになるのでプログラム上で読み込みたい
	[SerializeField]
	protected bool isUseParticle = false;
	[SerializeField]
	protected ParticleSystem duringDisappearance; // 消失中エフェクト
	[SerializeField]
	protected ParticleSystem completeDisappearance; // 消失後エフェクト
	[SerializeField]
	protected ParticleSystem regeneration; // 再生中エフェクト
	[SerializeField]
	protected ParticleSystem burst; // 消失時、爆発エフェクト
	[SerializeField]
	protected ParticleSystem irradiation; // 照射中エフェクト

	private GraspItem graspItem; // 持ち運び動作を制御

	protected override void Awake()
	{
		// 基底クラスのAwakeを呼ぶ
		// MyMonoBehaviourではオーバーライドするメソッドの最初に基底クラスのメソッドを呼ぶように設計している
		base.Awake();

		defaultAlpha = objectColor.alpha;
		currentFrame = Time.frameCount;

		// 子に存在するアイテムを入手不可に
		foreach (Transform child in transform)
		{
			Item item = child.GetComponent<Item>();
			if (item != null)
			{
				items.Add(item);
				item.isAcquisition = false;
			}
		}

		graspItem = GetComponent<GraspItem>();

		// パーティクルを使用するなら色を設定
		if (isUseParticle)
		{
			duringDisappearance.startColor = (Color)objectColor;
			completeDisappearance.startColor = (Color)objectColor;
			regeneration.startColor = (Color)objectColor;
			burst.startColor = (Color)objectColor;

			// 照射中エフェクトは照射されるレーザーの色に応じて変更するのでここでは設定しない
		}
	}

	/// <summary>
	/// ビームをあてる
	/// </summary>
	public virtual void Irradiated(ObjectColor laserColor)
	{
		// 同フレームに照射されている色を全て合成していく
		if (currentFrame == Time.frameCount)
		{
			IrradiationColor.state |= laserColor.state;
		}
		else
		{
			currentFrame = Time.frameCount;
			IrradiationColor.state = laserColor.state;
		}

		return;
	}

	protected override void LateUpdate()
	{
		if (!collider.enabled)
		{
			return;
		}

		if (IrradiationColor.state != ColorState.NONE)
		{
			if (isUseParticle)
			{
				// 消失中エフェクト
				irradiation.startColor = (Color)IrradiationColor;
				PlayParticle(irradiation);
			}
		}
		else
		{
			StopParticle(irradiation);
		}

		// レーザーの色とオブジェクトの色が一致
		if (IrradiationColor.state == objectColor.state)
		{
			// 消失中
			FadeAway();
		}
		else if (IrradiationColor.state != objectColor.state &&
				 IrradiationColor.state != ColorState.NONE)
		{
			ReGeneration();
			// レーザーの色とオブジェクトの色が不一致
			OnAnotherColorIrradiation();
		}
		else
		{
			// 再生中。ここでの再生は消失しきれなかった時などに
			// 元に戻ろうとする状態を言う
			if (endurance != 0.0f || endurance != 1.0f)
			{
				OnUnirradiated();
			}
		}

		renderer.material.color = objectColor.color;
		IrradiationColor = ColorState.NONE;
	}

	/// <summary>
	/// 消失中に呼ばれる
	/// </summary>
	protected void FadeAway()
	{
		if (endurance <= 0.0f)
			return;

		StopParticle(regeneration);
		PlayParticle(duringDisappearance);

		// 透明度を減算
		endurance -= Time.deltaTime / eraseTime;
		objectColor.alpha = defaultAlpha * endurance;

		if (endurance <= 0.0f)
		{
			StopParticle(duringDisappearance);

			// 消失
			endurance = 0.0f;
			if (isDisappearance)
				PlayParticle(completeDisappearance);
			else
				OnDisappearance();
		}
	}

	/// <summary>
	/// 再生中に呼ばれる
	/// </summary>
	protected void ReGeneration()
	{
		if (endurance >= 1.0f)
			return;

		StopParticle(completeDisappearance);
		PlayParticle(regeneration);

		// 透明度を加算
		endurance += Time.deltaTime;
		if (endurance >= 1.0f)
		{
			StopParticle(regeneration);

			endurance = 1.0f;

			if (isDisappearance)
				OnPlayBack(); // 復活
		}
		objectColor.alpha = defaultAlpha * endurance;
	}

	/// <summary>
	/// 別の色が照射されている時に呼ばれる
	/// </summary>
	protected virtual void OnAnotherColorIrradiation()
	{
		StopParticle(duringDisappearance);
	}

	/// <summary>
	/// 何も照射されてない時に呼ばれる
	/// </summary>
	protected virtual void OnUnirradiated()
	{
		StopParticle(duringDisappearance);
	}

	/// <summary>
	/// オブジェクトが完全に消去された瞬間に呼ばれる
	/// </summary>
	protected virtual void OnDisappearance()
	{
		// アイテムを入手可能に
		foreach (Item item in items)
		{
			item.isAcquisition = true;
		}

		// 爆発エフェクト
		PlayParticle(burst);

		isDisappearance = true;
		// 持ち運び不可に
		if (graspItem != null)
		{
			graspItem.enabled = false;
		}
	}

	/// <summary>
	/// オブジェクトが再生する瞬間に呼ばれる
	/// </summary>
	protected virtual void OnPlayBack()
	{
	}

	/// <summary>
	/// エフェクトを再生
	/// </summary>
	/// <param name="particleSystem"></param>
	protected void PlayParticle(ParticleSystem particleSystem)
	{
		if (!isUseParticle)
		{
			return;
		}

		particleSystem.Play();
	}

	/// <summary>
	/// エフェクトを停止
	/// </summary>
	/// <param name="particleSystem"></param>
	protected void StopParticle(ParticleSystem particleSystem)
	{
		if (!isUseParticle ||
			!particleSystem.isPlaying)
		{
			return;
		}

		particleSystem.Stop();
	}
}
