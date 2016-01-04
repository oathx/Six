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
}
