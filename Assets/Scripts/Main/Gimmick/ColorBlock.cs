using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
///     反消失型ブロック
/// </summary>
public class ColorBlock : ColorObjectBase
{
	[SerializeField] private bool isDisappearanceStart = false; // 消失中
	private List<Switch> ridingSwitches = new List<Switch>(); // 自分が乗っているSwitch
	private GameObject rideCollider; // 何かに乗るための判定用オブジェクト

	protected override void Awake()
	{
		base.Awake();

		if (useSounds.Count == 0)
		{
			useSounds.Add(SoundCollector.SoundName.CubeElase);
			useSounds.Add(SoundCollector.SoundName.CubeRestoration);
		}

		// 消失した瞬間
		if (eraseTime == 0 || isDisappearanceStart)
		{
			endurance = 0.0f;
			objectColor.alpha = 0.0f;
			OnDisappearance();
		}

		var meshCollider = collider.GetComponent<MeshCollider>();
		if (meshCollider != null)
		{
			if (!meshCollider.convex)
			{
				meshCollider.convex = true;
			}
		}

		//tag = TagName.Untagged;

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
	}

	/// <summary>
	/// オブジェクトが完全に消去された瞬間に呼ばれる
	/// </summary>
	protected override void OnDisappearance()
	{
		base.OnDisappearance();
		PlayParticle(completeDisappearance);
		if (collider.GetComponent<MeshCollider>() != null)
		{
			if (!collider.GetComponent<MeshCollider>().convex)
			{
				return;
			}
		}
		collider.isTrigger = true;

		// 物理演算関係の挙動
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

		// タイトル画面では音がならないように
		if (Application.loadedLevelName == SceneName.Menu)
			return;
		SoundPlayerSingleton.instance.PlaySE(gameObject, soundCollector[useSounds[0]], false, true, 0.5f, 0.0f, true);
	}

	/// <summary>
	/// オブジェクトが再生する瞬間に呼ばれる
	/// </summary>
	protected override void OnPlayBack()
	{
		// 中のアイテムを入手できなくする
		foreach (var item in items)
		{
			item.isAcquisition = false;
		}

		isDisappearance = false;

		// 持ち運び可能にする
		if (GetComponent<GraspItem>() != null)
		{
			GetComponent<GraspItem>().enabled = true;
		}

		if (collider.GetComponent<MeshCollider>() != null)
		{
			if (!collider.GetComponent<MeshCollider>().convex)
			{
				return;
			}
		}
		collider.isTrigger = false;

		// 重力を復活させる
		if (rigidbody != null)
		{
			rigidbody.isKinematic = false;
		}

		// 再び何かに乗せることができるようになる
		rideCollider.SetActive(true);

		SoundPlayerSingleton.instance.PlaySE(gameObject, soundCollector[useSounds[1]], false, true, 0.25f, 0.0f, true);
	}

	/// <summary>
	/// 何も照射されてない時に呼ばれる
	/// </summary>
	protected override void OnUnirradiated()
	{
		base.OnUnirradiated();

		if (isDisappearance)
			FadeAway();
		else
			ReGeneration();
	}

	private void OnTriggerEnter(Collider other)
	{
		var switchComponent = other.GetComponentInParent<Switch>();
		if (switchComponent == null)
			return;

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