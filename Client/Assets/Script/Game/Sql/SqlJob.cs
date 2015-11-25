using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// sqlite support
using Mono.Data.Sqlite;
using System.Data;

/// <summary>
/// Sql shape.
/// </summary>
public class SqlJob : ISqlPackage
{
	/// <summary>
	/// Gets the I.
	/// </summary>
	/// <value>The I.</value>
	public int 				ID
	{ get; set; }

	/// <summary>
	/// Gets the name.
	/// </summary>
	/// <value>The name.</value>
	public string			Name
	{ get; set; }

	/// <summary>
	/// Gets the shape I.
	/// </summary>
	/// <value>The shape I.</value>
	public int 				ShapeID
	{ get; set; }
	
	/// <summary>
	/// Gets the weapon.
	/// </summary>
	/// <value>The weapon.</value>
	public string			Weapon
	{ get; set; }
	
	/// <summary>
	/// Gets the describe.
	/// </summary>
	/// <value>The describe.</value>
	public string			Describe
	{ get; set; }

	
	/// <summary>
	/// Decode the specified sdr.
	/// </summary>
	/// <param name="sdr">Sdr.</param>
	public override void 	Decode (SqliteDataReader sdr)
	{
		ID			= System.Convert.ToInt32(sdr ["ID"]);
		Name 		= System.Convert.ToString(sdr ["Name"]);
		ShapeID 	= System.Convert.ToInt32(sdr ["ShapeID"]);
		Weapon 		= System.Convert.ToString(sdr ["Weapon"]);
		Describe	= System.Convert.ToString(sdr ["Describe"]);
	}

	/// <summary>
	/// Encode the specified row.
	/// </summary>
	/// <param name="row">Row.</param>
	public override void 	Encode (DataRow row)
	{
		ID			= System.Convert.ToInt32(row ["ID"]);
		Name 		= System.Convert.ToString(row ["Name"]);
		ShapeID 	= System.Convert.ToInt32(row ["ShapeID"]);
		Weapon 		= System.Convert.ToString(row ["Weapon"]);
		Describe	= System.Convert.ToString(row ["Describe"]);
	}
}

