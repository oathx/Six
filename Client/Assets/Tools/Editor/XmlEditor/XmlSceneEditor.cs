using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Xml;

/// <summary>
/// Xml scene setting.
/// </summary>
public class XmlSceneSetting : XmlStruct
{
	/// <summary>
	/// Gets or sets the path.
	/// </summary>
	/// <value>The path.</value>
	[DirectoryField]
	public string			Path
	{ get; set; }

	/// <summary>
	/// Gets or sets the name.
	/// </summary>
	/// <value>The name.</value>
	public string			Name
	{ get; set; }
}

/// <summary>
/// Xml scene struct.
/// </summary>
public class XmlSceneStruct : XmlStruct
{
	public XmlSceneSetting	Setting
	{ get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="XmlSceneStruct"/> class.
	/// </summary>
	public XmlSceneStruct()
	{
		Setting = new XmlSceneSetting ();
	}
}

/// <summary>
/// Xml scene editor.
/// </summary>
public class XmlSceneEditor : XmlEditorWindow<XmlSceneStruct>
{
	[MenuItem("Window/Xml Editor/Scene window")]
	public static void Create()
	{
		XmlSceneEditor window = EditorWindow.GetWindow<XmlSceneEditor> ();
		if (window)
		{
			window.Show();
			window.Focus();
		}
	}
}
