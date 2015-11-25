using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Xml;

/// <summary>
/// Zip editor.
/// </summary>
public class ZipEditor : XmlEditorWindow<ZipXmlStruct>
{
	[MenuItem("Window/Custom/Zip Editor")]
	public static void CreateSceneEditor()
	{
		ZipEditor window = EditorWindow.GetWindow<ZipEditor>();
		if (window)
		{
			window.Show();
			window.Focus();
		}
	}

	/// <summary>
	/// Raises the select directory change event.
	/// </summary>
	/// <param name="szDirectory">Size directory.</param>
	/// <param name="target">Target.</param>
	/// <param name="p">P.</param>
	public override void OnSelectDirectoryChange(string szDirectory, PropertyInfo p, object target)
	{
		FieldInfo field = target.GetType().GetField(ZipWord.FileList.ToString());
		if (field != default(FieldInfo))
		{
			PropertyInfo pi = GetProperty(target.GetType(), ZipWord.InputFileType.ToString());
			if (pi != default(PropertyInfo))
			{
				IList aryInputFile = (IList)field.GetValue(target);

				object result = pi.GetValue(
					target, new object[]{}
				);
				
				string[] aryFile = System.IO.Directory.GetFiles(szDirectory, 
				                                                string.Format("*.{0}", result.ToString()), SearchOption.AllDirectories);
				foreach(string file in aryFile)
				{
					string szAssetPath = file.Substring(Application.dataPath.Length - 6);
					
					using(FileStream stream = File.OpenRead(file))
					{
						aryInputFile.Add(
							new InputFile(szAssetPath, file.Length)
							);
						
						stream.Close();
					}
				}
			}
		}
		
	}

	/// <summary>
	/// Raises the custom GUI previous event.
	/// </summary>
	public override void OnCustomGUIPrev()
	{
		if (GUILayout.Button(KeyWord.Import.ToString()))
		{
			string szFilePath = EditorUtility.OpenFilePanel(KeyWord.Export.ToString(), Application.dataPath, KeyWord.Xml.ToString());
			if (!string.IsNullOrEmpty(szFilePath))
			{
				Instance.Load(szFilePath);
			}
		}
		
		if (GUILayout.Button(KeyWord.Export.ToString()))
		{
			string szPath = EditorUtility.SaveFilePanel(KeyWord.Save.ToString(), 
			                                            Application.dataPath, GetType().Name, KeyWord.Xml.ToString());
			if (!string.IsNullOrEmpty(szPath))
			{
				Instance.Save(szPath);
			}
		}
		
		if (GUILayout.Button(KeyWord.Create.ToString()))
		{
			Instance.Create(); 
		}
	}
}