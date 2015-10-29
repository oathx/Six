using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		//SubscribeEvent (TcpEvent.CMD_REQ_SCENE_EVENT, new EventCallback (OnReqSceneEvent));
		//SubscribeEvent (TcpEvent.CMD_REQ_SYSTEM_TIME, new EventCallback (OnReqServerTime));
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
		VirtualNetPackage vp = evt.Args as VirtualNetPackage;
		
		TcpEvent.SCNetCharacterSpawnInfo spawn = new TcpEvent.SCNetCharacterSpawnInfo ();
		spawn.Position 	= new Vector3 (-81.0f, 2.0f, 100.0f);
		spawn.Level 	= 1;
		spawn.MapID		= 1010;
		spawn.Job 		= 1;
		spawn.Name 		= GlobalUserInfo.MapID.ToString();
		
		m_Plugin.SendEvent (
			new IEvent (EngineEventType.EVENT_NET, TcpEvent.CMD_PUSH_ENTER_WORLD_SUCCESS, spawn)
			);
		
		return true;
	}

	/*
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
			TcpEvent.SCNetNPCSpawnInfo spawn = new TcpEvent.SCNetNPCSpawnInfo();
			spawn.PlayerID	= int.MinValue + idx;
			spawn.Hp		= 100000;
			spawn.Name		= spawn.PlayerID.ToString();
			spawn.NpcID		= aryResult[idx].ID;
			spawn.Position	= aryResult[idx].Position;
			
			m_Plugin.SendEvent (
				new IEvent (EngineEventType.EVENT_NET, TcpEvent.CMD_PUSH_NPC_ONLINE, spawn)
				);
		}
		
		return true;
	}
	
	/// <summary>
	/// Raises the req server time event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	private bool		OnReqServerTime(IEvent evt)
	{
		VirtualNetPackage vp = evt.Args as VirtualNetPackage;
		
		// rep server time
		TcpEvent.SCNetSystemTime rep = new TcpEvent.SCNetSystemTime ();
		rep.ClientTime = System.Convert.ToSingle((int)vp.buffer [0]) * 0.001f;
		rep.ServerTime = Time.time;
		
		m_Plugin.SendEvent (
			new IEvent (EngineEventType.EVENT_NET, TcpEvent.CMD_PUSH_SYSTEM_TIME, rep)
			);
		
		return true;
	}
	*/
}


