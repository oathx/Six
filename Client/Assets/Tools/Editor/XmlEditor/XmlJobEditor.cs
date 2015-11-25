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
public class XmlJobEditor : XmlEditorWindow<XmlJobStruct>
{
	[MenuItem("Window/Custom/Job Editor")]
	public static void Create()
	{
		XmlJobEditor window = EditorWindow.GetWindow<XmlJobEditor> ();
		if (window)
		{
			window.Show();
			window.Focus();
		}
	}
}

