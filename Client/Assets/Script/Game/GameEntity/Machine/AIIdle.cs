using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// I entity movable.
/// </summary>
public class AIIdle : AIBaseState
{
	/// <summary>
	/// Initializes a new instance of the <see cref="IdleState"/> class.
	/// </summary>
	/// <param name="nStateID">N state I.</param>
	/// <param name="entity">Entity.</param>
	public AIIdle(int nStateID, IEntity entity)
		: base(nStateID, entity)
	{

	}

	/// <summary>
	/// Raises the start event.
	/// </summary>
	public override bool 	OnStart()
	{
		IAIMachine machine = m_Entity.GetMachine ();
		if (machine)
		{
			IAIState prevState = machine.GetPrevState();
			if (prevState && prevState.GetStateID() == AITypeID.AI_FLY)
			{
				PlayStateAction (
					AIActionID.AI_FLY_GETUP
					);
			}
			else
			{
				PlayStateAction (
					AIActionID.AI_IDLE_ACTION
					);
			}
		}
		
		return base.OnStart ();
	}
}
