using UnityEngine;
using System.Collections;

/// <summary>
/// 反消失型ブロック
/// </summary>
public class ColorBlock : ColorObjectBase
{
	[SerializeField]
	private bool isDisappearanceStart = false; // 消失中

    protected override void Awake()
	{
		base.Awake();

		if (useSounds.Count == 0)
		{
			useSounds.Add(SoundCollector.SoundName.CubeElase);
			useSounds.Add(SoundCollector.SoundName.CubeRestoration);
		}

		// 消失した瞬間
		if(eraseTime == 0 || isDisappearanceStart)
		{
			endurance = 0.0f;
			objectColor.alpha = 0.0f;
			OnDisappearance();
		}

		MeshCollider meshCollider = collider.GetComponent<MeshCollider>();
        if (meshCollider != null)
		{
			if(!meshCollider.convex)
			{
				meshCollider.convex = true;
            }
		}
	}

	protected override void Update()
	{
		isPlayback = true;
	}

	// 消失完了時に呼ぶ
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

		if(rigidbody != null)
		{
			rigidbody.isKinematic = true;
			rigidbody.WakeUp();
		}

		// タイトル画面では音がならないように
		if (Application.loadedLevelName == SceneName.Menu)
			return;
		SoundPlayerSingleton.instance.PlaySE(gameObject, soundCollector[useSounds[0]], false, true, 0.5f, 0.0f, true);
	}

	// 再生完了時に呼ぶ
	protected override void OnPlayBack()
	{
		// 中のアイテムを入手できなくする
		foreach (Item item in items)
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

		SoundPlayerSingleton.instance.PlaySE(gameObject, soundCollector[useSounds[1]], false, true, 0.25f, 0.0f, true);
	}

	protected override void OnUnirradiated()
	{
		base.OnUnirradiated();

		if (isDisappearance)
			FadeAway();
		else
			ReGeneration();
	}
}
