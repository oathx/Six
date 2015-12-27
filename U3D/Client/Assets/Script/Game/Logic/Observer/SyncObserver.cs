using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Joystick observer.
/// </summary>
public class SyncObserver : IEventObserver
{
	protected MonsterManager		m_MonsterManager;
	protected PlayerManager			m_PlayerManager;
	protected IPlayerSyncManager	m_PlayerSyncManager;
	protected IMonsterSyncManager	m_MonsterSyncManager;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		m_MonsterManager 	= GameEngine.GetSingleton().QueryPlugin<MonsterManager>();
		if (!m_MonsterManager)
			throw new System.NullReferenceException();

		m_PlayerManager		= GameEngine.GetSingleton().QueryPlugin<PlayerManager>();
		if (!m_PlayerManager)
			throw new System.NullReferenceException();

		m_PlayerSyncManager = GameEngine.GetSingleton().QueryPlugin<IPlayerSyncManager>();
		if (!m_PlayerSyncManager)
			throw new System.NullReferenceException();

		m_MonsterSyncManager = GameEngine.GetSingleton().QueryPlugin<IMonsterSyncManager>();
		if (!m_MonsterSyncManager)
			throw new System.NullReferenceException();

		RegisterTcpEventFactory();
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		SubscribeEvent (TcpEvent.CMD_PUSH_SYSTEM_TIME,		OnSystemTime);
		SubscribeEvent (TcpEvent.CMD_PUSH_SCENE_CHANGE, 	OnChangeScene);
		SubscribeEvent (TcpEvent.CMD_PUSH_PLAYER_SKILL, 	OnPlayerSkill);
		SubscribeEvent (TcpEvent.CMD_PUSH_PLAYER_ONLINE, 	OnPlayerOnline);
		SubscribeEvent (TcpEvent.CMD_PUSH_PLAYER_OFFLINE, 	OnPlayerOffline);
		SubscribeEvent (TcpEvent.CMD_PUSH_NPC_ONLINE,		OnNpcOnline);

		// sync server time
		InvokeRepeating(
			"OnSyncServerTime", 0, 10.0f
			);
	}

	/// <summary>
	/// Registers the tcp event factory.
	/// </summary>
	/// <returns><c>true</c>, if tcp event factory was registered, <c>false</c> otherwise.</returns>
	protected void 		RegisterTcpEventFactory()
	{
		LogicPlugin plugin = GameEngine.GetSingleton ().QueryPlugin<LogicPlugin> ();
		if (plugin)
		{
			plugin.RegisterPackageFactory(TcpEvent.CMD_PUSH_SYSTEM_TIME, 
			                              new DefaultNetMessageFactory<TcpEvent.SCNetSystemTime>());
			plugin.RegisterPackageFactory(TcpEvent.CMD_PUSH_SCENE_CHANGE, 
			                              new DefaultNetMessageFactory<TcpEvent.SCNetSceneChange>());
			plugin.RegisterPackageFactory(TcpEvent.CMD_PUSH_PLAYER_SKILL, 
			                              new DefaultNetMessageFactory<TcpEvent.SCNetSkill>());
			plugin.RegisterPackageFactory(TcpEvent.CMD_PUSH_PLAYER_ONLINE, 	
			                              new DefaultNetMessageFactory<TcpEvent.SCNetPlayerOnline>());
			plugin.RegisterPackageFactory(TcpEvent.CMD_PUSH_PLAYER_OFFLINE, 	
			                              new DefaultNetMessageFactory<TcpEvent.SCNetPlayerOffline>());
			plugin.RegisterPackageFactory(TcpEvent.CMD_PUSH_NPC_ONLINE, 	
			                              new DefaultNetMessageFactory<TcpEvent.SCNetNPCSpawnInfo>());
		}
	}

	/// <summary>
	/// Gets the sync entity.
	/// </summary>
	/// <returns>The sync entity.</returns>
	/// <param name="bFlag">If set to <c>true</c> b flag.</param>
	/// <param name="nID">N I.</param>
	protected ISyncEntity GetSyncEntity(bool bFlag, int nID)
	{
		return bFlag ? m_PlayerSyncManager.QueryEntity (nID) : m_MonsterSyncManager.QueryEntity (nID);
	}

	/// <summary>
	/// Raises the sync server time event.
	/// </summary>
	protected void 		OnSyncServerTime()
	{
		SyncRequest.GetSingleton().ReqSystemTime();
	}
	
	/// <summary>
	/// Raises the system time event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool		OnSystemTime(IEvent evt)
	{
		TcpEvent.SCNetSystemTime v = evt.Args as TcpEvent.SCNetSystemTime;
		
		// save current net ping time
		GlobalUserInfo.PingTime = Mathf.Min(Time.time - v.ClientTime, 0.01f);
		
		float fSyncTime = v.ServerTime + GlobalUserInfo.PingTime / 2.0f;
		m_PlayerSyncManager.SynchonrizeTime(fSyncTime);
		m_MonsterSyncManager.SynchonrizeTime(fSyncTime);
		
		return true;
	}

	/// <summary>
	/// Raises the change scene event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool 		OnChangeScene(IEvent evt)
	{
		TcpEvent.SCNetSceneChange v = evt.Args as TcpEvent.SCNetSceneChange;

		GlobalUserInfo.MapID 	= v.MapID;
		GlobalUserInfo.Position = v.Position;
		GlobalUserInfo.Angle 	= v.Angle;

		return LogicHelper.ChangeScene(GlobalUserInfo.MapID);;
	}

	/// <summary>
	/// Raises the player online event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool 		OnPlayerOnline(IEvent evt)
	{
		return true;
	}

	/// <summary>
	/// Raises the player offline event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool 		OnPlayerOffline(IEvent evt)
	{
		return true;
	}

	/// <summary>
	/// Raises the npc online event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool		OnNpcOnline(IEvent evt)
	{
		TcpEvent.SCNetNPCSpawnInfo v = evt.Args as TcpEvent.SCNetNPCSpawnInfo;
		if (!m_MonsterManager.Exsit(v.ID))
		{
			IEntity entity = m_MonsterManager.CreateEntity(EntityType.ET_MONSTER.ToString(), EntityType.ET_MONSTER,
			                                               v.ID, v.Name, v.Position, Vector3.one, Vector3.zero, 0, v.MonsterID);
			if (entity) 
			{
				m_MonsterSyncManager.RegisterEntity(new BaseSyncEntity(entity));
			}
		}

		return true;
	}

	/// <summary>
	/// Raises the player skill event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool 		OnPlayerSkill(IEvent evt)
	{
		TcpEvent.SCNetSkill v = evt.Args as TcpEvent.SCNetSkill;

#if OPEN_DEBUG_LOG
		Debug.Log("AOI " + v.MagicID + " action " + v.SkillID);
#endif
		if (m_PlayerManager.IsSelf(v.AttackerID))
		{
			PlayerEntity player = m_PlayerManager.GetPlayer();
			if (player)
				PostAttackEvent(player, v);
		}
		else
		{
			ISyncEntity attacker = GetSyncEntity(v.IsAttackerPlayer, v.AttackerID);
			if (attacker)
			{
				CmdEvent.AttackEventArgs attEvt = new CmdEvent.AttackEventArgs();
				attEvt.MagicID 			= v.MagicID;
				attEvt.SourceID			= v.AttackerID;
				attEvt.TargetID			= v.TargetID;
				attEvt.SkillID			= v.SkillID;
				attEvt.IsAttackerPlayer = v.IsAttackerPlayer;
				attEvt.IsTargetPlayer	= v.IsTargetPlayer;

				attacker.RegisterStateFrame(
					AITypeID.AI_BATTLE, v.Timestamp, new IEvent(EngineEventType.EVENT_USER, CmdEvent.CMD_LOGIC_ATTACK, attEvt)
					);
			}
		}

		return true;
	}

	/// <summary>
	/// Posts the attack event.
	/// </summary>
	/// <param name="entity">Entity.</param>
	/// <param name="v">V.</param>
	protected void 		PostAttackEvent(IEntity entity, TcpEvent.SCNetSkill v)
	{
		IAIMachine machine = entity.GetMachine ();
		if (machine)
		{
			IAIState curState = machine.GetCurrentState ();
			if (curState.StateID != AITypeID.AI_BATTLE)
				machine.ChangeState (AITypeID.AI_BATTLE);
			
			CmdEvent.AttackEventArgs attEvt = new CmdEvent.AttackEventArgs();
			attEvt.MagicID 	= v.MagicID;
			attEvt.SourceID	= v.AttackerID;
			attEvt.TargetID	= v.TargetID;
			attEvt.SkillID	= v.SkillID;
			
			machine.PostEvent(
				new IEvent(EngineEventType.EVENT_USER, CmdEvent.CMD_LOGIC_ATTACK, attEvt)
				);
		}
	}
}
