using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

/// <summary>
/// Login server.
/// </summary>
public class SceneServer : VirtualServer
{
	/// <summary>
	/// Awake this instance.
	/// </summary>
	protected override void Awake ()
	{
		base.Awake();
	}
	
	// Use this for initialization
	void Start ()
	{
		SubscribeEvent (TcpEvent.CMD_REQ_ENTER_WORLD, new EventCallback (OnReqEnterWorld));
		SubscribeEvent (TcpEvent.CMD_REQ_SCENE_EVENT, new EventCallback (OnReqSceneEvent));
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	/// <summary>
	/// Raises the req enter world event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	private bool	OnReqEnterWorld(IEvent evt)
	{
		UICharList charList = UISystem.GetSingleton().Query<UICharList>(
			ResourceDef.UI_CHARLIST
			);
		if (charList)
		{
			UICharItem item = charList.GetSelectItem();
			if (!item)
				throw new System.NullReferenceException();

			VirtualNetPackage vp = evt.Args as VirtualNetPackage;

			TcpEvent.SCNetCharacterSpawnInfo spawn = new TcpEvent.SCNetCharacterSpawnInfo ();
			spawn.Position 	= new Vector3 (-81.0f, 2.0f, 100.0f);
			spawn.Level 	= (short)item.Level;
			spawn.MapID		= 3100;
			spawn.Job 		= (sbyte)item.Job;
			spawn.Name 		= GlobalUserInfo.MapID.ToString();
			
			m_Plugin.SendEvent (
				new IEvent (EngineEventType.EVENT_NET, TcpEvent.CMD_PUSH_ENTER_WORLD_SUCCESS, spawn)
				);

		}


		return true;
	}


	/// <summary>
	/// Raises the req scene event event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	private bool	OnReqSceneEvent(IEvent evt)
	{
		VirtualNetPackage vp = evt.Args as VirtualNetPackage;
		
		// reset current map id
		GlobalUserInfo.MapID = SceneSupport.GetSingleton ().GetSceneID ();
		
		// protocol param
		int nEventType 	= (int)vp.buffer [0];
		int nGroupID 	= (int)vp.buffer [1];
		
		// get the scene monster gourp
		List<MonsterStruct> aryResult = MonsterTable.GetSingleton ().GetGroupMonster (GlobalUserInfo.MapID, nGroupID);
		for(int idx=0; idx<aryResult.Count; idx++)
		{
			SqlMonster sqlMonster = GameSqlLite.GetSingleton().Query<SqlMonster>(aryResult[idx].NpcID);
			if (!sqlMonster)
				throw new System.NullReferenceException();

			TcpEvent.SCNetNPCSpawnInfo spawn = new TcpEvent.SCNetNPCSpawnInfo();
			spawn.ID		= aryResult[idx].ID;
			spawn.Hp		= sqlMonster.Durable;
			spawn.Name		= sqlMonster.Name;
			spawn.MonsterID	= aryResult[idx].NpcID;
			spawn.Position	= aryResult[idx].Position;
			
			m_Plugin.SendEvent (
				new IEvent (EngineEventType.EVENT_NET, TcpEvent.CMD_PUSH_NPC_ONLINE, spawn)
				);
		}
		
		return true;
	}
}


