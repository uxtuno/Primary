using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExcelData : ScriptableObject
{
	
	public List<Param> list = new List<Param> ();
	
	[System.SerializableAttribute]
	public class Param
	{
		public string skillName;
		public string skillEffect;
		public int damage;
	}
}

