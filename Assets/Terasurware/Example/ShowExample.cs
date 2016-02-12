using UnityEngine;
using System.Collections;

public class ShowExample : MonoBehaviour
{
	[SerializeField]
	ExcelData data = null;
	
	void OnGUI ()
	{
		if (data == null)
			return;
		
		foreach (ExcelData.Param p in data.list) {
			string msg = string.Format ("skill name: {0}, effect: {1}",
					p.skillName, p.skillEffect);
			GUILayout.Label (msg);
		}
	}
}
