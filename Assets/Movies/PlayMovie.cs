using UnityEngine;

public class PlayMovie : MonoBehaviour {
	[SerializeField]
	bool isLoop = true;
	// Use this for initialization
	void Start () {
		MovieTexture movie = GetComponent<Renderer>().material.mainTexture as MovieTexture;
		if (movie == null)
			return;

		movie.loop = isLoop;
		movie.Play();
	}
}
