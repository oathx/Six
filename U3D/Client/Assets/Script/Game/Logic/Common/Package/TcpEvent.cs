using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Net cmd.
/// </summary>
public class TcpEvent
{
	public const int CMD_REQ_LOGIN 					= 21;		//requeset login
	public const int CMD_REQ_DEFAULT_SERER_INFO		= 212;
	public const int CMD_REQ_REGISTER              	= 24;		//send register reqeuest
	public const int CMD_REQ_REGISTER_ROLES  		= 31;     	//All the game player role information request
	public const int CMD_REQ_CREATE_ROLE     		= 32;     	//The request to create the role
	public const int CMD_REQ_ENTER_WORLD          	= 35;     	//Request to enter the world
	public const int CMD_REQ_CHAR_ATTRIBUTE 		= 10300;	//Request Gets Role Properties
	public const int CMD_REQ_CHAR_ATTRIBUTE_UPDATE 	= 10301;	//Request to update the role of property
	public const int CMD_REQ_PLAYER_REVIVE			= 10301;

	public const int CMD_REQ_ITEM_DATA 				= 10400;	//Request backpack Data
	public const int CMD_REQ_ITEM_SWITCH 			= 10402;	//Request to exchange goods
	public const int CMD_REQ_ITEM_OPENSLOT 			= 10403;	//Request to open lattice
	public const int CMD_REQ_ITEM_ADD 				= 10404;	//Request to add items
	public const int CMD_REQ_USE_ITEM 				= 10409; 	//Request to Use Item.
	public const int CMD_REQ_ITEM_SELL 				= 10410;	//Request items for sale
	public const int CMD_REQ_DUP_LIST        		= 10600;    //Request level list 
	public const int CMD_REQ_ENTER_DUP       		= 10601;    //The request into the checkpoint 
	public const int CMD_REQ_LEAVE_DUP       		= 10602;    //Request level returns 
	public const int CMD_REQ_PLYCARD_DUP    		= 10603;     
	public const int CMD_REQ_DUP_REOCRD     		= 10604;     
	public const int CMD_REQ_SAO_DANG        		= 10605;     
	public const int CMD_REQ_GET_STAR_REWARD 		= 10607;     
	public const int CMD_REQ_BUY_ENTER_COUNT 		= 10608; 
	public const int CMD_REQ_PLAYER_INFO			= 10701;
	public const int CMD_REQ_PLAYER_SKILL			= 10702;
	public const int CMD_REQ_PLAYER_HIT				= 10708;
	public const int CMD_REQ_NPC_HIT				= 10709;
	public const int CMD_REQ_SYSTEM_TIME			= 10705;
	public const int CMD_REQ_CHANGE_SCENE			= 10703;
	public const int CMD_REQ_SCENE_EVENT			= 10707;
	public const int CMD_REQ_FIND_PATH				= 10710;
	public const int CMD_REQ_PICK_ITEM				= 10706;
	public const int CMD_REQ_PLAYER_MOVE			= 10700;
	public const int CMD_REQ_PLAYER_STATE			= 10711;
	public const int CMD_REQ_NPC_MOVE				= 10712;
	public const int CMD_REQ_AIMONSTER 				= 90106;
	public const int CMD_REQ_MISSION_LIST 			= 10800;
	public const int CMD_REQ_ACCEPT_MISSION 		= 10801;
	
	public const int CMD_PUSH_ERROR 				= 12;
	public const int CMD_PUSH_LOGIN_SUCCESS 		= 22;
	public const int CMD_PUSH_REGISTER_SUCCESS    	= 25;		// return register success
	public const int CMD_PUSH_REGISTER_ROLES 		= 33;     	//Returns all the game player role information
	public const int CMD_PUSH_CREATE_ROLE    		= 34;     	//Return to create the role of success
	public const int CMD_PUSH_HAS_LOGIN				= 37;
	public const int CMD_PUSH_ENTER_WORLD_SUCCESS 	= 20101; 	//Back into the world of success
	public const int CMD_PUSH_ITEM_DATA 			= 20400;	//Returns Backpack data
	public const int CMD_PUSH_ITEM_SWITCH 			= 20402;	//Return to exchange goods
	public const int CMD_PUSH_ITEM_NUM 				= 20401;	//Returns the number of items
	public const int CMD_PUSH_CHAR_ATTRIBUTE 		= 20300;	//Return to the role of property values
	public const int CMD_PUSH_CHAR_ATTRIBUTE_UPDATE = 20301;	//Returns the value of the updated attribute character
	public const int CMD_PUSH_CONTAINER_NEW_ITEM 	= 20403;	//Return to get new items
	public const int CMD_PUSH_USE_ITEM_RESULT 		= 20408;
	public const int CMD_PUSH_SALE_ITEM 			= 20410;
	public const int CMD_PUSH_PLAYER_ONLINE			= 20103;
	public const int CMD_PUSH_PLAYER_OFFLINE		= 20105;
	public const int CMD_PUSH_PLAYER_SKILL			= 20109;
	public const int CMD_PUSH_PLAYER_HIT			= 20114;
	public const int CMD_PUSH_PLAYER_REVIVE			= 20304;
	public const int CMD_PUSH_SYSTEM_TIME			= 20303;
	public const int CMD_PUSH_NPC_ONLINE			= 20106;
	public const int CMD_PUSH_NPC_MOVE				= 20107;
	public const int CMD_PUSH_SCENE_CHANGE			= 20100;
	public const int CMD_PUSH_PLAYER_SHAPE			= 20112;
	public const int CMD_PUSH_NPC_NAVMESH_MOVE		= 20120;
	public const int CMD_PUSH_FIND_PATH				= 20121;
	public const int CMD_PUSH_DROP_ITEM				= 20122;
	public const int CMD_PUSH_PLAYER_MOVE			= 20104;
	public const int CMD_PUSH_PLAYER_STATE			= 20102;
	public const int CMD_PUSH_PROPERTY_CHANGE 		= 20302;
	public const int CMD_PUSH_PLAYER_PROPERTY 		= 20300;
	public const int CMD_PUSH_MISSION_LIST 			= 20804;
	public const int CMD_PUSH_ACCEPT_MISSION_OK		= 20801;
	public const int CMD_PUSH_DUP_LIST        		= 20601;    
	public const int CMD_PUSH_PASS_DUP       		= 20602;    
	public const int CMD_PUSH_PLYCARD_DUP     		= 20603;    
	public const int CMD_PUSH_DUP_RECORD      		= 20604;    
	public const int CMD_PUSH_SAO_DANG_RESULT 		= 20605;    
	public const int CMD_PUSH_DUP_STAR_BOX    		= 20606;    
	public const int CMD_PUSH_DUP_ENTER_COUNT 		= 20607;   

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

	public class SCNetRegisterSuccess : IPackageArgs {}
	
	public class SCNetHasLogin : IPackageArgs {}
	
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

	public class SCNetPropertyChange : IPackageArgs
	{
		public int 					PropertyCount
		{ get; set; }
		
		/// <summary>
		/// The property list.
		/// </summary>
		public List<PropertyValue> 	PropertyList = new List<PropertyValue>();
		
		/// <summary>
		/// Decode the specified package.
		/// </summary>
		/// <param name="package">Package.</param>
		public override void 	Decode(IPackage package)
		{
			PropertyCount = package.GetInt8 ();
			for(int i=0; i<PropertyCount; i++)
			{
				PropertyValue v = new PropertyValue();
				v.Type	= (PropertyType)package.GetInt16();
				v.Value = package.GetInt32();
				
#if UNITY_EDITOR
				Debug.Log("Property Changed : " + v.Type.ToString() + ":" + v.Value);
#endif
				PropertyList.Add(v);
			}
		}
	}
	
	public class SCNetProperty : IPackageArgs
	{
		public int 			Cambat
		{ get; set; }
		
		public int 			FireControl
		{ get; set; }
		
		public int 			Protection
		{ get; set; }
		
		public int 			Performance
		{ get; set; }
		
		public int 			Energy
		{ get; set; }
		
		public int 			CloseinAttack
		{ get; set; }
		
		public int 			CloseinDefense
		{ get; set; }
		
		public int 			DistanceAttack
		{ get; set; }
		
		public int 			DistanceDefense
		{ get; set; }
		
		public int 			Durable
		{ get; set; }
		
		public int 			Crit
		{ get; set; }
		
		public int 			AntiCrit
		{ get; set; }
		
		public int 			Hit
		{ get; set; }
		
		public int 			Dodge
		{ get; set; }
		
		public int			FightValue
		{ get; set; }

		public override void 	Decode(IPackage package)
		{
			Cambat 			= package.GetInt32 ();
			FireControl 	= package.GetInt32 ();
			Protection 		= package.GetInt32 ();
			Performance 	= package.GetInt32 ();
			Energy 			= package.GetInt32 ();
			CloseinAttack 	= package.GetInt32 ();
			CloseinDefense 	= package.GetInt32 ();
			DistanceAttack 	= package.GetInt32 ();
			DistanceDefense = package.GetInt32 ();
			Durable 		= package.GetInt32 ();
			Crit 			= package.GetInt32 ();
			AntiCrit 		= package.GetInt32 ();
			Hit 			= package.GetInt32 ();
			Dodge 			= package.GetInt32 ();
			FightValue 		= package.GetInt32 ();
		}
	}

	public class SCNetSystemTime : IPackageArgs
	{
		public float 		ClientTime 
		{ get; set; }
		
		public float 		ServerTime 
		{ get; set; }
		
		public sbyte 		ServerFPS 
		{ get; set; }
		
		public int 			ServerUTCTime 
		{ get; set; }
		
		public override void Decode(IPackage package)
		{
			ClientTime 		= (float)package.GetInt32() / 1000; 
			ServerTime 		= (float)package.GetInt32() / 1000;
			ServerFPS 		= package.GetInt8(); //fps
			ServerUTCTime 	= package.GetInt32();
		}
	}

	public class SCNetSceneChange : IPackageArgs
	{
		public int 			Unknown 
		{ get; set; }
		public int 			MapID 
		{ get; set; }
		public Vector3 		Position 
		{ get; set; }
		public float 		Angle 
		{ get; set; }
		
		public override void Decode(IPackage package)
		{
			Unknown 	= package.GetInt32();
			MapID 		= package.GetInt32();
			Position 	= package.GetVector3();
			Angle 		= package.GetFloat();
		}
	}

	public class SCNetPlayerOnline : IPackageArgs
	{
		public int 			PlayerID 
		{ get; set; }
		
		public string 		Name 
		{ get; set; }
		
		public Vector3 		Position 
		{ get; set; }
		
		public float 		Angle
		{ get; set; }
		
		public sbyte 		Job 
		{ get; set; }
		
		public short 		Level
		{ get; set; }
		
		public int 			HP 
		{ get; set; }
		
		public List<int> 	Equip = new List<int>();
		
		public int 			EquipCount
		{ get; set; }
		
		public int 			Evalation
		{ get; set; }
		
		/// <summary>
		/// Decode the specified package.
		/// </summary>
		/// <param name="package">Package.</param>
		public override void Decode(IPackage package)
		{
			PlayerID 	= package.GetInt32();
			Name 		= package.GetString();
			Position 	= package.GetVector3();
			Angle 		= package.GetFloat();
			Job 		= package.GetInt8();
			Level 		= package.GetInt16();
			HP 			= package.GetInt32();
			EquipCount  = package.GetInt8();
			
			for(int idx=0; idx<EquipCount; idx++)
			{
				int nEquipID = package.GetInt32();
				if (nEquipID != 0)
					Equip.Add(nEquipID);
			}
			
			Evalation = package.GetInt16();
		}
	}
	
	public class SCNetPlayerOffline : IPackageArgs
	{
		public bool 	IsPlayer 
		{ get; set; }
		
		public int 		PlayerID 
		{ get; set; }
		
		/// <summary>
		/// Decode the specified package.
		/// </summary>
		/// <param name="package">Package.</param>
		public override void Decode(IPackage package)
		{
			IsPlayer = package.GetBool();
			PlayerID = package.GetInt32();
		}
	}

	public class SCNetNPCSpawnInfo : IPackageArgs
	{
		public int 		ID 
		{ get; set; }

		public string 	Name 
		{ get; set; }

		public Vector3 	Position 
		{ get; set; }

		public Vector3 	TargetPosition 
		{ get; set; }

		public float 	Angle 
		{ get; set; }

		public int 		Hp
		{ get; set; }

		public int 		MonsterID 
		{ get; set; }

		public int 		MaxHP 
		{ get; set; }

		public override void Decode(IPackage package)
		{
			ID 				= package.GetInt32();
			MonsterID 		= package.GetInt32();
			Position 		= package.GetVector3();
			TargetPosition 	= package.GetVector3();
			Angle 			= package.GetFloat();
			Hp 				= package.GetInt32();
			MaxHP 			= package.GetInt32();
		}
	}

	public class SCNetSkill : IPackageArgs
	{
		public float 		Timestamp 
		{ get; set; }
		
		public bool 		IsAttackerPlayer 
		{ get; set; }
		
		public int 			AttackerID
		{ get; set; }
		
		public int 			MagicID
		{ get; set; }
		
		public float	 	Angle 
		{ get; set; }
		
		public int 			SkillID 
		{ get; set; }
		
		public bool 		IsTargetPlayer 
		{ get; set; }
		
		public int 			TargetID
		{ get; set; }
		
		public override void Decode(IPackage package)
		{
			Timestamp 			= (float)package.GetInt32() / 1000;
			IsAttackerPlayer 	= package.GetBool();
			AttackerID 			= package.GetInt32();
			MagicID 			= package.GetInt32();
			Angle 				= package.GetFloat();
			SkillID 			= package.GetInt16();
			IsTargetPlayer 		= package.GetBool();
			TargetID 			= package.GetInt32();
		}
	}

	public class SCNetMissionList : IPackageArgs
	{
		/// <summary>
		/// Gets or sets the accept list.
		/// </summary>
		/// <value>The accept list.</value>
		public string			CurList
		{ get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this instance can accept.
		/// </summary>
		/// <value><c>true</c> if this instance can accept; otherwise, <c>false</c>.</value>
		public string			CanList
		{ get; set; }

		/// <summary>
		/// Decode the specified package.
		/// </summary>
		/// <param name="package">Package.</param>
		public override void 	Decode(IPackage package)
		{
			CurList 	= package.GetString();
			CanList 	= package.GetString();
		}
	}

	public class SCNetAcceptMissionSuccess : IPackageArgs
	{
		public int 				MissionID
		{ get; set; }

		public override void 	Decode(IPackage package)
		{
			MissionID 	= package.GetInt32();
		}
	}
}