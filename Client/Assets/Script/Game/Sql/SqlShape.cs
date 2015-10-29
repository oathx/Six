using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;

/// <summary>
/// Sql shape.
/// </summary>
public class SqlShape : ISqlPackage
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
	public string 			Skeleton
	{ get; private set; }
	
	/// <summary>
	/// The equip.
	/// </summary>
	public List<int>		Equip = new List<int> ();
	
	/// <summary>
	/// The colour.
	/// </summary>
	public Color			Colour
	{ get; private set; }
	
	/// <summary>
	/// The center.
	/// </summary>
	public Vector3			Center
	{ get; private set; }
	
	/// <summary>
	/// Gets the radius.
	/// </summary>
	/// <value>The radius.</value>
	public float			Radius
	{ get; private set; }
	
	/// <summary>
	/// Gets the height.
	/// </summary>
	/// <value>The height.</value>
	public float			Height
	{ get; private set; }
	
	/// <summary>
	/// Gets the foot step time.
	/// </summary>
	/// <value>The foot step time.</value>
	public float			FootStepTime
	{ get; private set; }
	
	/// <summary>
	/// Decode the specified sdr.
	/// </summary>
	/// <param name="sdr">Sdr.</param>
	public override void 	Decode (SqliteDataReader sdr)
	{
		ID			= System.Convert.ToInt32(sdr["ID"]);
		Name 		= System.Convert.ToString(sdr["Name"]);
		Skeleton	= System.Convert.ToString(sdr["Skeleton"]);
		Radius		= System.Convert.ToSingle(sdr["Radius"]);
		Height		= System.Convert.ToSingle(sdr["Height"]);
		FootStepTime= System.Convert.ToSingle(sdr["FootStepTime"]);
		Center 		= MathfEx.ToVector3(System.Convert.ToString(sdr["Center"]));

		string equip = System.Convert.ToString(sdr["Equip"]);
		if (!string.IsNullOrEmpty (equip))
		{
			string[] aryEquip = equip.Split(',');
			foreach(string s in aryEquip)
			{
				int nEquipID = System.Convert.ToInt32(s);
				if (nEquipID > 0)
					Equip.Add(nEquipID);
			}
		}
		
		string color = System.Convert.ToString(sdr["Colour"]);
		if (!string.IsNullOrEmpty (equip))
		{
			string[] aryColor = color.Split(',');
			if (aryColor.Length >= 4)
			{
				Colour = new Color32(
					byte.Parse(aryColor[0]), 
					byte.Parse(aryColor[1]), 
					byte.Parse(aryColor[2]), 
					byte.Parse(aryColor[3])
					);
			}
		}
	}
}
