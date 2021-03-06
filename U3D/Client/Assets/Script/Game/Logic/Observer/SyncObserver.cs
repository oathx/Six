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

		RegisterNetEventFactory();
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		SubscribeEvent(TcpEvent.CMD_REPLY_TEAM, OnTeamOnline);
	}

	/// <summary>
	/// Active this instance.
	/// </summary>
	public override void Active ()
	{
		base.Active ();
	}

	/// <summary>
	/// Registers the tcp event factory.
	/// </summary>
	/// <returns><c>true</c>, if tcp event factory was registered, <c>false</c> otherwise.</returns>
	protected void 		RegisterNetEventFactory()
	{
		LogicPlugin plugin = GameEngine.GetSingleton ().QueryPlugin<LogicPlugin> ();
		if (!plugin)
			throw new System.NullReferenceException();

		plugin.RegisterPackageFactory(TcpEvent.CMD_REPLY_TEAM,
			new DefaultNetMessageFactory<TcpEvent.SCNetTeamReply>());
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
	/// Raises the change scene event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool 		OnChangeScene(IEvent evt)
	{
		TcpEvent.SCNetSceneChangeReply v = evt.Args as TcpEvent.SCNetSceneChangeReply;

		GlobalUserInfo.MapID 	= v.MapID;
		GlobalUserInfo.Position = v.Position;
		GlobalUserInfo.Angle 	= v.Angle;

		return LogicHelper.ChangeScene(GlobalUserInfo.MapID);;
	}

	/// <summary>
	/// Raises the team online event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool		OnTeamOnline(IEvent evt)
	{
		PlayerEntity player = m_PlayerManager.GetPlayer();
		if (player)
		{
			TcpEvent.SCNetTeamReply v = evt.Args as TcpEvent.SCNetTeamReply;
			foreach(TcpEvent.TeamStruct t in v.TeamList)
			{
				Vector3 vBorn = SceneSupport.GetSingleton().GetRandomPosition(player.GetPosition(), 2.5f);

				// create team player
				PlayerEntity entity = m_PlayerManager.CreateEntity(EntityType.ET_PLAYER.ToString(), 
				                                                   EntityType.ET_PLAYER, 
				                                                   t.PlayerID, 
				                                                   string.Empty, 
				                                                   vBorn, 
				                                                   Vector3.one, 
				                                                   Vector3.zero,
				                                                   0, 
				                                                   t.JobID) as PlayerEntity;
				if (entity)
				{

				}
			}
		}

		return true;
	}
}
