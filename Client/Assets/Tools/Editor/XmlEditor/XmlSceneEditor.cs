using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Xml;

/// <summary>
/// Xml scene editor.
/// </summary>
public class XmlSceneEditor : XmlEditorWindow<XmlSceneStruct>
{
	[MenuItem("Window/Custom/Scene Editor")]
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
