﻿using UnityEngine;
using System.Collections;

// Applies an explosion force to all nearby rigidbodies
public class Explosion : MonoBehaviour
{
	public float radius = 5.0F;
	public float power = 10.0F;

	void Start()
	{
		Vector3 explosionPos = transform.position;
		Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
		Debug.Log(colliders.Length);
		foreach (Collider hit in colliders)
		{
			Rigidbody rb = hit.GetComponent<Rigidbody>();

			if (rb != null)
				rb.AddExplosionForce(power, explosionPos, radius, 3.0F);

		}
	}

	void OnCollisionEnter(Collision other)
	{
		other.transform.GetComponent<Rigidbody>().isKinematic = false;
		other.gameObject.AddComponent<Explosion>();
	}
}