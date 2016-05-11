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
	private ParticleData[] particleData = null;
	private float distance = 2.0f;
	private Thread particlePoitionCalculateThread = null;
	private const int maxParticle = 200000;

	void Start()
	{
		particleSystem = gameObject.GetComponent<ParticleSystem>();
		particleSystem.maxParticles = maxParticle;
		particles = new ParticleSystem.Particle[maxParticle];
		particleData = new ParticleData[maxParticle];
		particleSystem.Emit(maxParticle);
		particleSystem.GetParticles(particles);

		//new Thread(new ThreadStart(ParticleGenerate)).Start();

		ParticleGenerate();
		particlePoitionCalculateThread = new Thread(
			new ThreadStart(ParticlePositionCalculate));

		particlePoitionCalculateThread.Start();
	}

	private static readonly float oneFrameSeconds = 0.032f;

	private void ParticleGenerate()
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
			particleData[i].position = particles[i].position;
			particleData[i].size = particles[i].size;
		}
	}

	private bool isEnd;
	private void ParticlePositionCalculate()
	{
		isEnd = false;
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

					particleData[i].cos += oneFrameSeconds / 10.0f;
					particleData[i].vy += oneFrameSeconds / 20.0f;
					particleData[i].vy2 += particleData[i].vy;
					particleData[i].position = newPosition;
					particleData[i].size = particleData[i].size - 0.08f * oneFrameSeconds;
					particles[i].position = particleData[i].position;
					particles[i].size = particleData[i].size;
					if (particleData[i].size < 0.0f)
					{
						particleData[i].size = 0.0f;
						break;
					}
				}
			}

			if (particleData[0].size <= 0.0f)
				isEnd = true;

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