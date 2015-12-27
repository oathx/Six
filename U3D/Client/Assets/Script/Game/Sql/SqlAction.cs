using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// sqlite support
using Mono.Data.Sqlite;
using System.Data;

/// <summary>
/// Sql shape.
/// </summary>
public class SqlAction : ISqlPackage
{
	public int 				ID
	{ get; set; }

	public string			Name
	{ get; set; }
	
	public string			Motion
	{ get; set; }

	public float			MotionTransition
	{ get; set; }

	public float			MotionSpeed
	{ get; set; }

	public string			Describe
	{ get; set; }

	/// <summary>
	/// Decode the specified sdr.
	/// </summary>
	/// <param name="sdr">Sdr.</param>
	public override void 	Decode (SqliteDataReader sdr)
	{
		ID 				= System.Convert.ToInt32(sdr ["ID"]);
		Name 			= System.Convert.ToString(sdr ["Name"]);
		Motion			= System.Convert.ToString(sdr ["Motion"]);
		MotionTransition= System.Convert.ToSingle(sdr ["MotionTransition"]);
		MotionSpeed		= System.Convert.ToSingle(sdr ["MotionSpeed"]);
	}

	/// <summary>
	/// Fill the specified row.
	/// </summary>
	/// <param name="row">Row.</param>
	/// <param name="sdr">Sdr.</param>
	public override void 	Fill(DataRow sdr)
	{
		ID 				= System.Convert.ToInt32(sdr ["ID"]);
		Name 			= System.Convert.ToString(sdr ["Name"]);
		Motion			= System.Convert.ToString(sdr ["Motion"]);
		MotionTransition= System.Convert.ToSingle(sdr ["MotionTransition"]);
		MotionSpeed		= System.Convert.ToSingle(sdr ["MotionSpeed"]);
	}
}
