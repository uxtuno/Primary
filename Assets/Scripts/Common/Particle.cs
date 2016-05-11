using UnityEngine;
using System.Collections;
using System.Linq;
using System.Threading;

public class Particle : MyMonoBehaviour
{
	private new ParticleSystem particleSystem;

	struct ParticleData
	{
		public Vector3 position;
		public float size;
		public Vector3 direction;
		public float vy;
		public float vy2;
		public float speed;
		public float cos;
	}

	private ParticleSystem.Particle[] particles;
	private ParticleData[] particleData;
	private float distance = 2.0f;
	private Thread particlePoitionCalculateThread = null;

	void Start()
	{
		particleSystem = gameObject.GetComponent<ParticleSystem>();

		const int maxParticle = 1000;
		particleSystem.maxParticles = maxParticle;
		particles = new ParticleSystem.Particle[maxParticle];
		particleData = new ParticleData[maxParticle];
		particleSystem.Emit(maxParticle);
		particleSystem.GetParticles(particles);

		for (int i = 0; i < maxParticle; i++)
		{
			particles[i].position = Vector3.zero;
			particles[i].velocity = Vector3.zero;
			particles[i].size = 0.1f;

			float rotateX = Random.value * 360;
			float rotateY = Random.value * 180;
			float rotateZ = Random.value * 360;

			Quaternion q = new Quaternion();
			q.eulerAngles = new Vector3(rotateX, rotateY, rotateZ);
			particleData[i].direction = q * Vector3.forward;
			particleData[i].speed = 1;
			particleData[i].cos = i / ((float)maxParticle / 10);
			particleData[i].position = particles[i].position;
			particleData[i].size = particles[i].size;
		}

		particleSystem.SetParticles(particles, maxParticle);
		particlePoitionCalculateThread = new Thread(
			new ThreadStart(ParticlePositionCalculate));

		particlePoitionCalculateThread.Start();
	}

	private static readonly float oneFrameSeconds = 0.016f;
	private void ParticlePositionCalculate()
	{
		while (true)
		{
			Vector3 newPosition = new Vector3();
			for (int i = 0; i < particles.Length; i++)
			{
				lock (syncObj)
				{
					Vector3 direction = particleData[i].direction;
					float mul = (particleData[i].speed * (Mathf.Sin(particleData[i].cos)));

					direction.x *= mul * distance;
					direction.z *= mul * distance;
					direction.y *= mul * distance * 5.0f;
					newPosition.x = direction.x;
					newPosition.y = direction.y + particleData[i].vy2;
					newPosition.z = direction.z;
					particles[i].size -= 0.08f * oneFrameSeconds;

					particleData[i].cos += oneFrameSeconds / 10.0f;
					particleData[i].vy += oneFrameSeconds / 20.0f;
					particleData[i].vy2 += particleData[i].vy;
					particleData[i].position = newPosition;
					particleData[i].size = particleData[i].size - 0.08f * oneFrameSeconds;

					if (particleData[i].size < 0.0f)
					{
						particleData[i].size = 0.0f;
						break;
					}
				}
			}

			Thread.Sleep(Mathf.FloorToInt(1000 * oneFrameSeconds));
		}
	}

	void OnApplicationQuit()
	{
		particlePoitionCalculateThread.Abort();
	}

	protected override void Update()
	{
		Vector3 newPosition = new Vector3();
		for (int i = 0; i < particles.Length; i++)
		{
			particles[i].position = particleData[i].position;
			particles[i].size = particleData[i].size;

			if (particles[i].size < 0.0f)
			{
				Destroy(gameObject);
				particlePoitionCalculateThread.Abort();
				break;
			}
		}

		particleSystem.SetParticles(particles, particles.Length);

		transform.Rotate(0.0f, 120f * Time.deltaTime, 0.0f);
	}

	private Object syncObj = new Object();
}