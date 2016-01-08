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

	public int 				SelfActionID
	{ get; set; }

	public int 				SelfBuffID
	{ get; set; }
	
	public int 				TargetActionID
	{ get; set; }

	public int 				TargetBuffID
	{ get; set; }

	public float 			AttackDistance
	{ get; set; }

	/// <summary>
	/// Decode the specified sdr.
	/// </summary>
	/// <param name="sdr">Sdr.</param>
	public override void 	Decode (SqliteDataReader sdr)
	{
		ID 					= System.Convert.ToInt32(sdr ["ID"]);
		Name 				= System.Convert.ToString(sdr ["Name"]);
		SelfBuffID 			= System.Convert.ToInt32(sdr ["SelfBuffID"]);
		TargetBuffID 		= System.Convert.ToInt32(sdr ["TargetBuffID"]);
		SelfActionID 		= System.Convert.ToInt32(sdr ["SelfActionID"]);
		TargetActionID		= System.Convert.ToInt32(sdr ["TargetActionID"]);
		AttackDistance		= System.Convert.ToSingle(sdr ["AttackDistance"]);
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
		SelfBuffID 			= System.Convert.ToInt32(sdr ["SelfBuffID"]);
		TargetBuffID 		= System.Convert.ToInt32(sdr ["TargetBuffID"]);
		SelfActionID 		= System.Convert.ToInt32(sdr ["SelfActionID"]);
		TargetActionID		= System.Convert.ToInt32(sdr ["TargetActionID"]);
		AttackDistance		= System.Convert.ToSingle(sdr ["AttackDistance"]);
	}
}



