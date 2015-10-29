using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CityType
{
	CT_NEW = 1,
	CT_MAIN = 2,
	CT_SINGLE = 3,
	CT_PVP = 4,
	CT_TEST = 100,
}

/// <summary>
/// Global user info.
/// </summary>
public class GlobalUserInfo
{
	public static string		GateIPAddress;
	public static int 			GatePort;
	public static string		LoginCode;
	public static int 			LoginTime;
	public static float			PingTime;
	public static int 			UserID;
	public static int 			PlayerID;
	public static int 			Job;
	public static int 			Level;
	public static Vector3		Position;
	public static int 			MapID;
	public static string		Name;
	public static float			Angle;
	public static int 			Combat;
	public static int 			FireControl;
	public static int 			Protection;
	public static int 			Performance;
	public static int 			Energy;
	public static int 			CloseinAttack;
	public static int 			CloseinDefense;
	public static int 			DistanceAttack;
	public static int 			DistanceDefense;
	public static int 			Durable;
	public static int 			Crit;
	public static int 			AntiCrit;
	public static int 			Hit;
	public static int 			Dodge;
	public static int			FightValue;
	public static int			Honor;
	public static int       	SkillPoint;
	public static int			MaxPowerTimeStamp;
	public static int 			HP;
	public static int 			Exp;
	public static int 			Mp;
	public static int      		MaxMp;
	public static int 			Money;
	public static int 			EMoney;
	public static CityType		City;

	/// <summary>
	/// The property integer.
	/// </summary>
	public static Dictionary<PropertyType, 
		int> PropertyInteger = new Dictionary<PropertyType, int>();

	/// <summary>
	/// Sets the property.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="nValue">N value.</param>
	public static void 			SetProperty(PropertyType type, int nValue)
	{
		if (!PropertyInteger.ContainsKey (type))
		{
			PropertyInteger.Add (type, nValue);
		}
		else
		{
			PropertyInteger [type] = nValue;
		}
	}

	/// <summary>
	/// Gets the property.
	/// </summary>
	/// <returns>The property.</returns>
	/// <param name="type">Type.</param>
	public static int			GetProperty(PropertyType type)
	{
		return PropertyInteger [type];
	}
}
