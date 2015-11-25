using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;

public enum ItemType
{
	IT_EQUIP	= 0,
	IT_ARM		= 1,
	IT_MATRIAL	= 2,
	IT_DRUG		= 3,
}

/// <summary>
/// Sql shape.
/// </summary>
public class SqlItem : ISqlPackage
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
	/// Gets the type.
	/// </summary>
	/// <value>The type.</value>
	public int 				Type
	{ get; private set; }

	/// <summary>
	/// Gets the skeleton.
	/// </summary>
	/// <value>The skeleton.</value>
	public string 			Url
	{ get; private set; }

	/// <summary>
	/// Gets the type of the part.
	/// </summary>
	/// <value>The type of the part.</value>
	public int 				Part
	{ get; private set; }

	/// <summary>
	/// Gets the extand URL.
	/// </summary>
	/// <value>The extand URL.</value>
	public string			ExtendUrl
	{ get; private set; }

	/// <summary>
	/// Gets the icon.
	/// </summary>
	/// <value>The icon.</value>
	public string			Icon
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
		Type		= System.Convert.ToInt32(sdr["Type"]);
		Part		= System.Convert.ToInt32(sdr["PartType"]);
		Icon		= System.Convert.ToString(sdr["Icon"]);
		ExtendUrl	= System.Convert.ToString(sdr["ExtendUrl"]);
	}

	/// <summary>
	/// Encode the specified row.
	/// </summary>
	/// <param name="row">Row.</param>
	public override void 	Encode (DataRow row)
	{
		ID			= System.Convert.ToInt32(row["ID"]);
		Name 		= System.Convert.ToString(row["Name"]);
		Url			= System.Convert.ToString(row["Url"]);
		Type		= System.Convert.ToInt32(row["Type"]);
		Part		= System.Convert.ToInt32(row["PartType"]);
		Icon		= System.Convert.ToString(row["Icon"]);
		ExtendUrl	= System.Convert.ToString(row["ExtendUrl"]);
	}
}

