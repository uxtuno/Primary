using UnityEngine;
using System.Collections;

public class IrradiationPort : MyMonoBehaviour
{
	private bool _isIrradiated = true;

	public bool isIrradiated
	{
		get
		{
			return _isIrradiated;
		}

		set
		{
			_isIrradiated = value;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.GetComponentInParent<Player>())
		{
			return;
		}

		if(other.GetComponent<ColorObjectBase>() != null)
		{
			isIrradiated = false;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.GetComponentInParent<Player>())
		{
			return;
		}

		if (other.GetComponent<ColorObjectBase>() != null)
		{
			isIrradiated = true;
		}
	}
}
