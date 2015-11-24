using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Xml;

/// <summary>
/// Xml scene struct.
/// </summary>
public class XmlVersionStruct : XmlStruct
{
	/// <summary>
	/// Initializes a new instance of the <see cref="XmlSceneStruct"/> class.
	/// </summary>
	public XmlVersionStruct()
	{

	}
}

/// <summary>
/// Xml scene editor.
/// </summary>
public class XmlVersionEditor : XmlEditorWindow<XmlVersionStruct>
{
	[MenuItem("Window/Xml Editor/Version window")]
	public static void Create()
	{
		XmlVersionEditor window = EditorWindow.GetWindow<XmlVersionEditor> ();
		if (window)
		{
			window.Show();

			// active the version window
			window.Focus();
		}
	}
}

