using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// sqlite support
using Mono.Data.Sqlite;
using System.Data;

/// <summary>
/// Sql shape.
/// </summary>
public class SqlMagic : ISqlPackage
{
	public int 				ID
	{ get; set; }

	public string			Name
	{ get; set; }

	public int 				Level
	{ get; set; }

	public int 				Type
	{ get; set; }

	public int 				SelfActionID
	{ get; set; }
	
	public int 				TargetActionID
	{ get; set; }

	/// <summary>
	/// Decode the specified sdr.
	/// </summary>
	/// <param name="sdr">Sdr.</param>
	public override void 	Decode (SqliteDataReader sdr)
	{
		ID 					= System.Convert.ToInt32(sdr ["ID"]);
		Name 				= System.Convert.ToString(sdr ["Name"]);
		Level 				= System.Convert.ToInt32(sdr ["Level"]);
		Type 				= System.Convert.ToInt32(sdr ["Type"]);
		SelfActionID 		= System.Convert.ToInt32(sdr ["SelfActionID"]);
		TargetActionID		= System.Convert.ToInt32(sdr ["TargetActionID"]);

	}

	/// <summary>
	/// Fill the specified row.
	/// </summary>
	/// <param name="row">Row.</param>
	/// <param name="sdr">Sdr.</param>
	public override void 	Fill(DataRow sdr)
	{
		ID 					= System.Convert.ToInt32(sdr ["ID"]);
		Name 				= System.Convert.ToString(sdr ["Name"]);
		Level 				= System.Convert.ToInt32(sdr ["Level"]);
		Type 				= System.Convert.ToInt32(sdr ["Type"]);
		SelfActionID 		= System.Convert.ToInt32(sdr ["SelfActionID"]);
		TargetActionID		= System.Convert.ToInt32(sdr ["TargetActionID"]);
	}
}



