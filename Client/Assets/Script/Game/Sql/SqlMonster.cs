using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// sqlite support
using Mono.Data.Sqlite;
using System.Data;

/// <summary>
/// Sql shape.
/// </summary>
public class SqlMonster : ISqlPackage
{
	/// <summary>
	/// Gets or sets the I.
	/// </summary>
	/// <value>The I.</value>
	public int 				ID
	{ get; set; }

	/// <summary>
	/// Gets or sets the shape I.
	/// </summary>
	/// <value>The shape I.</value>
	public int 				ShapeID
	{ get; set; }

	/// <summary>
	/// Decode the specified sdr.
	/// </summary>
	/// <param name="sdr">Sdr.</param>
	public override void 	Decode (SqliteDataReader sdr)
	{

	}

	/// <summary>
	/// Encode the specified row.
	/// </summary>
	/// <param name="row">Row.</param>
	public override void 	Encode (DataRow row)
	{

	}
}
