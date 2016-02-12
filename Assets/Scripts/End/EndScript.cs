using UnityEngine;
using System.Collections;

public class EndScript : MonoBehaviour
{
	void Update()
	{
		bool isSceneChange = Input.GetMouseButtonDown(0) ||	Input.GetKeyDown(KeyCode.Return);

		if(isSceneChange)
		{
			SceneChangeSingleton.instance.LoadLevel(Scenes.Menu.name);
		}
	}
}
