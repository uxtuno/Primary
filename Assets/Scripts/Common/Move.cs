using UnityEngine;
using System.Collections;

public class Move : MonoBehaviour {

	public enum Axis
	{
		forward,
		up,
		right,
	}

	[SerializeField]
	private Axis direction = Axis.forward;
	[SerializeField]
	private float speed = 1.0f;
	Vector3 directionVec;

	void Start()
	{
		switch (direction)
		{
			case Axis.forward:
				directionVec = Vector3.forward;
				break;
			case Axis.up:
				directionVec = Vector3.up;
				break;
			case Axis.right:
				directionVec = Vector3.right;
				break;
		}
	}
	
	void Update () {
		transform.Translate(directionVec * Time.deltaTime * speed);
	}
}
