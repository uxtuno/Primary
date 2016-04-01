using UnityEngine;
using System.Collections;

public class PlayMovie : MonoBehaviour {
	[SerializeField]
	bool isLoop = true;
	// Use this for initialization
	void Start () {
		MovieTexture movie = GetComponent<Renderer>().material.mainTexture as MovieTexture;
		movie.loop = isLoop;
		movie.Play();
	}
}
