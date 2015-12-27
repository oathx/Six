using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// sqlite support
using Mono.Data.Sqlite;
using System.Data;

/// <summary>
/// Sql shape.
/// </summary>
public class SqlSpread : ISqlPackage
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
	/// Gets the describe.
	/// </summary>
	/// <value>The describe.</value>
	public string			Icon
	{ get; private set; }
	
	/// <summary>
	/// Gets the name.
	/// </summary>
	/// <value>The name.</value>
	public int				Level
	{ get; private set; }
	
	/// <summary>
	/// Gets the state.
	/// </summary>
	/// <value>The state.</value>
	public int 				State
	{ get; private set; }
	
	/// <summary>
	/// Gets the style.
	/// </summary>
	/// <value>The style.</value>
	public int 				Area
	{ get; private set; }
	
	/// <summary>
	/// Gets the observer.
	/// </summary>
	/// <value>The observer.</value>
	public string			Observer
	{ get; private set; }
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="SqlSpread"/> enable menu.
	/// </summary>
	/// <value><c>true</c> if enable menu; otherwise, <c>false</c>.</value>
	public bool				EnableMenu
	{ get; private set; }
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="SqlSpread"/> enable joystick.
	/// </summary>
	/// <value><c>true</c> if enable joystick; otherwise, <c>false</c>.</value>
	public bool				EnableJoystick
	{ get; private set; }
	
	/// <summary>
	/// Decode the specified sdr.
	/// </summary>
	/// <param name="sdr">Sdr.</param>
	public override void 	Decode (SqliteDataReader sdr)
	{
		ID 				= System.Convert.ToInt32(sdr ["ID"]);
		Name 			= System.Convert.ToString(sdr ["Name"]);
		Icon 			= System.Convert.ToString(sdr ["Icon"]);
		Level 			= System.Convert.ToInt32(sdr ["Level"]);
		State			= System.Convert.ToInt32(sdr ["State"]);
		Area			= System.Convert.ToInt32(sdr ["Area"]);
		Observer		= System.Convert.ToString(sdr ["Observer"]);
		EnableMenu		= System.Convert.ToInt32(sdr ["EnableMenu"]) != 0;
		EnableJoystick	= System.Convert.ToInt32(sdr ["EnableJoystick"]) != 0;
	}
	
	/// <summary>
	/// Fill the specified dc.
	/// </summary>
	/// <param name="dc">Dc.</param>
	public override void 	Fill(DataRow row)
	{
		ID 				= System.Convert.ToInt32(row ["ID"]);
		Name 			= System.Convert.ToString(row ["Name"]);
		Icon 			= System.Convert.ToString(row ["Icon"]);
		Level 			= System.Convert.ToInt32(row ["Level"]);
		State			= System.Convert.ToInt32(row ["State"]);
		Area			= System.Convert.ToInt32(row ["Area"]);
		Observer		= System.Convert.ToString(row ["Observer"]);
		EnableMenu		= System.Convert.ToInt32(row ["EnableMenu"]) != 0;
		EnableJoystick	= System.Convert.ToInt32(row ["EnableJoystick"]) != 0;
	}
}

