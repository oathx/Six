using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IAIAction
{
	/// <summary>
	/// The m_n action I.
	/// </summary>
	protected int 	m_nActionID = int.MinValue;

	/// <summary>
	/// Sets the event I.
	/// </summary>
	/// <param name="nEventID">N event I.</param>
	public void 	SetActionID(int nActionID)
	{
		m_nActionID = nActionID;
	}
	
	/// <summary>
	/// Gets the event ID.
	/// </summary>
	/// <returns>The event I.</returns>
	public int 		GetActionID()
	{
		return m_nActionID;
	}
}

/// <summary>
/// IAI state.
/// </summary>
abstract public class IAIState : IAIAction
{
	public static implicit operator bool(IAIState ep)
	{return ep != null;}

	/// <summary>
	/// The state ID.
	/// </summary>
	protected int 			m_nStateID 	= 0;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="IAIState`1"/> class.
	/// </summary>
	/// <param name="nID">N I.</param>
	/// <param name="entity">Entity.</param>
	public IAIState(int nStateID)
	{
		m_nStateID = nStateID;
	}

	/// <summary>
	/// Gets the state I.
	/// </summary>
	/// <returns>The state I.</returns>
	public virtual int		GetStateID() { return m_nStateID; }
	
	/// <summary>
	/// Raises the condition event.
	/// </summary>
	/// <param name="target">Target.</param>
	public abstract bool 	OnCondition(IAIState target);
	/// <summary>
	/// Raises the start event.
	/// </summary>
	public abstract bool 	OnStart();

	/// <summary>
	/// Raises the update event.
	/// </summary>
	public abstract bool 	OnUpdate(float fElapsed);

	/// <summary>
	/// Raises the exit event.
	/// </summary>
	public abstract bool 	OnExit();

	/// <summary>
	/// Raises the event event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	public abstract bool 	OnEvent(IEvent evt);
}
