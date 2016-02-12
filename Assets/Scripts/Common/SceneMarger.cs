using UnityEngine;
using System.Collections;

public class SceneMarger : MonoBehaviour
{
    [SerializeField]
    private string[] mergeScenes = null;

    private static bool semaphore;

    void Awake()
    {
        if (semaphore)
        {
            return;
        }

        semaphore = true;

        foreach (string scene in mergeScenes)
        {
            Application.LoadLevelAdditive(scene);
        }
    }

}
