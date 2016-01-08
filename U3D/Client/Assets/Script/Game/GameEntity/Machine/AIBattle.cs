using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// I entity movable.
/// </summary>
public class AIBattle : AIBaseState
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AIBattle"/> class.
	/// </summary>
	/// <param name="nStateID">N state I.</param>
	/// <param name="entity">Entity.</param>
	public AIBattle(int nStateID, IEntity entity)
		: base(nStateID, entity)
	{

	}

	
	/// <summary>
	/// Raises the condition event.
	/// </summary>
	/// <param name="target">Target.</param>
	public override bool 	OnCondition (IAIState target)
	{
		return true;
	}
	
	/// <summary>
	/// Raises the start event.
	/// </summary>
	public override bool 	OnStart ()
	{
		return true;
	}
	
	/// <summary>
	/// Raises the update event.
	/// </summary>
	/// <param name="fElapsed">F elapsed.</param>
	public override bool 	OnUpdate (float fElapsed)
	{
		return false;
	}
	
	/// <summary>
	/// Raises the exit event.
	/// </summary>
	public override bool 	OnExit ()
	{
		return true;
	}
	
	/// <summary>
	/// Raises the event event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	public override bool 	OnEvent (IEvent evt)
	{
		switch(evt.ID)
		{
		case CmdEvent.CMD_LOGIC_ATTACK:
			return OnAttack(evt);
		}

		return true;
	}

	/// <summary>
	/// Raises the attack event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	public virtual bool		OnAttack(IEvent evt)
	{
		CmdEvent.AttackEventArgs v = evt.Args as CmdEvent.AttackEventArgs;

		return true;
	}
}

