using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

/// <summary>
/// Sql shape.
/// </summary>
public class SqlScene : ISqlPackage
{
	/// <summary>
	/// Gets or sets the I.
	/// </summary>
	/// <value>The I.</value>
	public int 				ID
	{ get; set; }
	
	/// <summary>
	/// Gets the name.
	/// </summary>
	/// <value>The name.</value>
	public string 			Name
	{ get; private set; }
	
	/// <summary>
	/// Gets the skeleton.
	/// </summary>
	/// <value>The skeleton.</value>
	public string 			Url
	{ get; private set; }
	
	/// <summary>
	/// The colour.
	/// </summary>
	public bool				Stream
	{ get; private set; }
	
	/// <summary>
	/// Gets the radius.
	/// </summary>
	/// <value>The radius.</value>
	public string			Script
	{ get; private set; }
	
	/// <summary>
	/// Gets the height.
	/// </summary>
	/// <value>The height.</value>
	public string			Describe
	{ get; private set; }

	/// <summary>
	/// Decode the specified sdr.
	/// </summary>
	/// <param name="sdr">Sdr.</param>
	public override void 	Decode (SqliteDataReader sdr)
	{
		ID			= System.Convert.ToInt32(sdr["ID"]);
		Name 		= System.Convert.ToString(sdr["Name"]);
		Url			= System.Convert.ToString(sdr["Url"]);
		Stream		= System.Convert.ToInt32(sdr["Stream"]) != 0;
		Script 		= System.Convert.ToString(sdr["Script"]);
		Describe 	= System.Convert.ToString(sdr["Describe"]);
	}
}
