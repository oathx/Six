using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// IAI machine.
/// </summary>
public class IAIMachine
{
	public static implicit operator bool(IAIMachine ep)
	{return ep != null;}

	/// <summary>
	/// The state of the m_ global.
	/// </summary>
	protected IAIState	m_GlobalState;
	protected IAIState	m_CurrentState;
	protected IAIState	m_PrevState;

	/// <summary>
	/// The state of the m_d.
	/// </summary>
	protected Dictionary<int,
		IAIState> m_dState = new Dictionary<int, IAIState> ();
	
	// local post event buffer
	protected class PostEventBuffer
	{
		public static implicit operator bool(PostEventBuffer peb)
		{return peb != null;}

		// buffer event param
		public IEvent	evt
		{ get; set; }

		// target state 
		public int 		target
		{ get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="IAIMachine+PostEventBuffer"/> class.
		/// </summary>
		/// <param name="e">E.</param>
		/// <param name="nTargetID">N target I.</param>
		public PostEventBuffer(IEvent e, int nTargetID)
		{
			evt = e; target = nTargetID;
		}
	}

	/// <summary>
	/// The m_d post event.
	/// </summary>
	protected List<PostEventBuffer> 
		m_dPostEvent = new List<PostEventBuffer> ();

	/// <summary>
	/// Initializes a new instance of the <see cref="IAIMachine`1"/> class.
	/// </summary>
	/// <param name="entity">Entity.</param>
	public IAIMachine()
	{

	}

	/// <summary>
	/// Sets the state of the global.
	/// </summary>
	/// <param name="state">State.</param>
	public void			SetGlobalState(IAIState state)
	{
		m_GlobalState = state;
	}

	/// <summary>
	/// Gets the state of the global.
	/// </summary>
	/// <returns>The global state.</returns>
	public IAIState		GetGlobalState()
	{
		return m_GlobalState;
	}

	/// <summary>
	/// Sets the state of the global.
	/// </summary>
	/// <param name="state">State.</param>
	public void			SetCurrentState(IAIState state)
	{
		if (m_CurrentState != state)
		{
			if (m_CurrentState)
			{
				m_CurrentState.OnExit();
			}
		}

		m_PrevState		= m_CurrentState;
		m_CurrentState 	= state;

		if (m_CurrentState)
			m_CurrentState.OnStart ();
	}
	
	/// <summary>
	/// Gets the state of the global.
	/// </summary>
	/// <returns>The global state.</returns>
	public IAIState		GetCurrentState()
	{
		return m_CurrentState;
	}

	/// <summary>
	/// Gets the state of the previous.
	/// </summary>
	/// <returns>The previous state.</returns>
	public IAIState		GetPrevState()
	{
		return m_PrevState;
	}

	/// <summary>
	/// Registers the state.
	/// </summary>
	/// <param name="state">State.</param>
	public void 		RegisterState(IAIState state)
	{
		if (!m_dState.ContainsKey(state.StateID))
		{
			m_dState.Add(state.StateID, state);
		}
	}

	/// <summary>
	/// Gets the state.
	/// </summary>
	/// <returns>The state.</returns>
	/// <param name="nStateID">N state I.</param>
	public IAIState		GetState(int nStateID)
	{
		if (m_dState.ContainsKey(nStateID))
			return m_dState [nStateID];

		return default(IAIState);
	}

	/// <summary>
	/// Determines whether this instance is current state the specified state.
	/// </summary>
	/// <returns><c>true</c> if this instance is current state the specified state; otherwise, <c>false</c>.</returns>
	/// <param name="state">State.</param>
	public bool			IsCurrentState(IAIState state)
	{
		return state.StateID == m_CurrentState.StateID;
	}

	/// <summary>
	/// Determines whether this instance is current state the specified nStateID.
	/// </summary>
	/// <returns><c>true</c> if this instance is current state the specified nStateID; otherwise, <c>false</c>.</returns>
	/// <param name="nStateID">N state I.</param>
	public bool			IsCurrentState(int nStateID)
	{
		return m_CurrentState.StateID == nStateID;
	}

	/// <summary>
	/// Removes the state.
	/// </summary>
	/// <param name="nStateID">N state I.</param>
	public void 		RemoveState(int nStateID)
	{
		if (m_dState.ContainsKey(nStateID))
		{
			m_dState.Remove(nStateID);
		}
	}

	/// <summary>
	/// Determines whether this instance has state the specified nStateID.
	/// </summary>
	/// <returns><c>true</c> if this instance has state the specified nStateID; otherwise, <c>false</c>.</returns>
	/// <param name="nStateID">N state I.</param>
	public bool			HasState(int nStateID)
	{
		return m_dState.ContainsKey (nStateID);
	}

	/// <summary>
	/// Clearup this instance.
	/// </summary>
	public void			Clearup()
	{
		// clear post event
		m_dPostEvent.Clear ();

		// clear all state
		List<IAIState> list = new List<IAIState> (m_dState.Values);
		for (int idx=0; idx<list.Count; idx++)
			list.RemoveAt (idx);

		m_dState.Clear ();
	}

	/// <summary>
	/// Changes the state.
	/// </summary>
	/// <param name="state">State.</param>
	public void 		ChangeState(IAIState state)
	{
		bool bYes = true;

		if (m_CurrentState)
		{
			bYes = m_CurrentState.OnCondition(state);
		}

		if (bYes)
		{
			SetCurrentState (state);
		}
	}

	/// <summary>
	/// Changes the state.
	/// </summary>
	/// <param name="nStateID">N state I.</param>
	public void 		ChangeState(int nStateID)
	{
		IAIState state = GetState (nStateID);
		if (!state)
			throw new System.NullReferenceException ("Can't find state " + nStateID);

		ChangeState (state);
	}

	/// <summary>
	/// Posts the event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	public void 		PostEvent(IEvent evt)
	{
		if (evt.DelayTime <= 0.0f)
		{
			bool bResult = false;

			// If the current state of processing the event,
			// then termination event delivery, otherwise the event forwarding to the global state
			if (m_CurrentState)
				bResult = m_CurrentState.OnEvent(evt);

			// post global state proc
			if (!bResult)
			{
				if (m_GlobalState)
					m_GlobalState.OnEvent(evt);
			}
		}
		else
		{
			m_dPostEvent.Add(
				new PostEventBuffer(evt, m_CurrentState.StateID)
					);
		}
	}

	/// <summary>
	/// Translats the buffer event.
	/// </summary>
	public void 		TranslatPostEvent()
	{
		for(int idx=0; idx<m_dPostEvent.Count; idx++)
		{
			PostEventBuffer buffer = m_dPostEvent[idx];

			// event wait ...
			if (buffer.evt.DelayTime <= 0.0f)
			{
				bool bResult = false;
				
				// If the current state of processing the event,
				// then termination event delivery, otherwise the event forwarding to the global state
				if (m_CurrentState)
				{
#if OPEN_DEBUG_LOG
					Debug.Log("AI State event " + m_CurrentState.StateID + " args " + buffer.evt.ID);
#endif
					bResult = m_CurrentState.OnEvent(buffer.evt);
				}

				// post global state proc
				if (!bResult)
				{
					if (m_GlobalState)
						m_GlobalState.OnEvent(buffer.evt);
				}

				// remove the event
				m_dPostEvent.RemoveAt(idx);
			}
			else
			{
				// delay the event
				buffer.evt.DelayTime -= Time.deltaTime;
				
				// reset event time
				if (buffer.evt.DelayTime <= 0.0f)
					buffer.evt.DelayTime = 0.0f;
			}
		}
	}

	/// <summary>
	/// Updates the machine.
	/// </summary>
	public void 		UpdateMachine(float fElapsed)
	{
		// dispatch machine event
		TranslatPostEvent ();

		if (m_GlobalState)
			m_GlobalState.OnUpdate(fElapsed);

		if (m_CurrentState)
			m_CurrentState.OnUpdate(fElapsed);
	}
}
