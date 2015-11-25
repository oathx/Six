using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Scene editor.
/// </summary>
public class SceneEditor : XmlEditorWindow<SceneXmlStruct>
{
	[MenuItem("Window/Custom/Scene Editor")]
	public static void CreateSceneEditor()
	{
		SceneEditor window = EditorWindow.GetWindow<SceneEditor>();
		if (window)
		{
			window.Show();
			window.Focus();
		}
	}

	/// <summary>
	/// Raises the disable event.
	/// </summary>
	public override void OnDisable()
	{
		for(int i=0; i<Instance.Monster.Count; i++)
		{
			if (!Instance.Monster[i].Shape)
			{
				Instance.Monster.RemoveAt(i);
			}
		}
	}

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	public override void OnEnable ()
	{

	}

	/// <summary>
	/// Raises the hierarchy change event.
	/// </summary>
	public override void OnHierarchyChange()
	{
		for(int i=0; i<Instance.Monster.Count; i++)
		{
			if (!Instance.Monster[i].Shape)
			{
				Instance.Monster.RemoveAt(i);
			}
		}
	}

	/// <summary>
	/// Raises the custom field event.
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="type">Type.</param>
	public override void OnCustomField(object target, PropertyInfo pi)
	{
		if (pi.Name == SceneKeyWord.SqlID.ToString())
		{
			int nValue = (int)pi.GetValue(target, new object[]{});
			nValue = EditorGUILayout.IntPopup(pi.Name, nValue, new List<string>(Instance.Setting.Select.Values).ToArray(), 
			                                  new List<int>(Instance.Setting.Select.Keys).ToArray());
			pi.SetValue(target, nValue, new object[]{});

			try{
				Instance.Update(nValue);
			}
			catch(System.Exception e)
			{
				Debug.LogException(e);
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
				Instance.Path = szFilePath.Substring(szFilePath.LastIndexOf("/") + 1);
				Instance.Name = GetSceneName();
			}
		}
		
		if (GUILayout.Button(KeyWord.Export.ToString()))
		{
			string szName = GetSceneName();
			if (!string.IsNullOrEmpty(szName))
				szName = szName.Replace(KeyWord.Unity.ToString().ToLower(), KeyWord.Xml.ToString());
			
			string szPath = EditorUtility.SaveFilePanel(KeyWord.Save.ToString(), 
			                                            Application.dataPath, szName, KeyWord.Xml.ToString());
			if (!string.IsNullOrEmpty(szPath))
			{
				Instance.Save(szPath);

				Instance.Path = szPath.Substring(szPath.LastIndexOf("/") + 1);
				Instance.Name = szName;
			}
		}

		if (GUILayout.Button(KeyWord.Create.ToString()))
		{
			Instance.Create(); 
		}
	}


	/// <summary>
	/// Gets the name of the scene.
	/// </summary>
	/// <returns>The scene name.</returns>
	public virtual string GetSceneName()
	{
		if (!string.IsNullOrEmpty(EditorApplication.currentScene))
		{
			return EditorApplication.currentScene.Substring(EditorApplication.currentScene.LastIndexOf("/") + 1);
		}

		return string.Empty;
	}
}
