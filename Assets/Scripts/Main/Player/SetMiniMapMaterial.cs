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
		defaultMaterials = thisRenderer.materials;
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
			for (int i = 0; i < thisRenderer.materials.Length; i++)
			{
				thisRenderer.materials[i] = material;
			}
			thisRenderer.material = material;
		}
		else
		{
			thisRenderer.materials = defaultMaterials;
		}
	}
}
