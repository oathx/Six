using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// I entity movable.
/// </summary>
public class BaseEntityState : IAIState
{
	/// <summary>
	/// Gets the player mgr.
	/// </summary>
	/// <value>The player mgr.</value>
	protected PlayerManager		m_PlayerManager;

	/// <summary>
	/// Gets the monster mgr.
	/// </summary>
	/// <value>The monster mgr.</value>
	protected MonsterManager	m_MonsterManager;

	/// <summary>
	/// Gets the resource mgr.
	/// </summary>
	/// <value>The resource mgr.</value>
	protected IResourceManager	m_ResourceManager;

	/// <summary>
	/// The entity object
	/// </summary>
	protected ICharacterEntity	m_Entity;

	/// <summary>
	/// Initializes a new instance of the <see cref="BaseEntityState"/> class.
	/// </summary>
	/// <param name="nStateID">N state I.</param>
	/// <param name="entity">Entity.</param>
	public BaseEntityState(int nStateID, IEntity entity)
		: base(nStateID)
	{
		m_ResourceManager = GameEngine.GetSingleton().QueryPlugin<IResourceManager>();
		if (!m_ResourceManager)
			throw new System.NullReferenceException();

		m_PlayerManager = GameEngine.GetSingleton().QueryPlugin<PlayerManager>();
		if (!m_PlayerManager)
			throw new System.NullReferenceException();

		m_MonsterManager = GameEngine.GetSingleton().QueryPlugin<MonsterManager>();
		if (!m_MonsterManager)
			throw new System.NullReferenceException();

		m_Entity = entity as ICharacterEntity;
	}

	/// <summary>
	/// Raises the condition event.
	/// </summary>
	/// <param name="target">Target.</param>
	public override bool 	OnCondition(IAIState target)
	{
		return true;
	}

	/// <summary>
	/// Raises the start event.
	/// </summary>
	public override bool 	OnStart()
	{
		return true;
	}
	
	/// <summary>
	/// Raises the update event.
	/// </summary>
	public override bool 	OnUpdate(float fElapsed)
	{
		return true;
	}
	
	/// <summary>
	/// Raises the exit event.
	/// </summary>
	public override bool 	OnExit()
	{
		return true;
	}
	
	/// <summary>
	/// Raises the event event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	public override bool 	OnEvent(IEvent evt)
	{
		return true;
	}
}
