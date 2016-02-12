using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class ExcelProcessor : AssetPostprocessor
{
	private static readonly string filePath = "Assets/Terasurware/ExcelData.xls";
	private static readonly string exportPath = "Assets/Terasurware/ExcelData.asset";
	
	// 1
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			
			if (!filePath.Equals (asset))
				continue;
			
			// 2
			
			ExcelData data = (ExcelData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(ExcelData));

			if (data == null) {
				data = ScriptableObject.CreateInstance<ExcelData> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				data.hideFlags = HideFlags.NotEditable;
			}
			
			// 3
			
			data.list.Clear ();
			
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read)) {
				IWorkbook book = new HSSFWorkbook (stream);
				
				ISheet sheet = book.GetSheetAt (0);
				Debug.Log (sheet.SheetName);
				
				for (int i=1; i< sheet.LastRowNum; i++) {
					
					IRow row = sheet.GetRow (i);

					ExcelData.Param p = new ExcelData.Param ();
					p.skillName = row.GetCell (0).StringCellValue;
					p.skillEffect = row.GetCell (1).StringCellValue;
					p.damage = (int)row.GetCell (2).NumericCellValue;
					
					data.list.Add (p);
				}
			}
			
			// 4

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}