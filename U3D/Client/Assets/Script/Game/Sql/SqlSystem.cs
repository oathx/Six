using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// sqlite support
using Mono.Data.Sqlite;
using System.Data;

/// <summary>
/// Sql shape.
/// </summary>
public class SqlSystem : ISqlPackage
{
	/// <summary>
	/// Gets the I.
	/// </summary>
	/// <value>The I.</value>
	public string 			Version
	{ get; set; }

	/// <summary>
	/// Gets or sets the server version.
	/// </summary>
	/// <value>The server version.</value>
	public int 				ServerVersion
	{ get; set; }

	/// <summary>
	/// Gets or sets the server I.
	/// </summary>
	/// <value>The server I.</value>
	public int 				ServerID
	{ get; set; }

	/// <summary>
	/// Gets or sets the IP address.
	/// </summary>
	/// <value>The IP address.</value>
	public string			IPAddress
	{ get; set; }

	/// <summary>
	/// Gets or sets the port.
	/// </summary>
	/// <value>The port.</value>
	public int 				Port
	{ get; set; }

	/// <summary>
	/// Gets or sets the user I.
	/// </summary>
	/// <value>The user I.</value>
	public int 				UserID
	{ get; set; }

	/// <summary>
	/// Gets the name.
	/// </summary>
	/// <value>The name.</value>
	public string			UserName
	{ get; set; }
	
	/// <summary>
	/// Gets the shape I.
	/// </summary>
	/// <value>The shape I.</value>
	public string 			Password
	{ get; set; }
	
	/// <summary>
	/// Gets the weapon.
	/// </summary>
	/// <value>The weapon.</value>
	public string			MainScript
	{ get; set; }

	/// <summary>
	/// Decode the specified sdr.
	/// </summary>
	/// <param name="sdr">Sdr.</param>
	public override void 	Decode (SqliteDataReader sdr)
	{
		Version			= System.Convert.ToString(sdr ["Version"]);
		UserName 		= System.Convert.ToString(sdr ["UserName"]);
		Password 		= System.Convert.ToString(sdr ["Password"]);
		MainScript 		= System.Convert.ToString(sdr ["MainScript"]);
		ServerVersion	= System.Convert.ToInt32(sdr ["ServerVersion"]);
		ServerID 		= System.Convert.ToInt32(sdr ["ServerID"]);
		IPAddress 		= System.Convert.ToString(sdr ["IPAddress"]);
		Port 			= System.Convert.ToInt32(sdr ["Port"]);
	}

	/// <summary>
	/// Fill the specified row.
	/// </summary>
	/// <param name="row">Row.</param>
	/// <param name="sdr">Sdr.</param>
	public override void 	Fill(DataRow sdr)
	{
		Version			= System.Convert.ToString(sdr ["Version"]);
		UserName 		= System.Convert.ToString(sdr ["UserName"]);
		Password 		= System.Convert.ToString(sdr ["Password"]);
		MainScript 		= System.Convert.ToString(sdr ["MainScript"]);
		ServerVersion	= System.Convert.ToInt32(sdr ["ServerVersion"]);
		ServerID 		= System.Convert.ToInt32(sdr ["ServerID"]);
		IPAddress 		= System.Convert.ToString(sdr ["IPAddress"]);
		Port 			= System.Convert.ToInt32(sdr ["Port"]);
	}
}


