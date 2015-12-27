using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;

public enum SceneType
{
	SCENE_LOGIN			= 0,
	SCENE_CHARACTER		= 1,
	SCENE_CITY			= 2,
	SCENE_SINGLE		= 3,
	SCENE_MULTIPLE		= 4,
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
	{ get; set; }

	/// <summary>
	/// Gets the type.
	/// </summary>
	/// <value>The type.</value>
	public SceneType 		Type
	{ get; set; }

	/// <summary>
	/// Gets the skeleton.
	/// </summary>
	/// <value>The skeleton.</value>
	public string 			Url
	{ get; set; }

	/// <summary>
	/// Gets the name of the asset.
	/// </summary>
	/// <value>The name of the asset.</value>
	public string			AssetName
	{ get; set; }

	/// <summary>
	/// Gets the navmesh.
	/// </summary>
	/// <value>The navmesh.</value>
	public string			Navmesh
	{ get; set; }

	/// <summary>
	/// Gets or sets the pass.
	/// </summary>
	/// <value>The pass.</value>
	public string			Pass
	{ get; set; }

	/// <summary>
	/// The pre load.
	/// </summary>
	public List<string>		PreLoad = new List<string>();

	/// <summary>
	/// Gets the bron.
	/// </summary>
	/// <value>The bron.</value>
	public Vector3			Born
	{ get; set; }

	/// <summary>
	/// Gets the camera I.
	/// </summary>
	/// <value>The camera I.</value>
	public int 				CameraID
	{ get; set; }

	/// <summary>
	/// The colour.
	/// </summary>
	public bool				Stream
	{ get; set; }
	
	/// <summary>
	/// Gets the radius.
	/// </summary>
	/// <value>The radius.</value>
	public string			Script
	{ get; set; }
	
	/// <summary>
	/// Gets the height.
	/// </summary>
	/// <value>The height.</value>
	public string			Describe
	{ get; set; }

	/// <summary>
	/// Decode the specified sdr.
	/// </summary>
	/// <param name="sdr">Sdr.</param>
	public override void 	Decode (SqliteDataReader sdr)
	{
		ID			= System.Convert.ToInt32(sdr["ID"]);
		Name 		= System.Convert.ToString(sdr["Name"]);
		Url			= System.Convert.ToString(sdr["Url"]);
		AssetName	= System.Convert.ToString(sdr["AssetName"]);
		Navmesh		= System.Convert.ToString(sdr["Navmesh"]);
		Pass		= System.Convert.ToString(sdr["Pass"]);
		Stream		= System.Convert.ToInt32(sdr["Stream"]) != 0;
		Script 		= System.Convert.ToString(sdr["Script"]);
		Describe 	= System.Convert.ToString(sdr["Describe"]);
		Type 		= (SceneType)System.Convert.ToInt32(sdr["Type"]);
		Born		= MathfEx.ToVector3(System.Convert.ToString(sdr["Born"]));
		CameraID	= System.Convert.ToInt32(sdr["CameraID"]);

		string preLoad = System.Convert.ToString(sdr["PreLoad"]);
		if (!string.IsNullOrEmpty(preLoad))
		{
			string[] arySplit = preLoad.Split(',');
			foreach(string s in arySplit)
			{
				if (s != string.Empty)
					PreLoad.Add(s);
			}
		}
	}

	/// <summary>
	/// Fill the specified sdr.
	/// </summary>
	/// <param name="sdr">Sdr.</param>
	public override void 	Fill(DataRow sdr)
	{
		ID			= System.Convert.ToInt32(sdr["ID"]);
		Name 		= System.Convert.ToString(sdr["Name"]);
		Url			= System.Convert.ToString(sdr["Url"]);
		AssetName	= System.Convert.ToString(sdr["AssetName"]);
		Navmesh		= System.Convert.ToString(sdr["Navmesh"]);
		Pass		= System.Convert.ToString(sdr["Pass"]);
		Stream		= System.Convert.ToInt32(sdr["Stream"]) != 0;
		Script 		= System.Convert.ToString(sdr["Script"]);
		Describe 	= System.Convert.ToString(sdr["Describe"]);
		Type 		= (SceneType)System.Convert.ToInt32(sdr["Type"]);
		Born		= MathfEx.ToVector3(System.Convert.ToString(sdr["Born"]));
		CameraID	= System.Convert.ToInt32(sdr["CameraID"]);

		string preLoad = System.Convert.ToString(sdr["PreLoad"]);
		if (!string.IsNullOrEmpty(preLoad))
		{
			string[] arySplit = preLoad.Split(',');
			foreach(string s in arySplit)
			{
				if (s != string.Empty)
					PreLoad.Add(s);
			}
		}
	}
}
