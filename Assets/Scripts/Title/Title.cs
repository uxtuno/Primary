using UnityEngine;
using System.Collections;

public class Title : MonoBehaviour
{
	//[SerializeField]
   // private string[] scenes;
    void Awake()
    {
        //scenes = new string[] { "Player", "TestStage01" };
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneChangeSingleton.instance.LoadLevel("TutorialStage1");
        }
    }
}
