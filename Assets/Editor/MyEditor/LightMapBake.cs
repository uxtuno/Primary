using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class LightMapBake : MonoBehaviour
{
	private static int currentSceneIndex = 0;
	private static StageData stageData;

	[MenuItem("Utility/AllStageLightMapBake")]
	static void BakeScenes()
	{
		if (Lightmapping.isRunning)
			return;

		currentSceneIndex = 0;
		stageData = Resources.Load<StageData>("StageData/StageData");

		Bake();
	}

	[MenuItem("Utility/BaleCancel")]
	static void BekeCancel()
	{
		if(!Lightmapping.isRunning)
			return;

		Lightmapping.Cancel();
	}

	private static void Bake()
	{
		if (currentSceneIndex >= stageData.param.Count)
			return;

		EditorApplication.SaveScene();
		string path = GetBakeScenePath(currentSceneIndex, stageData);
		EditorApplication.OpenScene(path);
		Lightmapping.BakeAsync();
		currentSceneIndex++;

		Lightmapping.completed = Bake;
	}

	/// <summary>
	/// StageDataの現在のインデックスに当たるシーン名が
	/// buildsettingsに登録されていればそのシーンpathを返す
	/// </summary>
	/// <param name="index">index</param>
	/// <param name="stageData">ステージ順を表すSirializedObject</param>
	/// <returns></returns>
	private static string GetBakeScenePath(int index, StageData stageData)
	{
		for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
		{
			// 
			string name = Path.GetFileNameWithoutExtension(EditorBuildSettings.scenes[i].path);
			if (stageData.param[index].Name == name)
			{
				return EditorBuildSettings.scenes[i].path;
			}
		}

		return string.Empty;
	} 

}
