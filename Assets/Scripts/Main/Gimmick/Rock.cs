using UnityEngine;
using System.Collections;

public class Rock : ColorObjectBase {
	protected override void Update()
	{
		Debug.Log(renderer.material.color.a);
		renderer.material.color = objectColor.color;
	}

	private bool _Frag = true;
	public bool frag
	{
		get { return _Frag; }
		set { _Frag = value; }
	}

	protected override void OnDisappearance()
	{
		_Frag = false;
	}
}
