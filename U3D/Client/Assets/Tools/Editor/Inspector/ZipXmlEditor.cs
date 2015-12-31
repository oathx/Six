using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Xml;
using System;
using LitJson;

public class ZipPackage
{
	[ReadonlyField]
	public string				LastTime
	{ get; set; }
	
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="ZipPackage"/> out zip.
	/// </summary>
	/// <value><c>true</c> if out zip; otherwise, <c>false</c>.</value>
	public bool					NoBuild
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the input path.
	/// </summary>
	/// <value>The input path.</value>
	public DirectoryStruct		InputPath
	{ get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="ZipPackage"/> class.
	/// </summary>
	public ZipPackage()
	{
		InputPath = new DirectoryStruct ();

	}
}

/// <summary>
/// Zip xml struct.
/// </summary>
public class ZipXmlStruct : XmlStruct
{
	/// <summary>
	/// Gets or sets the work space.
	/// </summary>
	/// <value>The work space.</value>
	[ReadonlyField]
	public string				WorkSpace
	{ get; set; }

	/// <summary>
	/// Gets or sets the middle sapce.
	/// </summary>
	/// <value>The middle sapce.</value>
	[ReadonlyField]
	public string 				MidSapce
	{ get; set; }

	/// <summary>
	/// Gets or sets the out space.
	/// </summary>
	/// <value>The out space.</value>
	[ReadonlyField]
	public string				OutSpace
	{ get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="ZipXmlStruct"/> generate version.
	/// </summary>
	/// <value><c>true</c> if generate version; otherwise, <c>false</c>.</value>
	public bool					GenerateVersion
	{ get; set; }

	/// <summary>
	/// Gets or sets the build clicked.
	/// </summary>
	/// <value>The build clicked.</value>
	[ButtonField("Build")]
	public ButtonClicked		BuildClicked
	{ get; set; }

	/// <summary>
	/// Gets or sets the delete clicked.
	/// </summary>
	/// <value>The delete clicked.</value>
	[ButtonField("Delete")]
	public ButtonClicked		DeleteClicked
	{ get; set; }

	/// <summary>
	/// Gets or sets the create clicked.
	/// </summary>
	/// <value>The create clicked.</value>
	[ButtonField("Create")]
	public ButtonClicked		CreateClicked
	{ get; set; }

	/// <summary>
	/// The path.
	/// </summary>
	public List<ZipPackage>			
		Package = new List<ZipPackage> ();

	/// <summary>
	/// Initializes a new instance of the <see cref="ZipXmlStruct"/> class.
	/// </summary>
	public ZipXmlStruct()
	{
		WorkSpace 		= Application.dataPath;
		MidSapce		= XmlEditorHelper.MidPath;
		OutSpace 		= Application.streamingAssetsPath;

		CreateClicked 	= new ButtonClicked (OnCreateClicked);
		BuildClicked	= new ButtonClicked (OnBuildClicked);
		DeleteClicked	= new ButtonClicked (OnDeleteClicked);
	}

	/// <summary>
	/// Raises the create clicked event.
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="p">P.</param>
	public void 				OnCreateClicked(object target, PropertyInfo p)
	{
		Package.Add (new ZipPackage ());
	}

	/// <summary>
	/// Raises the build clicked event.
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="p">P.</param>
	public void 				OnBuildClicked(object target, PropertyInfo p)
	{
		foreach (ZipPackage zip in Package) 
		{
			if (!zip.NoBuild && !string.IsNullOrEmpty(zip.InputPath.FullPath))
			{
				XmlEditorHelper.BuildAssetBundles(
					zip.InputPath.FullPath, zip.InputPath.FileType);

				zip.LastTime = System.DateTime.Now.ToString();
			}
		}

		if (GenerateVersion)
		{
			if (!Directory.Exists(Application.streamingAssetsPath))
				Directory.CreateDirectory(Application.streamingAssetsPath);
			
			string szTimeString = System.DateTime.Now.ToString ().Replace("/", string.Empty).Replace(" ", string.Empty).Replace(":", string.Empty);
			XmlEditorHelper.CreateZipFile (XmlEditorHelper.MidPath, 
			                               string.Format ("{0}/{1}.zip", Application.streamingAssetsPath, szTimeString), "*", SearchFileType.unity3d);
		}
	}

	/// <summary>
	/// Raises the build clicked event.
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="p">P.</param>
	public void 				OnDeleteClicked(object target, PropertyInfo p)
	{
		for(int idx=0; idx<Package.Count; idx++)
		{
			if (Package[idx].NoBuild)
				Package.RemoveAt(idx);
		}
	}
}

/// <summary>
/// Zip xml editor.
/// </summary>
public class ZipXmlEditor : XmlEditorWindow<ZipXmlStruct>
{
	[MenuItem("Custom/Zip Window")]
	public static void CreateXmlEditor()
	{
		ZipXmlEditor window = EditorWindow.GetWindow<ZipXmlEditor> ();
		if (window) 
		{
			window.Show();
			window.Focus();
		}
	}
}
