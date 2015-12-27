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
	/// Gets the describe.
	/// </summary>
	/// <value>The describe.</value>
	public int				MapID
	{ get; private set; }
	
	/// <summary>
	/// Gets the level.
	/// </summary>
	/// <value>The level.</value>
	public int 				Level
	{ get; private set; }
	
	/// <summary>
	/// Gets the shape I.
	/// </summary>
	/// <value>The shape I.</value>
	public int				ShapeID
	{ get; private set; }
	
	/// <summary>
	/// Gets the type.
	/// </summary>
	/// <value>The type.</value>
	public int 				Type
	{ get; private set; }
	
	/// <summary>
	/// Gets the type of the sub.
	/// </summary>
	/// <value>The type of the sub.</value>
	public int 				SubType
	{ get; private set; }
	
	/// <summary>
	/// Gets the durable.
	/// </summary>
	/// <value>The durable.</value>
	public int 				Durable
	{ get; private set; }

	/// <summary>
	/// The magic list.
	/// </summary>
	public List<MagicStruct> 
		MagicList = new List<MagicStruct>();

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
		ID 			= System.Convert.ToInt32(sdr ["id"]);
		MapID		= System.Convert.ToInt32(sdr ["map"]);
		Name 		= System.Convert.ToString(sdr ["name"]);
		Level		= System.Convert.ToInt32(sdr ["level"]);
		ShapeID 	= System.Convert.ToInt32(sdr ["Shape"]);
		Type 		= System.Convert.ToInt32(sdr ["type"]);
		SubType 	= System.Convert.ToInt32(sdr ["subType"]);
		Durable		= System.Convert.ToInt32(sdr ["durable"]);
		Blueprint	= System.Convert.ToString(sdr ["AIBlueprint"]);

		string skill = System.Convert.ToString(sdr["defskill"]);
		if (!string.IsNullOrEmpty (skill))
		{
			string[] arySkill = skill.Split('|');
			foreach(string s in arySkill)
			{
				string[] aryStruct = s.Split(':');
				if (aryStruct.Length == 2)
				{
					MagicStruct ms = new MagicStruct();
					ms.ID 			= int.Parse(aryStruct[0]);
					ms.Probability 	= int.Parse(aryStruct[1]);
					
					MagicList.Add(ms);
				}
			}
		}
	}

	/// <summary>
	/// Fill the specified sdr.
	/// </summary>
	/// <param name="sdr">Sdr.</param>
	public override void 	Fill (DataRow sdr)
	{
		ID 			= System.Convert.ToInt32(sdr ["id"]);
		MapID		= System.Convert.ToInt32(sdr ["map"]);
		Name 		= System.Convert.ToString(sdr ["name"]);
		Level		= System.Convert.ToInt32(sdr ["level"]);
		ShapeID 	= System.Convert.ToInt32(sdr ["Shape"]);
		Type 		= System.Convert.ToInt32(sdr ["type"]);
		SubType 	= System.Convert.ToInt32(sdr ["subType"]);
		Durable		= System.Convert.ToInt32(sdr ["durable"]);
		Blueprint	= System.Convert.ToString(sdr ["AIBlueprint"]);

		string skill = System.Convert.ToString(sdr["defskill"]);
		if (!string.IsNullOrEmpty (skill))
		{
			string[] arySkill = skill.Split('|');
			foreach(string s in arySkill)
			{
				string[] aryStruct = s.Split(':');
				if (aryStruct.Length == 2)
				{
					MagicStruct ms = new MagicStruct();
					ms.ID 			= int.Parse(aryStruct[0]);
					ms.Probability 	= int.Parse(aryStruct[1]);
					
					MagicList.Add(ms);
				}
			}
		}
	}
}
