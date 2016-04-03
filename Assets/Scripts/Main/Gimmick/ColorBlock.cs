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

	//protected override void LateUpdate()
	//{
	//	// 半透明で判定が無い状態
	//	if (collider.isTrigger)
	//	{
	//		//if (IrradiationColor.state != objectColor.state && IrradiationColor.state != ColorState.NONE)
	//		//{
	//		//	// 自分の色とは違う色が当たっているので再生させる
	//		//	ReGeneration();
	//		//}
	//		//else if(endurance != 0.0f)
	//		//{
	//		//	// 自分の色と同じ色が当たっているので消失させる
	//		//	PlayParticle(duringDisappearance);
	//		//	FadeAway();
	//		//}
	//	}
	//	else // 判定がある状態
	//	{
	//		// 同じ色の光線が当たっているので
	//		if (IrradiationColor.state == objectColor.state)
	//		{
	//			FadeAway();
	//		}
	//		else
	//		{
 //               if (endurance < 1.0f)
 //               {
	//			    ReGeneration();
 //               }
	//		}
	//	}

 //       if (IrradiationColor.state != ColorState.NONE)
 //       {
	//		// 白なら光を反射するので消えない
	//		if (isUseParticle && objectColor.state != ColorState.WHITE)
	//		{
	//			irradiation.startColor = (Color)IrradiationColor;
	//			irradiation.Stop();
	//			irradiation.Play();
	//		}
 //       }
 //       else
 //       {
	//		// 何も光線が当たっていないのでエフェクトを消す
	//		if (isUseParticle)
	//		{
	//			irradiation.Stop();
	//		}
 //       }

	//	if(IrradiationColor.state == objectColor.state)
	//	{
	//		if(endurance != 0)
	//		{
	//			// 消失中エフェクト
	//			PlayParticle(duringDisappearance);
	//		}
	//	}
	//	else if(IrradiationColor.state != ColorState.NONE)
	//	{
	//		if (endurance != 1.0f)
	//		{
	//			// 再生中エフェクト
	//			PlayParticle(regeneration);
	//		}
	//		else
	//		{
	//			if (isUseParticle)
	//			{
	//				regeneration.Stop();
	//			}
	//		}
	//	}
	//	else
	//	{
	//		if (isUseParticle)
	//		{
	//			duringDisappearance.Stop();
	//			regeneration.Stop();
	//		}
	//	}
	//	renderer.material.color = objectColor.color;

	//	// 毎フレーム照射されている色をリセット
	//	IrradiationColor = ColorState.NONE;
	//}

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
