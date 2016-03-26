using UnityEngine;
using System.Collections.Generic;

public class SetMiniMapMaterial : MyMonoBehaviour
{
	private Camera miniMapCamera = null;
	[SerializeField]
	private Material material = null;
	private Material[] defaultMaterials;
	private Renderer thisRenderer;

	protected override void Awake()
	{
		thisRenderer = GetComponent<Renderer>();
		defaultMaterials = thisRenderer.sharedMaterials;
		var go = GameObject.FindGameObjectWithTag(Tags.MiniMapCamera);
		if(go)
		{
			miniMapCamera = go.GetComponent<Camera>();
		}
	}

	void OnWillRenderObject()
	{
		if (Camera.current == miniMapCamera)
		{
			thisRenderer.sharedMaterial = material;
		}
		else
		{
			thisRenderer.sharedMaterials = defaultMaterials;
		}
	}
}
