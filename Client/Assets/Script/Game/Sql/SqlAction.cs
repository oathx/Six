using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// sqlite support
using Mono.Data.Sqlite;
using System.Data;

/// <summary>
/// Action effect.
/// </summary>
public class ActionEffect
{
	public int			EffectID
	{ get; private set; }
	
	public float		DelayTime
	{ get; private set; }
	
	/// <summary>
	/// Initializes a new instance of the <see cref="SqlState+StateEffect"/> class.
	/// </summary>
	/// <param name="nEffectID">N effect I.</param>
	/// <param name="fDelayTime">F delay time.</param>
	public ActionEffect(int nEffectID, float fDelayTime)
	{
		EffectID 	= nEffectID;
		DelayTime 	= fDelayTime;
	}
}

/// <summary>
/// Sql shape.
/// </summary>
public class SqlAction : ISqlPackage
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
	/// Gets the animation I.
	/// </summary>
	/// <value>The animation I.</value>
	public int 				AnimationID
	{ get; set; }

	/// <summary>
	/// The effect.
	/// </summary>
	public List<ActionEffect>	
		Effect = new List<ActionEffect>();

	/// <summary>
	/// Decode the specified sdr.
	/// </summary>
	/// <param name="sdr">Sdr.</param>
	public override void 	Decode (SqliteDataReader sdr)
	{
		ID 					= System.Convert.ToInt32(sdr ["ID"]);
		Name 				= System.Convert.ToString(sdr ["Name"]);
		AnimationID 		= System.Convert.ToInt32(sdr ["AnimationID"]);

		DecodeEffect(
			System.Convert.ToString(sdr ["Effect"])
			);
	}

	/// <summary>
	/// Encode the specified row.
	/// </summary>
	/// <param name="row">Row.</param>
	public override void 	Encode (DataRow row)
	{
		ID 					= System.Convert.ToInt32(row ["ID"]);
		Name 				= System.Convert.ToString(row ["Name"]);
		AnimationID 		= System.Convert.ToInt32(row ["AnimationID"]);
		
		DecodeEffect(
			System.Convert.ToString(row ["Effect"])
			);
	}

	/// <summary>
	/// Decodes the effect.
	/// </summary>
	/// <param name="text">Text.</param>
	protected void 			DecodeEffect(string text)
	{
		if (!string.IsNullOrEmpty(text))
		{
			string[] arySplit = text.Split(',');
			foreach(string effect in arySplit)
			{
				string[] aryEffect = effect.Split(':');
				if (aryEffect.Length == 2)
				{
					Effect.Add(new ActionEffect(int.Parse(aryEffect[0]), float.Parse(arySplit[1])));
				}
			}
		}
	}
}