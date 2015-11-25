using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using System.Reflection;

/// <summary>
/// Xml scene struct.
/// </summary>
public class XmlSqlLiteStruct : XmlStruct
{
	[DirectoryField]
	public string			SqlExternTool
	{ get; set; }

	/// <summary>
	/// Gets or sets the name of the sql.
	/// </summary>
	/// <value>The name of the sql.</value>
	public string			SqlName
	{ get; set; }

	/// <summary>
	/// Gets or sets the sql path.
	/// </summary>
	/// <value>The sql path.</value>
	[DirectoryField]
	public string			SqlPath
	{ get; set; }

	/// <summary>
	/// Gets or sets the new sql.
	/// </summary>
	/// <value>The new sql.</value>
	[ButtonField("NewSql")]
	public ButtonCallback	NewSqlCallback
	{ get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="XmlSceneStruct"/> class.
	/// </summary>
	public XmlSqlLiteStruct()
	{
		NewSqlCallback = new ButtonCallback (OnNewSqlDatabase);
	}

	/// <summary>
	/// Raises the new sql database event.
	/// </summary>
	public void 			OnNewSqlDatabase(PropertyInfo p, object target)
	{

	}
}



