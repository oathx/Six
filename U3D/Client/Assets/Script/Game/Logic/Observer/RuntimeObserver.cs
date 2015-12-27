using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Login observer.
/// </summary>
public class RuntimeObserver : IEventObserver
{
	protected IResourceManager	m_ResourceManager;
	protected LogicPlugin		m_Logic;
	protected PlayerManager		m_PlayerManager;
	protected MonsterManager	m_MonsterManager;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	protected virtual void Awake()
	{
		m_ResourceManager = GameEngine.GetSingleton ().QueryPlugin<IResourceManager> ();
		if (!m_ResourceManager)
			throw new System.NullReferenceException ();

		m_Logic = GameEngine.GetSingleton().QueryPlugin<LogicPlugin>();
		if (!m_Logic)
			throw new System.NullReferenceException();

		// register main player create factory
		m_PlayerManager 	= GameEngine.GetSingleton ().QueryPlugin<PlayerManager> ();
		m_MonsterManager 	= GameEngine.GetSingleton ().QueryPlugin<MonsterManager> ();
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	protected virtual void Start()
	{

	}
}
