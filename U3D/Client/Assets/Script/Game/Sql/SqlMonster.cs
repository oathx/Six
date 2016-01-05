using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// sqlite support
using Mono.Data.Sqlite;
using System.Data;

/// <summary>
/// Magic struct.
/// </summary>
public class MagicStruct
{
	public int 				ID
	{ get; set; }
	
	public int 				Probability
	{ get; set; }
}

/// <summary>
/// Sql shape.
/// </summary>
public class SqlMonster : ISqlPackage
{
	/// <summary>
	/// Gets the shape I.
	/// </summary>
	/// <value>The shape I.</value>
	public int 				ID
	{ get; private set; }
	
	/// <summary>
	/// Gets the weapon.
	/// </summary>
	/// <value>The weapon.</value>
	public string			Name
	{ get; private set; }
		
	/// <summary>
	/// Gets the shape I.
	/// </summary>
	/// <value>The shape I.</value>
	public int				ShapeID
	{ get; private set; }
	
	/// <summary>
	/// Gets the level.
	/// </summary>
	/// <value>The level.</value>
	public int 				Level
	{ get; private set; }

	/// <summary>
	/// Gets or sets the AI blueprint.
	/// </summary>
	/// <value>The AI blueprint.</value>
	public string			Blueprint
	{ get; set; }

	/// <summary>
	/// Decode the specified sdr.
	/// </summary>
	/// <param name="sdr">Sdr.</param>
	public override void 	Decode (SqliteDataReader sdr)
	{
		ID 			= System.Convert.ToInt32(sdr ["ID"]);
		Name		= System.Convert.ToString(sdr ["Name"]);
		ShapeID		= System.Convert.ToInt32(sdr ["Shape"]);
		Level		= System.Convert.ToInt32(sdr ["Level"]);
		Blueprint	= System.Convert.ToString(sdr ["AI"]);
	}

	/// <summary>
	/// Fill the specified sdr.
	/// </summary>
	/// <param name="sdr">Sdr.</param>
	public override void 	Fill (DataRow sdr)
	{
		ID 			= System.Convert.ToInt32(sdr ["ID"]);
		Name		= System.Convert.ToString(sdr ["Name"]);
		ShapeID		= System.Convert.ToInt32(sdr ["Shape"]);
		Level		= System.Convert.ToInt32(sdr ["Level"]);
		Blueprint	= System.Convert.ToString(sdr ["AI"]);
	}
}
