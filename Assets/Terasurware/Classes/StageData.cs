using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageData : ScriptableObject
{	
	public List<Param> param = new List<Param> ();

	[System.SerializableAttribute]
	public class Param
	{
		
		public string Name;
	}
}