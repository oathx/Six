using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;

/// <summary>
/// Xml scene struct.
/// </summary>
public class XmlJobStruct : XmlStruct
{
	public List<SqlJob>	
		Job = new List<SqlJob>();
	
	/// <summary>
	/// Initializes a new instance of the <see cref="XmlSceneStruct"/> class.
	/// </summary>
	public XmlJobStruct()
	{
		GameSqlLite.GetSingleton ().OpenDB (WUrl.SqlitePathWin32);
		Job = GameSqlLite.GetSingleton ().QueryTable<SqlJob> ();
		GameSqlLite.GetSingleton ().CloseDB ();
	}
}


