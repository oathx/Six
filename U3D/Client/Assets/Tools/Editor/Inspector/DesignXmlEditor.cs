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
}

