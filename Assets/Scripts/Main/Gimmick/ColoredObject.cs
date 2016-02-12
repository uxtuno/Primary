using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// プライマリーレイガンによって完全に消失されるオブジェクト
/// ColorBlockと同じく基底クラスにColorObjectBaseを持つが、クラスの役割分担がうまくいってないため
/// 共通コードが目立つ。そのうち直したい
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
	}

	protected override void Update()
	{
		isPlayback = true;

		if (rigidbody != null)
		{
			rigidbody.WakeUp();
		}
	}

	protected override void LateUpdate()
	{
		if (!collider.enabled)
		{
			return;
		}

		if (IrradiationColor.state == objectColor.state)
		{
			FadeAway();
			if (isDisappearance)
			{
				return;
			}
		}
		else
		{
			if (endurance < 1.0f)
			{
				ReGeneration();
			}
		}

		if (IrradiationColor.state != ColorState.NONE)
		{
			if (isUseParticle)
			{
				irradiation.startColor = (Color)IrradiationColor;
				irradiation.Play();
				if (endurance != 1.0f && !isDisappearance)
				{
					PlayParticle(duringDisappearance);
				}
				else
				{
					regeneration.Stop();
				}
			}
		}
		else
		{
			if (isUseParticle)
			{
				irradiation.Stop();
			}
		}

		//Debug.Log(endurance);
		renderer.material.color = objectColor.color;
		IrradiationColor = ColorState.NONE;
	}

	/// <summary>
	/// 消失中
	/// </summary>
	private void FadeAway()
	{
		endurance -= Time.deltaTime / eraseTime;
		objectColor.alpha = defaultAlpha * endurance;

		if (endurance < 0.0f)
		{
			endurance = 0.0f;
			OnDisappearance();
		}
	}

	// 再生中
	private void ReGeneration()
	{
		endurance += Time.deltaTime;
		if (endurance > 1.0f)
		{
			endurance = 1.0f;
			OnPlayBack();
		}
		objectColor.alpha = defaultAlpha * endurance;
	}

	/// <summary>
	/// 完全に消失した瞬間
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
	}

	// 再生完了した瞬間
	protected override void OnPlayBack()
	{
		base.OnPlayBack();
		if(isUseParticle)
		{
			duringDisappearance.Stop();
		}
	}
}
