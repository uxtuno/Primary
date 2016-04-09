using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class SmosePoint : MyMonoBehaviour
{
	private Transform controller;
	private int thisIndex = 0;
	private Vector3 oldPrevPosition;
	private Vector3 oldNextPosition;
	void Start()
	{
		controller = transform.parent;
		for (int i = 0; i < controller.childCount; i++)
		{
			if (controller.GetChild(i) == transform)
			{
				thisIndex = i;
			}
		}
	}

	protected override void Update()
	{
		if (Application.isPlaying || (thisIndex - 1 < 0) || (thisIndex + 1 > controller.childCount))
		{
			return;
		}

		Transform prev = controller.GetChild(thisIndex - 1);
		Transform next = controller.GetChild(thisIndex + 1);

		if (oldPrevPosition != prev.position)
		{
			Vector3 direction = (prev.position - transform.position);
			next.position = (-direction.normalized * (next.position - transform.position).magnitude) + transform.position;
		}
		else if (oldNextPosition != next.position)
		{
			Vector3 direction = (next.position - transform.position);
			prev.position = (-direction.normalized * (prev.position - transform.position).magnitude) + transform.position;
		}

		oldPrevPosition = prev.position;
		oldNextPosition = next.position;
	}
}
