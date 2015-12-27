using UnityEngine;
using System.Collections;
using System.Collections.Generic;
// sqlite support
using Mono.Data.Sqlite;
using System.Data;

/// <summary>
/// Combo magic.
/// </summary>
public class ComboMagic
{
	/// <summary>
	/// Gets the source.
	/// </summary>
	/// <value>The source.</value>
	public int 				Source
	{ get; private set; }
	
	/// <summary>
	/// Gets the target.
	/// </summary>
	/// <value>The target.</value>
	public int 				Target
	{ get; private set; }
	
	/// <summary>
	/// Initializes a new instance of the <see cref="ComboSkill"/> class.
	/// </summary>
	/// <param name="nSource">N source.</param>
	/// <param name="nTarget">N target.</param>
	public ComboMagic(int nSource, int nTarget)
	{
		Source = nSource; 
		Target = nTarget;
	}
}


/// <summary>
/// Sql shape.
/// </summary>
public class SqlSkill : ISqlPackage
{
	/// <summary>
	/// Gets the I.
	/// </summary>
	/// <value>The I.</value>
	public int 					ID
	{ get; private set; }

	/// <summary>
	/// Gets the magic I.
	/// </summary>
	/// <value>The magic I.</value>
	public int 					MagicID
	{ get; private set; }

	/// <summary>
	/// Gets the self I.
	/// </summary>
	/// <value>The self I.</value>
	public int 					SelfID
	{ get; private set; }

	/// <summary>
	/// The hit I.
	/// </summary>
	public List<int>			
		HitID = new List<int> ();

	/// <summary>
	/// The fly hit I.
	/// </summary>
	public List<int> 			
		FlyID = new List<int>();

	/// <summary>
	/// Gets the boss hit I.
	/// </summary>
	/// <value>The boss hit I.</value>
	public int 					BossHitID
	{ get; private set; }

	/// <summary>
	/// Gets the fly hit I.
	/// </summary>
	/// <value>The fly hit I.</value>
	public int 					FlyHitID
	{ get; private set; }

	/// <summary>
	/// Gets the play recover action.
	/// </summary>
	/// <value>The play recover action.</value>
	public int 					PlayRecoverAction
	{ get; private set; }
	
	/// <summary>
	/// Gets the name.
	/// </summary>
	/// <value>The name.</value>
	public string				Name
	{ get; private set; }

	/// <summary>
	/// Gets the combo initial.
	/// </summary>
	/// <value>The combo initial.</value>
	public int 					ComboInitial
	{ get; private set; }

	/// <summary>
	/// Gets the nextmagic.
	/// </summary>
	/// <value>The nextmagic.</value>
	public List<ComboMagic>		
		NextMagic = new List<ComboMagic>();

	/// <summary>
	/// Gets the start time.
	/// </summary>
	/// <value>The start time.</value>
	public float				StartTime
	{ get; private set; }

	/// <summary>
	/// Gets the end time.
	/// </summary>
	/// <value>The end time.</value>
	public float				EndTime
	{ get; private set; }

	/// <summary>
	/// Gets the minimum charge distance.
	/// </summary>
	/// <value>The minimum charge distance.</value>
	public float				MinChargeDistance
	{ get; private set; }

	/// <summary>
	/// Gets the max charge distance.
	/// </summary>
	/// <value>The max charge distance.</value>
	public float				MaxChargeDistance
	{ get; private set; }

	/// <summary>
	/// Gets the follow.
	/// </summary>
	/// <value>The follow.</value>
	public float				Follow
	{ get; private set; }

	/// <summary>
	/// Gets the noplay.
	/// </summary>
	/// <value>The noplay.</value>
	public int 					Noplay
	{ get; private set; }

	/// <summary>
	/// Gets the automatic.
	/// </summary>
	/// <value>The automatic.</value>
	public int 					Automatic
	{ get; private set; }

	/// <summary>
	/// Gets the weapon.
	/// </summary>
	/// <value>The weapon.</value>
	public int 					Weapon
	{ get; private set; }

	/// <summary>
	/// The impact time start.
	/// </summary>
	public List<float>			
		ImpactTimeStart = new List<float>();

	/// <summary>
	/// The takeaim.
	/// </summary>
	public List<float>			
		Takeaim = new List<float>();

	/// <summary>
	/// The subsection.
	/// </summary>
	public List<int> 
		Subsection = new List<int>();

	/// <summary>
	/// Gets the play speed.
	/// </summary>
	/// <value>The play speed.</value>
	public float			PlaySpeed
	{ get; private set; }

	/// <summary>
	/// Gets the speed time.
	/// </summary>
	/// <value>The speed time.</value>
	public float			SpeedTime
	{ get; private set; }

	/// <summary>
	/// Gets the camera time.
	/// </summary>
	/// <value>The camera time.</value>
	public float			CameraTime
	{ get; private set; }

	/// <summary>
	/// Gets the camera I.
	/// </summary>
	/// <value>The camera I.</value>
	public int 				CameraID
	{ get; private set; }

	/// <summary>
	/// Gets the max distance.
	/// </summary>
	/// <value>The max distance.</value>
	public float			MaxDistance
	{ get; private set; }

	/// <summary>
	/// Gets the range.
	/// </summary>
	/// <value>The range.</value>
	public float			Range
	{ get; private set; }

	/// <summary>
	/// Gets the radius.
	/// </summary>
	/// <value>The radius.</value>
	public float			Radius
	{ get; private set; }

	/// <summary>
	/// Gets the attack height s.
	/// </summary>
	/// <value>The attack height s.</value>
	public float			AttackHeightS
	{ get; private set; }

	/// <summary>
	/// Gets the attack angle distance.
	/// </summary>
	/// <value>The attack angle distance.</value>
	public float			AttackAngleDistance
	{ get; private set; }

	/// <summary>
	/// Gets the explain.
	/// </summary>
	/// <value>The explain.</value>
	public string			Explain
	{ get; private set; }

	/// <summary>
	/// Decode the specified sdr.
	/// </summary>
	/// <param name="sdr">Sdr.</param>
	public override void 	Decode (SqliteDataReader sdr)
	{
		ID 					= System.Convert.ToInt32(sdr ["id"]);
		MagicID 			= System.Convert.ToInt32(sdr ["skillId"]);
		SelfID 				= System.Convert.ToInt32(sdr ["SelfID"]);

		HitID				= MathfEx.SplitInt32(
			System.Convert.ToString(sdr["HitID"]), '|');
	
		FlyID				= MathfEx.SplitInt32(
			System.Convert.ToString(sdr["FlyID"]), '|');

		BossHitID 			= System.Convert.ToInt32(sdr ["BOSSID"]);
		FlyHitID 			= System.Convert.ToInt32(sdr ["flyHitId"]);
		PlayRecoverAction 	= System.Convert.ToInt32(sdr ["playRecoverAction"]);

		Name 				= System.Convert.ToString(sdr ["Name"]);
		ComboInitial 		= System.Convert.ToInt32(sdr ["ComboInitial"]);

		string szMagic 		= System.Convert.ToString(sdr["Nextmagic"]);
		string[] arySplit 	= szMagic.Split('|');
		foreach(string s in arySplit)
		{
			string[] aryNext = s.Split(':');
			if (aryNext.Length >= 2)
			{
				NextMagic.Add(new ComboMagic(int.Parse(aryNext[0]), int.Parse(aryNext[1])));
			}
		}

		StartTime 			= System.Convert.ToSingle(sdr ["Starttime"]);
		EndTime 			= System.Convert.ToSingle(sdr ["endtime"]);

		List<int> aryCharge = MathfEx.SplitInt32(
			System.Convert.ToString(sdr ["Charge"]), '|');
		if (aryCharge.Count >= 2)
		{
			MinChargeDistance = aryCharge[0];
			MaxChargeDistance = aryCharge[1];
		}

		Follow 				= System.Convert.ToInt32(sdr ["Follow"]);
		Noplay 				= System.Convert.ToInt32(sdr ["Noplay"]);
		Automatic 			= System.Convert.ToInt32(sdr ["Automatic"]);

		Weapon 				= System.Convert.ToInt32(sdr ["Weapon"]);

		ImpactTimeStart 	= MathfEx.SplitSingle(
			System.Convert.ToString(sdr ["impactTimeStart"]), '|');
		Takeaim				= MathfEx.SplitSingle(
			System.Convert.ToString(sdr["takeaim"]), '|');
		Subsection			= MathfEx.SplitInt32(
			System.Convert.ToString(sdr ["Subsection"]), '|');

		PlaySpeed 			= System.Convert.ToSingle(sdr ["playspeed"]);
		SpeedTime 			= System.Convert.ToSingle(sdr ["speedtime"]);
		CameraTime 			= System.Convert.ToSingle(sdr ["CameraTime"]);
		CameraID 			= System.Convert.ToInt32(sdr ["CameraId"]);
		MaxDistance 		= System.Convert.ToSingle(sdr ["MaxDistance"]);
		Range 				= System.Convert.ToSingle(sdr ["Range"]);
		Radius 				= System.Convert.ToSingle(sdr ["radius"]);
		AttackHeightS 		= System.Convert.ToSingle(sdr ["attackHeightS"]);
		AttackAngleDistance = System.Convert.ToSingle(sdr ["attackAngleDistance"]);
		Explain 			= System.Convert.ToString(sdr ["Explain"]);

	}

	public override void 	Fill(DataRow sdr)
	{
		ID 					= System.Convert.ToInt32(sdr ["id"]);
		MagicID 			= System.Convert.ToInt32(sdr ["skillId"]);
		SelfID 				= System.Convert.ToInt32(sdr ["SelfID"]);
		
		HitID				= MathfEx.SplitInt32(
			System.Convert.ToString(sdr["HitID"]), '|');
		
		FlyID				= MathfEx.SplitInt32(
			System.Convert.ToString(sdr["FlyID"]), '|');
		
		BossHitID 			= System.Convert.ToInt32(sdr ["BOSSID"]);
		FlyHitID 			= System.Convert.ToInt32(sdr ["flyHitId"]);
		PlayRecoverAction 	= System.Convert.ToInt32(sdr ["playRecoverAction"]);
		
		Name 				= System.Convert.ToString(sdr ["Name"]);
		ComboInitial 		= System.Convert.ToInt32(sdr ["ComboInitial"]);
		
		string szMagic 		= System.Convert.ToString(sdr["Nextmagic"]);
		string[] arySplit 	= szMagic.Split('|');
		foreach(string s in arySplit)
		{
			string[] aryNext = s.Split(':');
			if (aryNext.Length >= 2)
			{
				NextMagic.Add(new ComboMagic(int.Parse(aryNext[0]), int.Parse(aryNext[1])));
			}
		}
		
		StartTime 			= System.Convert.ToSingle(sdr ["Starttime"]);
		EndTime 			= System.Convert.ToSingle(sdr ["endtime"]);
		
		List<int> aryCharge = MathfEx.SplitInt32(
			System.Convert.ToString(sdr ["Charge"]), '|');
		if (aryCharge.Count >= 2)
		{
			MinChargeDistance = aryCharge[0];
			MaxChargeDistance = aryCharge[1];
		}
		
		Follow 				= System.Convert.ToInt32(sdr ["Follow"]);
		Noplay 				= System.Convert.ToInt32(sdr ["Noplay"]);
		Automatic 			= System.Convert.ToInt32(sdr ["Automatic"]);
		
		Weapon 				= System.Convert.ToInt32(sdr ["Weapon"]);
		
		ImpactTimeStart 	= MathfEx.SplitSingle(
			System.Convert.ToString(sdr ["impactTimeStart"]), '|');
		Takeaim				= MathfEx.SplitSingle(
			System.Convert.ToString(sdr["takeaim"]), '|');
		Subsection			= MathfEx.SplitInt32(
			System.Convert.ToString(sdr ["Subsection"]), '|');
		
		PlaySpeed 			= System.Convert.ToSingle(sdr ["playspeed"]);
		SpeedTime 			= System.Convert.ToSingle(sdr ["speedtime"]);
		CameraTime 			= System.Convert.ToSingle(sdr ["CameraTime"]);
		CameraID 			= System.Convert.ToInt32(sdr ["CameraId"]);
		MaxDistance 		= System.Convert.ToSingle(sdr ["MaxDistance"]);
		Range 				= System.Convert.ToSingle(sdr ["Range"]);
		Radius 				= System.Convert.ToSingle(sdr ["radius"]);
		AttackHeightS 		= System.Convert.ToSingle(sdr ["attackHeightS"]);
		AttackAngleDistance = System.Convert.ToSingle(sdr ["attackAngleDistance"]);
		Explain 			= System.Convert.ToString(sdr ["Explain"]);
	}
}


