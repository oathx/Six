using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

public enum SceneType
{
	SCENE_LOGIN			= 0,
	SCENE_CHARACTER		= 1,
	SCENE_CITY			= 2,
	SCENE_SINGLE		= 3,
}

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
	/// Gets the type.
	/// </summary>
	/// <value>The type.</value>
	public SceneType 		Type
	{ get; private set; }

	/// <summary>
	/// Gets the skeleton.
	/// </summary>
	/// <value>The skeleton.</value>
	public string 			Url
	{ get; private set; }

	/// <summary>
	/// Gets the bron.
	/// </summary>
	/// <value>The bron.</value>
	public Vector3			Born
	{ get; private set; }

	/// <summary>
	/// Gets the camera I.
	/// </summary>
	/// <value>The camera I.</value>
	public int 				CameraID
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
		Type 		= (SceneType)System.Convert.ToInt32(sdr["Type"]);
		Born		= MathfEx.ToVector3(System.Convert.ToString(sdr["Born"]));
		CameraID	= System.Convert.ToInt32(sdr["CameraID"]);
	}
}
