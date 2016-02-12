//C# Example
using System;
using UnityEditor;
using UnityEditorInternal;
using System.Text;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

[InitializeOnLoad]
public class StageSelector : EditorWindow
{
	class ReadScene : IComparable
	{
		public string name;
		public string path;

		public int CompareTo(object obj)
		{
			return name.CompareTo((obj as ReadScene).name);
		}
	}

	private static List<ReadScene> stages = new List<ReadScene>();
	private Vector2 scrollPos = Vector2.zero;

	// Add menu item named "My Window" to the Window menu
    [MenuItem("Window/StageSelector")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(StageSelector));
	}

	static StageSelector()
	{
		AddStage("TutorialStage", "Assets/Scenes/{0}.unity");
		AddStage("Stage", "Assets/Scenes/{0}.unity");
		AddStage("K.R.EasyStage", "Assets/Scenes/KRStage/{0}.unity");
		AddStage("K.R.MiddleStage", "Assets/Scenes/KRStage/{0}.unity");
		AddStage("K.R.HardStage", "Assets/Scenes/KRStage/{0}.unity");
		AddStage("N.EasyStage", "Assets/Scenes/NSStage/{0}.unity");
		AddStage("N.MiddleStage", "Assets/Scenes/NSStage/{0}.unity");
		AddStage("N.HardStage", "Assets/Scenes/NSStage/{0}.unity");
		AddStage("UXStage", "Assets/Scenes/UXStage/{0}.unity");
	}

	private static void AddStage(string keyName, string path)
	{
		int count = stages.Count;
		for (int i = 0; i < EditorBuildSettings.scenes.Count(); i++)
		{
			string sceneName = Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[i].path);
			if (sceneName.Substring(0, sceneName.Length - 2) == keyName)
			{
				ReadScene readScene = new ReadScene();
				readScene.name = sceneName;
				readScene.path = path;
				stages.Add(readScene);
			}
		}
		stages.Sort(count, stages.Count - count, null);
	}

	void OnGUI()
	{
		GUILayout.Label(Path.GetFileNameWithoutExtension(EditorApplication.currentScene), EditorStyles.boldLabel);

		if (GUILayout.Button("前のステージ"))
		{
			string currentScene = Path.GetFileNameWithoutExtension(EditorApplication.currentScene);
			int currentStageIndex = 0;

			foreach(ReadScene readScene in stages)
			{
				if(readScene.name == currentScene)
				{
					break;
				}
				else
				{
					currentStageIndex++;
				}
			}

			if (currentStageIndex > 0)
			{
				OpenScene(stages[--currentStageIndex].name, stages[--currentStageIndex].path);
			}
		}

		if (GUILayout.Button("次のステージ"))
		{
			string currentScene = Path.GetFileNameWithoutExtension(EditorApplication.currentScene);
			int currentStageIndex = 0;

			foreach (ReadScene readScene in stages)
			{
				if (readScene.name == currentScene)
				{
					break;
				}
				else
				{
					currentStageIndex++;
				}
			} 
			if (currentStageIndex < stages.Count() - 1)
			{
				OpenScene(stages[++currentStageIndex].name, stages[++currentStageIndex].path);
			}
		}

		if (GUILayout.Button("リロード"))
		{
			string currentScene = Path.GetFileNameWithoutExtension(EditorApplication.currentScene);
			int currentStageIndex = 0;

			foreach (ReadScene readScene in stages)
			{
				if (readScene.name == currentScene)
				{
					break;
				}
				else
				{
					currentStageIndex++;
				}
			}
			OpenScene(stages[currentStageIndex].name, stages[currentStageIndex].path);
		}

		EditorGUILayout.Separator();

		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUI.skin.box);
		{
			foreach (ReadScene scene in stages)
			{
				if (GUILayout.Button(scene.name))
				{
					OpenScene(scene.name, scene.path);
				}
			}
		}

		EditorGUILayout.EndScrollView();
	}

	private static void OpenScene(string scene, string path)
	{
		if (EditorApplication.isPlaying)
		{
			return;
		}

		if(EditorApplication.SaveCurrentSceneIfUserWantsTo())
		{
			Debug.Log(scene);
			EditorApplication.OpenScene(string.Format(path, scene));
		}
	}
}
