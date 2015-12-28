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
					string szAssetPath = AssetDatabase.GetAssetPath (mat.mainTexture).ToLower();

					int nStart 	= 0;
					int nEnd	= szAssetPath.LastIndexOf('.') + 1;

					string szExtName = szAssetPath.Substring(nEnd);
					if (szExtName == SearchFileType.dds.ToString())
					{
						string szPngPath = string.Format("{0}{1}", szAssetPath.Substring(0, nEnd), SearchFileType.png.ToString());
						if (!File.Exists(szPngPath))
						{
							Debug.LogError(szPngPath);
						}
						else
						{
							Texture pngTexture = AssetDatabase.LoadAssetAtPath<Texture>(szPngPath);
							if (!pngTexture)
								throw new System.NullReferenceException();

							mat.mainTexture = pngTexture;

							AssetDatabase.ImportAsset(path);
						}
					}
				}
			}
		}
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


