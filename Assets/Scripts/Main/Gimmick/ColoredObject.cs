using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     プライマリーレイガンによって完全に消失されるオブジェクト
///     ColorBlockと同じく基底クラスにColorObjectBaseを持つが、クラスの役割分担がうまくいってないため
///     共通コードが目立つ。そのうち直したい
/// </summary>
public class ColoredObject : ColorObjectBase
{
	private List<Switch> ridingSwitches = new List<Switch>(); // 自分が乗っているSwitch
	private GameObject rideCollider; // 何かに乗るための判定用オブジェクト

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

		// RideOnタグが付いたオブジェクトを探し格納
		// スイッチの上に乗るために使用するColliderがついたオブジェクト
		// 消失時に無効果したりするために使用
		foreach (var child in GetComponentsInChildren<Transform>())
		{
			if (child.tag == TagName.RideOn)
			{
				rideCollider = child.gameObject;
				break;
			}
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

		// 消失したのでスイッチから自分の情報を消す
		ridingSwitches.ForEach(obj => obj.RemoveHitObject(rideCollider));

		// あたり判定を無効化して何かに乗ることができないようにする
		if (rideCollider != null)
			rideCollider.SetActive(false);

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

	private void OnTriggerEnter(Collider other)
	{
		var switchComponent = other.GetComponentInParent<Switch>();
		if (switchComponent == null)
			return;

		// スイッチに触れたら情報を送信するため格納
		ridingSwitches.Add(switchComponent);
	}

	private void OnTriggerExit(Collider other)
	{
		var switchComponent = other.GetComponentInParent<Switch>();
		if (switchComponent == null)
			return;

		ridingSwitches.Remove(switchComponent);
	}
}