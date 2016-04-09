using UnityEngine;

/// <summary>
///		物質消去グリッド
/// </summary>
public class SubstanceEraseGrid : Gimmick
{
	[SerializeField]
	private ObjectColor color = null; // 許可されない色
	private static readonly string burstParticleName = "Burst";
	private ParticleSystem burstParticle;
	private ParticleSystem eraseGridParticle1;
	private ParticleSystem eraseGridParticle2;
	private GraspItem collisionItem;

	protected override void Awake()
	{
		base.Awake();

		burstParticle = transform.FindChild(burstParticleName).GetComponent<ParticleSystem>();
		if (burstParticle == null)
		{
			Debug.Log("消失用のパーティクルが存在しません");
		}

		// パーティクルの色を設定
		foreach (ParticleSystem particle in GetComponentsInChildren<ParticleSystem>())
		{
			particle.startColor = (Color)color;
		}

		ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
		eraseGridParticle1 = particleSystems[0];
		eraseGridParticle2 = particleSystems[1];
		eraseGridParticle1.simulationSpace = ParticleSystemSimulationSpace.World;
		eraseGridParticle2.simulationSpace = ParticleSystemSimulationSpace.World;
	}

	void OnTriggerEnter(Collider other)
	{
		ColorObjectBase colorObject = other.GetComponent<ColorObjectBase>();
		if (colorObject == null)
		{
			return;
		}

		if (colorObject.GetComponent<GraspItem>() != null)
		{
			collisionItem = colorObject.GetComponent<GraspItem>();

			// &の結果はブロックと消去グリッドの共通色
			// その色がブロックの色を全て含んでないならreturn
			if ((colorObject.objectColor.state & color.state) < colorObject.objectColor.state)
			{
				return;
			}

			burstParticle.transform.position = colorObject.transform.position;
			burstParticle.startColor = (Color)colorObject.objectColor;
			burstParticle.Play();
			colorObject.GetComponent<GraspItem>().Respawn();
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (collisionItem != null && other.transform == collisionItem.transform)
		{
			collisionItem = null;
		}
	}
}