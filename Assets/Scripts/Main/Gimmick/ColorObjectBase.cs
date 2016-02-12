using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// レーザーによって消去できる色のついたオブジェクトの抽象クラス
/// </summary>
public abstract class ColorObjectBase : Gimmick
{
	[SerializeField]
	private ObjectColor _objectColor;  // オブジェクトの色

	private bool _isDisappearance = false;
	/// <summary>
	/// 消失状態を切り替える(true : 消失)
	/// </summary>
	public bool isDisappearance
	{
		get { return _isDisappearance; }
		set
		{
			_isDisappearance = value;
			if(!(collider is MeshCollider))
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

	protected static readonly float eraseTime = 2.0f;   // 消えるまでの時間(秒)
	protected float defaultAlpha = 0.0f;   // 透明度の初期値
	protected ObjectColor IrradiationColor = ColorState.NONE;   // 現在のフレームで照射されているレーザーの色
	private float currentFrame; // 現在のフレーム
	protected bool isPlayback = false;
	protected float endurance = 1.0f; // 現在の耐久値

	protected List<Item> items = new List<Item>();

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

		// パーティクルを使用するなら色を設定
		if (isUseParticle)
		{
			duringDisappearance.startColor = (Color)objectColor;
			completeDisappearance.startColor = (Color)objectColor;
			regeneration.startColor = (Color)objectColor;
			burst.startColor = (Color)objectColor;
		}
	}

	/// <summary>
	/// ビームをあてる
	/// </summary>
	public virtual void Irradiated(ObjectColor laserColor)
	{
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

	/// <summary>
	/// オブジェクトが完全に消去された瞬間に呼ばれる
	/// </summary>
	protected virtual void OnDisappearance()
	{
		foreach (Item item in items)
		{
			item.isAcquisition = true;
		}

		PlayParticle(burst);
		isDisappearance = true;
		if(GetComponent<GraspItem>() != null)
		{
			GetComponent<GraspItem>().enabled = false;
        }
	}

	/// <summary>
	/// オブジェクトが再生する瞬間に呼ばれる
	/// </summary>
	protected virtual void OnPlayBack()
	{
		//Debug.Log("オブジェクト再生");
		foreach (Item item in items)
		{
			item.isAcquisition = false;
		}
		isDisappearance = false;
		if (GetComponent<GraspItem>() != null)
		{
			GetComponent<GraspItem>().enabled = true;
		}
	}

	/// <summary>
	/// エフェクトを再生
	/// 同時に再生しないエフェクトは止める
	/// もっとスマートに書けないか模索したい
	/// </summary>
	/// <param name="particleSystem"></param>
	protected void PlayParticle(ParticleSystem particleSystem)
	{
		if (!isUseParticle)
		{
			return;
		}

		if (particleSystem == completeDisappearance)
		{
			completeDisappearance.Play();
			duringDisappearance.Stop();
			duringDisappearance.Clear();
			regeneration.Stop();
			regeneration.Clear();

		}
		else if (particleSystem == duringDisappearance)
		{
			completeDisappearance.Stop();
			completeDisappearance.Clear();
			duringDisappearance.Play();
			regeneration.Stop();
			regeneration.Clear();
		}
		else if (particleSystem == regeneration)
		{
			completeDisappearance.Stop();
			completeDisappearance.Clear();
			duringDisappearance.Stop();
			duringDisappearance.Clear();
			regeneration.Play();
		}
		else if (particleSystem == burst)
		{
			completeDisappearance.Stop();
			completeDisappearance.Clear();
			duringDisappearance.Stop();
			duringDisappearance.Clear();
			regeneration.Stop();
			regeneration.Clear();
			burst.Play();
		}
		else if (particleSystem == null)
		{
			completeDisappearance.Stop();
			duringDisappearance.Stop();
			regeneration.Stop();
			burst.Stop();
		}
	}
}
