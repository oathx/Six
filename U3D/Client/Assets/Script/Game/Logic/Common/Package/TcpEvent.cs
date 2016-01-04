using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Net cmd.
/// </summary>
public class TcpEvent
{
	public const int 	CMD_REPLY_ERROR				= 9999;
	public class SCNetErrorReply 
		: IPackageArgs
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


	public const int CMD_REQ_REGISTER_ACCOUNT		= 1000;		
	public const int CMD_REPLY_REGISTER_SUCCESS		= 1001;
	public class SCNetRegisterSuccess 
		: IPackageArgs 
	{
		
	}

	/* Tcp cmd : request login to server
	 * 
	 * int 		Server Version, 
	 * string 	User Name,
	 * string 	Password, 
	 * int 		Server ID
	 */
	public const int CMD_REQ_LOGIN					= 1002;
	public const int CMD_REPLY_LOGIN_SUCCESS		= 1003;

	// user info struct
	public class SCNetLoginReply
		: IPackageArgs
	{
		public int 		UserID;
		public string 	GateIP;
		public int 		Port;
		public int 		LoginTime;
		public string	LoginCode;

		public override void Decode(IPackage package)
		{
			UserID 		= package.GetInt32();
			GateIP 		= package.GetString();
			Port 		= package.GetInt32();
			LoginTime	= package.GetInt32 ();
			LoginCode 	= package.GetString ();
		}
	}

	/* Tcp cmd:	Request register a role
	 * 
	 * Send to server
	 * 
	 * int 		User ID		
	 * int 		Login Time 
	 * string 	Login Code
	 */
	public const int CMD_REQ_CHARACTER_LIST			= 2000;
	public const int CMD_REPLY_CHARACTER_LIST		= 2001;
	///////////////////////////////////////////////////////
	public class CharacterStruct
	{
		public int 			PlayerID;
		public string 		Name;
		public sbyte 		Job;
		public int 			Rank;

		// the character equip
		public int 			EquipCount;
		public List<int>	
			Equip = new List<int>();
	}

	///////////////////////////////////////////////////////
	public class SCNetCharacterListReply
		: IPackageArgs 
	{	
		/// <summary>
		/// The character list.
		/// </summary>
		public List<CharacterStruct> 
			List = new List<CharacterStruct> ();
		
		/// <summary>
		/// Decode the specified package.
		/// </summary>
		/// <param name="package">Package.</param>
		public override void Decode(IPackage package)
		{
			int nCount = package.GetInt8 ();
			for(int idx=0; idx<nCount; idx++)
			{
				CharacterStruct ci = new CharacterStruct();
				ci.Name			= package.GetString();
				ci.Job			= package.GetInt8();
				ci.Rank			= package.GetInt8();
				ci.PlayerID		= package.GetInt32();
				ci.EquipCount	= package.GetInt8();
				
				for(int j=0; j<ci.EquipCount; j++)
				{
					ci.Equip.Add(package.GetInt32());
				}
				
				List.Add(ci);
			}
		}
	}

	//////////////////////////////////////////////////////
	/* Tcp Cmd: Request create a role
	 * 
	 * string	Role name
	 * sbyte	job id
	 * 
	 */
	public const int CMD_REQ_CREATE_ROLE			= 2002;
	public const int CMD_REPLY_CREATE_ROLE			= 2003;

	// reply create role success
	public class SCNetCreateRoleReply
		: IPackageArgs
	{
		public int 			PlayerID;
		public string 		PlayerName;
		public sbyte 		Occupation;

		public override void Decode(IPackage package)
		{
			PlayerID 	= package.GetInt32 ();
			PlayerName 	= package.GetString ();
			Occupation 	= package.GetInt8 ();
		}
	}

	//////////////////////////////////////////////////////
	/* Tcp Cmd: Request join game world
	 * 
	 * int		player id
	 * 
	 */
	public const int CMD_REQ_ENTER_WORLD			= 2004;
	public const int CMD_REPLY_ENTER_WORLD			= 2005;

	public class SCNetEnterWorldReply 
		: IPackageArgs
	{
		public Vector3	Position;
		public int 		Level;
		public int 		MapID;
		public int 		Job;
		public string	Name;
		public int 		PlayerID;

		public override void Decode(IPackage package)
		{
			MapID		= package.GetInt32();
			Level		= package.GetInt32();
			Name		= package.GetString();
			PlayerID	= package.GetInt32();
			Job			= package.GetInt16();
			Position	= package.GetVector3();
		}
	}

	/* Tcp Cmd: Request join level
	 * 
	 * int		level id
	 * 
	 */
	public const int CMD_REQ_ENTER_LEVEL			= 2006;

	/* Tcp Cmd: Request exit level
	 * 
	 */
	public const int CMD_REQ_LEAVE_LEVEL			= 2008;

	/* Tcp Cmd: Request change scene
	 * 
	 * int		scene id
	 * int 		script id
	 */
	public const int CMD_REQ_CHANGE_SCENE			= 2010;
	public const int CMD_REPLY_SCENE_CHANGE			= 2011;

	public class SCNetSceneChangeReply 
		: IPackageArgs
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
	public const int CMD_REQ_SCENE_EVENT		= 2012;


	///////////////////////////////////////////////////////
	public class SCNetEnterWorld : IPackageArgs
	{
		public int 	    PlayerID
		{ get; set; }
		
		public int 		MapID
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

		/// <summary>
		/// Decode the specified package.
		/// </summary>
		/// <param name="package">Package.</param>
		public override void Decode(IPackage package)
		{

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

		public int 			EquipCount
		{ get; set; }

		public List<int> 
			Equip = new List<int>();

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
		}
	}
}