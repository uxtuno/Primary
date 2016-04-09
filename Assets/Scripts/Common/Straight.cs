using UnityEngine;
using System.Collections;

public class Straight :MyMonoBehaviour {
	//[SerializeField]
	//private float speed = 5.0f;
	[SerializeField]
	private GameObject prefab = null;

	// Use this for initialization
	void Start () {
		rigidbody.AddForce(transform.forward * 100);
	}

	protected override void Update()
	{
		base.Update();
		rigidbody.AddForce(transform.forward * 100);

	}

	void OnCollisionEnter(Collision other)
	{
		if(other.transform.tag == Tags.Finish)
		{
			other.transform.GetComponent<Collider>().isTrigger = true;
		}

		foreach(Transform child in other.transform)
		{
			child.GetComponent<Rigidbody>().isKinematic = false;
		}

		Instantiate(prefab, other.contacts[0].point, Quaternion.identity);
	}
}
