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
public class DDSXmlStruct : XmlStruct
{
	/// <summary>
	/// Gets or sets the convert.
	/// </summary>
	/// <value>The convert.</value>
	[ButtonField("Convert To PNG")]
	public ButtonClicked	ConvertClicked
	{ get; set; }

	/// <summary>
	/// Gets or sets the clear clicked.
	/// </summary>
	/// <value>The clear clicked.</value>
	[ButtonField("Clear DDS Texture")]
	public ButtonClicked	ClearClicked
	{ get; set; }

	/// <summary>
	/// Gets or sets the input path.
	/// </summary>
	/// <value>The input path.</value>
	public DirectoryStruct	InputPath
	{ get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="DDSXmlStruct"/> class.
	/// </summary>
	public DDSXmlStruct()
	{
		InputPath 		= new DirectoryStruct();
		ConvertClicked 	= new ButtonClicked (OnConvertClicked);
		ClearClicked 	= new ButtonClicked (OnClearClicked);
	}
	
	/// <summary>
	/// Raises the convert clicked event.
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="pi">Pi.</param>
	public void 			OnConvertClicked(object target, PropertyInfo p)
	{
		foreach(string path in InputPath.FileArray)
		{
			if (!string.IsNullOrEmpty(path))
			{
				Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
				if (mat)
				{
					string szAssetPath = AssetDatabase.GetAssetPath (mat.mainTexture);
					if (szAssetPath.Contains(".dds"))
					{
						string szPngPath = szAssetPath.Replace(".dds", ".png");
						if (!File.Exists(szPngPath))
							Debug.LogError("Can't find png " + szAssetPath);

						Texture pngTexture = AssetDatabase.LoadAssetAtPath<Texture>(szPngPath);
						if (pngTexture)
						{
							mat.mainTexture = pngTexture;
							AssetDatabase.ImportAsset(path);
						}
					}
				}
			}
		}

		Terrain t;
	}

	/// <summary>
	/// Raises the convert clicked event.
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="p">P.</param>
	public void 			OnClearClicked(object target, PropertyInfo p)
	{
		string[] aryFile = System.IO.Directory.GetFiles (Application.dataPath, "*.dds", SearchOption.AllDirectories);
		foreach(string file in aryFile)
		{
			File.Delete(file);
			File.Delete(file + ".meta");
		}
	}
}

/// <summary>
/// Zip xml editor.
/// </summary>
public class DDSXmlEditor : XmlEditorWindow<DDSXmlStruct>
{
	[MenuItem("Custom/DDS Convert Window")]
	public static void CreateXmlEditor()
	{
		DDSXmlEditor window = EditorWindow.GetWindow<DDSXmlEditor> ();
		if (window) 
		{
			window.Show();
			window.Focus();
		}
	}
}


