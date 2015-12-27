using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Xml;
using System;
using LitJson;
using System.Data;
using Mono.Data.Sqlite;
using System.Text;

/// <summary>
/// Zip xml struct.
/// </summary>
public class DesignXmlStruct : XmlStruct
{

}

/// <summary>
/// Zip xml editor.
/// </summary>
public class DesignXmlEditor : XmlEditorWindow<DesignXmlStruct>
{
	[MenuItem("Custom/Design Window")]
	public static void CreateXmlEditor()
	{
		DesignXmlEditor window = EditorWindow.GetWindow<DesignXmlEditor> ();
		if (window) 
		{
			window.Show();
			window.Focus();
		}
	}

	/// <summary>
	/// Converts the DD.
	/// </summary>
	[MenuItem("Convert/DDS")]
	public static void ConvertDDS()
	{
		Material mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Environment/Building/Materials/environment_Building_caopeng_001_01.mat");
		Debug.Log (mat);
		string szTextureFile = AssetDatabase.GetAssetPath (mat.mainTexture);
		if (szTextureFile.Contains(".dds"))
		{
			Texture t = AssetDatabase.LoadAssetAtPath<Texture>(szTextureFile.Replace(".dds", ".png"));
			if (t)
			{
				mat.mainTexture = t;
				AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(mat));
			}
		}
	}
}

