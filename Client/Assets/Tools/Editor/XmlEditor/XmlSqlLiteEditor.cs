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
public class XmlSqlLiteEditor : XmlEditorWindow<XmlSqlLiteStruct>
{
	[MenuItem("Window/Custom/Design Editor")]
	public static void Create()
	{
		XmlSqlLiteEditor window = EditorWindow.GetWindow<XmlSqlLiteEditor> ();
		if (window)
		{
			window.Show();
			window.Focus();
		}
	}
}

