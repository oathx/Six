using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Net cmd.
/// </summary>
public class TcpEvent
{
	public const int CMD_REQ_DEFAULT_SERER_INFO	= 212;
	
	// requeset server info
	public class SCNetServerInfo : IPackageArgs
	{
		public int 		ID
		{ get; set; }
		
		public string 	Name
		{ get; set; }
		
		public int 		Status
		{ get; set; }
		
		/// <summary>
		/// Decode the specified package.
		/// </summary>
		/// <param name="package">Package.</param>
		public override void Decode(IPackage package)
		{
			string text = package.GetString ();
			if (!string.IsNullOrEmpty(text))
			{
				string[] aryText = text.Split('+');
				
				ID 		= System.Convert.ToInt32(aryText[0]);
				Name 	= aryText[1];
				Status 	= 1;
			}
		}
	}

	// requeset login
	public const int CMD_REQ_LOGIN 			= 21;
	public const int CMD_PUSH_LOGIN_SUCCESS = 22;
	
	// user info struct
	public class SCNetLogin : IPackageArgs
	{
		public int 		UserID
		{ get; set; }
		
		public string 	GateIP
		{ get; set; }
		
		public int 		Port
		{ get; set; }
		
		public int 		LoginTime
		{ get; set; }
		
		public string	LoginCode
		{ get; set; }
		
		/// <summary>
		/// Decode the specified package.
		/// </summary>
		/// <param name="package">Package.</param>
		public override void Decode(IPackage package)
		{
			UserID 		= package.GetInt32();
			GateIP 		= package.GetString();
			Port 		= package.GetInt32();
			LoginTime	= package.GetInt32 ();
			LoginCode 	= package.GetString ();
		}
	}
	
	public const int CMD_PUSH_ERROR 		= 12;
	// net error
	public class SCNetError : IPackageArgs
	{
		public int 		ErrorCode
		{ get; set; }
		
		/// <summary>
		/// Decode the specified package.
		/// </summary>
		/// <param name="package">Package.</param>
		public override void Decode(IPackage package)
		{
			ErrorCode = package.GetInt32 ();
		}
	}

	//Send all the game player information request(username:string, password:string)
	public const int CMD_REQ_REGISTER_ROLES  		= 31;     	//All the game player role information request
	public const int CMD_REQ_CREATE_ROLE     		= 32;     	//The request to create the role
	public const int CMD_PUSH_REGISTER_ROLES 		= 33;     	//Returns all the game player role information
	public const int CMD_PUSH_CREATE_ROLE    		= 34;     	//Return to create the role of success
	
	public const int CMD_REQ_ENTER_WORLD          	= 35;     	//Request to enter the world	
	public const int CMD_PUSH_ENTER_WORLD_SUCCESS 	= 20101; 	//Back into the world of success
	
	public class CharacterInfo
	{
		/// <summary>
		/// Gets or sets the role I.
		/// </summary>
		/// <value>The role I.</value>
		public int 			PlayerID
		{ get; set; }
		
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string 		Name
		{ get; set; }
		
		/// <summary>
		/// Gets or sets the job.
		/// </summary>
		/// <value>The job.</value>
		public sbyte 		Job
		{ get; set; }
		
		/// <summary>
		/// Gets or sets the rank.
		/// </summary>
		/// <value>The rank.</value>
		public int 			Rank
		{ get; set; }
		
		/// <summary>
		/// Gets or sets the evalation.
		/// </summary>
		/// <value>The evalation.</value>
		public int 			Evalation
		{ get; set; }
		
		/// <summary>
		/// Gets or sets the equip count.
		/// </summary>
		/// <value>The equip count.</value>
		public int 			EquipCount
		{ get; set; }
		
		/// <summary>
		/// The equip list.
		/// </summary>
		public List<int>	EquipList = new List<int>();
	}
	
	public class SCNetCreateRole : IPackageArgs
	{
		/// <summary>
		/// The player I.
		/// </summary>
		public int 		PlayerID;
		/// <summary>
		/// The name of the player.
		/// </summary>
		public string 	PlayerName;
		/// <summary>
		/// The occupation.
		/// </summary>
		public sbyte 	Occupation;
		
		/// <summary>
		/// Decode the specified package.
		/// </summary>
		/// <param name="package">Package.</param>
		public override void Decode(IPackage package)
		{
			PlayerID 	= package.GetInt32 ();
			PlayerName 	= package.GetString ();
			Occupation 	= package.GetInt8 ();
		}
	}
	
	/// <summary>
	/// SC net character info.
	/// </summary>
	public class SCNetCharacterInfoList : IPackageArgs 
	{	
		/// <summary>
		/// The character list.
		/// </summary>
		public List<CharacterInfo> CharacterList = new List<CharacterInfo> ();
		
		/// <summary>
		/// Decode the specified package.
		/// </summary>
		/// <param name="package">Package.</param>
		public override void Decode(IPackage package)
		{
			int nCount = package.GetInt8 ();
			for(int idx=0; idx<nCount; idx++)
			{
				CharacterInfo ci = new CharacterInfo();
				ci.Name			= package.GetString();
				ci.Job			= package.GetInt8();
				ci.Rank			= package.GetInt8();
				ci.PlayerID		= package.GetInt32();
				ci.Evalation	= package.GetInt16();
				ci.EquipCount	= package.GetInt8();
				
				for(int j=0; j<ci.EquipCount; j++)
				{
					ci.EquipList.Add(package.GetInt32());
				}
				
				CharacterList.Add(ci);
			}
		}
	}

	public class SCNetCharacterSpawnInfo : IPackageArgs
	{
		public int 	    PlayerID
		{ get; set; }
		
		public int 		MapID
		{ get; set; }
		
		public int 		WorldID
		{ get; set; }
		
		public int 		SelfID
		{ get; set; }
		
		public string 	Name
		{ get; set; }
		
		public float	Angle
		{ get; set; }
		
		public sbyte	Job
		{ get; set; }
		
		public short 	Level
		{ get; set; }
		
		public int 		HP
		{ get; set; }
		
		public int 		Exp
		{ get; set; }
		
		public int		MP
		{ get; set; }
		
		
		public int 		Money
		{ get; set; }
		
		public int		Emoney
		{ get; set; }
		
		public int		Honor
		{ get; set; }
		
		public Vector3	Position
		{ get; set; }
		
		public int 		Timestamp
		{ get; set; }
		
		public int		FightValue
		{ get; set; }
		
		public sbyte	BagOpen
		{ get; set; }
		
		public sbyte	StoreOpen
		{ get; set; }
		
		public int		EnergyPiece
		{ get; set; }
		
		public sbyte	MakeEnergyIndex
		{ get; set; }
		
		public int 		StartStone
		{ get; set;}
		
		public sbyte 	StoneBuyTimes
		{ get; set;}
		
		public sbyte    MaxPowerBuyTimes
		{ get; set;}
		
		public int 		MaxPowerTimeStamp
		{ get; set;}
		
		public int 		LastedLevelGotGiftID
		{ get; set; }
		
		public  int 	LastedGotOnlineGiftID
		{ get; set; }
		
		public  int 	LastedGiftTime
		{ get; set; }
		
		public  int 	NextGiftTime
		{ get; set; }
		
		public int 		BoolAttendance
		{ get; set; }
		
		public int 		LastAttendanceTime
		{ get; set; }
		
		public int 		TotalAttendanceCount
		{ get; set; }
		
		public int 		UntilAttendanceID
		{ get; set; }
		
		public int 		AccumulateID
		{ get; set; }
		
		public int 		UntilPayEmoney
		{ get; set; }
		
		public int 		LastVipGiftID
		{ get; set; }
		
		public int 		GrowId
		{ get; set; }
		
		public string 	MagicWeaponInfo
		{get;set;}
		
		public int 		OpenEffectState
		{ get; set; }
		
		/// <summary>
		/// Decode the specified package.
		/// </summary>
		/// <param name="package">Package.</param>
		public override void Decode(IPackage package)
		{
			Timestamp 				= package.GetInt32 ();
			PlayerID 				= package.GetInt32 ();
			MapID					= package.GetInt32 ();
			WorldID	 				= package.GetInt32 ();
			SelfID 					= package.GetInt32 ();
			Position				= package.GetVector3 ();
			Angle 					= package.GetFloat ();
			Name 					= package.GetString ();
			Job 					= package.GetInt8 ();
			Level 					= package.GetInt16();
			Exp						= package.GetInt32();
			BagOpen					= package.GetInt8();
			StoreOpen				= package.GetInt8();
			Money					= package.GetInt32();
			Honor					= package.GetInt32();
			HP 						= package.GetInt32();
			MP 						= package.GetInt32();
			Emoney      			= package.GetInt32();
			EnergyPiece 			= package.GetInt32();
			MakeEnergyIndex       	= package.GetInt8();
			StartStone            	= package.GetInt32 ();
			StoneBuyTimes         	= package.GetInt8 ();
			MaxPowerTimeStamp     	= package.GetInt32();
			LastedLevelGotGiftID  	= package.GetInt8();
			NextGiftTime          	= package.GetInt32();
			LastedGotOnlineGiftID 	= package.GetInt8();
			LastAttendanceTime    	= package.GetInt32();
			TotalAttendanceCount  	= package.GetInt16();
			UntilAttendanceID     	= package.GetInt16();
			AccumulateID          	= package.GetInt8();
			MaxPowerBuyTimes      	= package.GetInt8 ();
			UntilPayEmoney 			= package.GetInt32();
			LastVipGiftID 			= package.GetInt8();
			GrowId 					= package.GetInt8();
			MagicWeaponInfo 		= package.GetString();
			//OpenEffectState 		= package.GetInt8();
		}
	}
}