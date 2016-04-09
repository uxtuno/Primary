using UnityEngine;

/// <summary>
///     プライマリーレイガンによって完全に消失されるオブジェクト
///     ColorBlockと同じく基底クラスにColorObjectBaseを持つが、クラスの役割分担がうまくいってないため
///     共通コードが目立つ。そのうち直したい
/// </summary>
public class ColoredObject : ColorObjectBase
{
	protected override void Awake()
	{
		base.Awake();
		if (eraseTime == 0)
		{
			objectColor.alpha = 0.0f;
			OnDisappearance();
		}

		if (useSounds.Count == 0)
		{
			useSounds.Add(SoundCollector.SoundName.Explosion);
		}
	}

	protected override void Update()
	{
		isPlayback = true;

		if (rigidbody != null)
		{
			rigidbody.WakeUp();
		}
	}

	/// <summary>
	///     完全に消失した瞬間
	/// </summary>
	protected override void OnDisappearance()
	{
		base.OnDisappearance();
		if (isUseParticle)
		{
			irradiation.Stop();
			duringDisappearance.Stop();
		}

		collider.enabled = false;
		if (collider.GetComponent<MeshCollider>() != null)
		{
			return;
		}

		if (rigidbody != null)
		{
			rigidbody.isKinematic = true;
			rigidbody.WakeUp();
		}

		renderer.enabled = false;
		SoundPlayerSingleton.instance.PlaySE(gameObject, soundCollector[useSounds[0]], false, true, 0.5f, 0.0f, true);
	}

	/// <summary>
	/// 何も照射されてない時に呼ばれる
	/// </summary>
	protected override void OnUnirradiated()
	{
		base.OnUnirradiated();
		ReGeneration();
	}

	/// <summary>
	/// オブジェクトが再生する瞬間に呼ばれる
	/// </summary>
	protected override void OnPlayBack()
	{
		base.OnPlayBack();
		if (isUseParticle)
		{
			duringDisappearance.Stop();
		}
	}
}