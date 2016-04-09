//C# Example
using System;
using UnityEditor;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
	private static List<ReadScene> otherScenes = new List<ReadScene>();
	private Vector2 stageListScrollPos = Vector2.zero;
	private Vector2 otherSceneListScrollPos = Vector2.zero;
	private static StageData stageData;

	// Add menu item named "My Window" to the Window menu
	[MenuItem("Window/StageSelector")]
	public static void ShowWindow()
	{
		//Show existing window instance. If one doesn't exist, make one.
		EditorWindow.GetWindow(typeof(StageSelector));
	}

	static StageSelector()
	{

		//AddStage("TutorialStage", "Assets/Scenes/{0}.unity");
		//AddStage("Stage", "Assets/Scenes/{0}.unity");
		//AddStage("K.R.EasyStage", "Assets/Scenes/KRStage/{0}.unity");
		//AddStage("K.R.MiddleStage", "Assets/Scenes/KRStage/{0}.unity");
		//AddStage("K.R.HardStage", "Assets/Scenes/KRStage/{0}.unity");
		//AddStage("N.EasyStage", "Assets/Scenes/NSStage/{0}.unity");
		//AddStage("N.MiddleStage", "Assets/Scenes/NSStage/{0}.unity");
		//AddStage("N.HardStage", "Assets/Scenes/NSStage/{0}.unity");
		//AddStage("UXStage", "Assets/Scenes/UXStage/{0}.unity");

		stageData = null;

		foreach (var scene in EditorBuildSettings.scenes)
		{
			ReadScene readScene = new ReadScene();
			readScene.name = Path.GetFileNameWithoutExtension(scene.path);
			readScene.path = scene.path;
			stages.Add(readScene);
		}
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

	void Start()
	{
		stageData = null;
	}

	void Update()
	{
		if (stageData == null || stageData.param.Count == 0)
			LoadStageData();
	}

	private void LoadStageData()
	{
		stageData = Resources.Load<StageData>("StageData/StageData");

		var allScene = stages
			.OrderBy(stage =>
			{
				int index = -1;
				// StageData上の位置を求める
				foreach (var param in stageData.param.Where(param => stage.name == param.Name))
				{
					index = stageData.param.IndexOf(param);
				}

				Debug.Log(index);
				return index >= 0 ? index : stages.Count;
			})
			.ToList();

		stages = allScene.Take(stageData.param.Count).ToList();
		otherScenes = allScene.Skip(stageData.param.Count).ToList();
	}

	void OnGUI()
	{
		GUILayout.Label(Path.GetFileNameWithoutExtension(EditorApplication.currentScene), EditorStyles.boldLabel);

		if (GUILayout.Button("前のステージ"))
		{
			string currentScene = Path.GetFileNameWithoutExtension(EditorApplication.currentScene);
			int currentStageIndex = 0;
			foreach (var readScene in stages)
			{
				if (readScene.name == currentScene)
					break;

				currentStageIndex++;
			}

			if (currentStageIndex > 0)
			{
				OpenScene(stages[currentStageIndex - 1].name, stages[currentStageIndex - 1].path);
			}
		}

		if (GUILayout.Button("次のステージ"))
		{
			string currentScene = Path.GetFileNameWithoutExtension(EditorApplication.currentScene);

			int currentStageIndex = 0;
			foreach (var readScene in stages)
			{
				if (readScene.name == currentScene)
					break;

				currentStageIndex++;
			}

			if (currentStageIndex < stages.Count - 1)
			{
				OpenScene(stages[currentStageIndex + 1].name, stages[currentStageIndex + 1].path);
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

		EditorGUILayout.LabelField("Stage List");

		EditorGUILayout.BeginHorizontal();
		{
			stageListScrollPos = EditorGUILayout.BeginScrollView(stageListScrollPos, GUI.skin.box);
			{
				// インデント！ 波動拳！！
				foreach (var scene in stages)
				{
					bool isDisable = scene.path == EditorApplication.currentScene;

					EditorGUI.BeginDisabledGroup(isDisable);
					{
						if (GUILayout.Button(scene.name))
						{
							OpenScene(scene.name, scene.path);
						}
					}
					EditorGUI.EndDisabledGroup();
				}

				EditorGUILayout.Separator();
				EditorGUILayout.LabelField("Other Scenes");

				foreach (var scene in otherScenes)
				{
					if (GUILayout.Button(scene.name))
					{
						OpenScene(scene.name, scene.path);
					}
				}
			}
			EditorGUILayout.EndScrollView();
		}
		EditorGUILayout.EndHorizontal();
	}

	private static void OpenScene(string scene, string path)
	{
		if (EditorApplication.isPlaying)
		{
			return;
		}

		if (EditorApplication.SaveCurrentSceneIfUserWantsTo())
		{
			Debug.Log(scene);
			EditorApplication.OpenScene(string.Format(path, scene));
		}
	}
}
