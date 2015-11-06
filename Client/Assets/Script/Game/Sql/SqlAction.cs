using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// sqlite support
using Mono.Data.Sqlite;

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
	{ get; private set; }

	/// <summary>
	/// Gets the name.
	/// </summary>
	/// <value>The name.</value>
	public string			Name
	{ get; private set; }

	/// <summary>
	/// Gets the animation I.
	/// </summary>
	/// <value>The animation I.</value>
	public int 				AnimationID
	{ get; private set; }

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