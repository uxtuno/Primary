using UnityEngine;
using System.Collections;

/// <summary>
/// 実行時に破棄する
/// </summary>
public class DestoroyOnAwake : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Destroy (gameObject);
	}
}
