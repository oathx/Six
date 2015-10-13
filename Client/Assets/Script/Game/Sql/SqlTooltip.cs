using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

/// <summary>
/// Sql shape.
/// </summary>
public class SqlTooltip : ISqlPackage
{
	public int 		ID
	{get; set;}

	public string	Text
	{get; set;}

	/// <summary>
	/// Decode the specified sdr.
	/// </summary>
	/// <param name="sdr">Sdr.</param>
	public override void 	Decode (SqliteDataReader sdr)
	{
		ID			= System.Convert.ToInt32(sdr["ID"]);
		Text 		= System.Convert.ToString(sdr["Text"]);
	}
}
