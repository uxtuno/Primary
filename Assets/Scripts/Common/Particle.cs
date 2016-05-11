using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using System.Threading;
using Object = UnityEngine.Object;

public class Particle : MyMonoBehaviour
{
	private new ParticleSystem particleSystem;

	struct ParticleData
	{
		public Vector3 direction;
		public float vy;
		public float vy2;
		public float speed;
		public float cos;
	}

	private ParticleSystem.Particle[] particles;
	private ParticleData[] particleData = null;
	private float distance;
	private Thread particlePoitionCalculateThread = null;
	private const int maxParticle = 2000;

	IEnumerator Start()
	{
		particleSystem = gameObject.GetComponent<ParticleSystem>();
		distance = 1.0f;
		isGenerated = false;
		isInit = false;
		isEnd = false;

		new Thread(new ThreadStart(ParticleGenerate)).Start();
		while (true)
		{
			yield return null;
			if (isGenerated)
				break;
		}

		particleSystem.maxParticles = maxParticle;
		particleSystem.Emit(maxParticle);
		particleSystem.GetParticles(particles);

		particlePoitionCalculateThread = new Thread(
			new ThreadStart(ParticlePositionCalculate));

		particlePoitionCalculateThread.Start();
	}

	private static readonly float oneFrameSeconds = 0.032f;

	private bool isGenerated;
	private void ParticleGenerate()
	{
		particles = new ParticleSystem.Particle[maxParticle];
		particleData = new ParticleData[maxParticle];
		lock (syncObj)
		{
			isGenerated = true;
		}
	}

	private bool isInit;
	private void ParticleInitialize()
	{
		System.Random r = new System.Random();
		for (int i = 0; i < maxParticle; i++)
		{
			particles[i].position = Vector3.zero;
			particles[i].velocity = Vector3.zero;
			particles[i].size = 0.1f;

			float rotateX = (float)r.NextDouble() * 360;
			float rotateY = (float)r.NextDouble() * 180;
			float rotateZ = (float)r.NextDouble() * 360;

			Quaternion q = new Quaternion();
			q.eulerAngles = new Vector3(rotateX, rotateY, rotateZ);
			particleData[i].direction = q * Vector3.forward;
			particleData[i].speed = 1;
			particleData[i].cos = i / ((float)maxParticle / 10);
		}

		lock (syncObj)
		{
			isInit = true;
		}
	}

	private bool isEnd;
	private void ParticlePositionCalculate()
	{
		ParticleInitialize();

		while (true)
		{
			if (!isInit)
			{
				Thread.Sleep(0);
				continue;
			}

			Vector3 newPosition = new Vector3();
			for (int i = 0; i < particles.Length; i++)
			{
				Vector3 direction = particleData[i].direction;
				float mul = (particleData[i].speed * (Mathf.Sin(particleData[i].cos))) * distance;

				direction.x *= mul;
				direction.z *= mul;
				direction.y *= mul * 5.0f;
				newPosition.x = direction.x;
				newPosition.y = direction.y + particleData[i].vy2;
				newPosition.z = direction.z;

				particleData[i].cos += oneFrameSeconds / 10.0f;
				particleData[i].vy += oneFrameSeconds / 20.0f;
				particleData[i].vy2 += particleData[i].vy;

				// スレッドの外で参照されるのはここだけ
				lock (syncObj)
				{
					particles[i].position = newPosition;
					particles[i].size -= 0.08f * oneFrameSeconds;

					if (particles[i].size < 0.0f)
					{
						particles[i].size = 0.0f;
						isEnd = true;
						break;
					}
				}

			}

			distance *= 1.04f;
			Thread.Sleep(Mathf.FloorToInt(1000 * oneFrameSeconds));
		}
	}

	void OnApplicationQuit()
	{
		particlePoitionCalculateThread.Abort();
	}

	protected override void Update()
	{
		if (!isInit)
			return;

		Vector3 newPosition = new Vector3();

		if (isEnd)
		{
			Destroy(gameObject);
			particlePoitionCalculateThread.Abort();
		}

		particleSystem.SetParticles(particles, particles.Length);
		transform.Rotate(0.0f, 120f * Time.deltaTime, 0.0f);
	}

	private Object syncObj = new Object();
}