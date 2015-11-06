using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// I entity movable.
/// </summary>
public class AIMove : AIBaseState
{
	protected float		m_fRepPostionTime 	= 0.0f;
	protected float		m_fRepPostionCount	= 5.0f;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="MainIdle"/> class.
	/// </summary>
	/// <param name="nStateID">N state I.</param>
	/// <param name="entity">Entity.</param>
	public AIMove(int nStateID, IEntity entity)
		: base(nStateID, entity)
	{
		m_fRepPostionTime = 1.0f / m_fRepPostionCount;
	}
	
	/// <summary>
	/// Raises the start event.
	/// </summary>
	public override bool 	OnStart()
	{
		IEntityShape shape = m_Entity.GetShape();
		if (shape)
			shape.ApplyRootMotion(false);

		PlayStateAction (
			AIActionID.AI_MOVE_ACTION
			);
		
		return base.OnStart ();
	}
	
	/// <summary>
	/// Raises the update event.
	/// </summary>
	public override bool 	OnUpdate(float fElapsed)
	{
		/*
		if (m_Property.Type == EntityType.ET_MAIN)
		{
			m_fRepPostionTime -= Time.deltaTime;
			if (m_fRepPostionTime <= 0.0f)
			{
				// sync sync position
				AOIRequest.GetSingleton().RequestPlayerMove(
					m_Entity.GetPosition(), m_Entity.GetEulerAngle().y, true);
				
				m_fRepPostionTime = 1.0f / m_fRepPostionCount;
			}
		}
		*/
		
		return base.OnUpdate (fElapsed);
	}
	
	/// <summary>
	/// Raises the exit event.
	/// </summary>
	public override bool 	OnExit()
	{
		base.OnExit ();

		// stop move
		if (m_Entity)
			m_Entity.Move (Vector3.zero, 0);

		return true;
	}
	
	/// <summary>
	/// Raises the event event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	public override bool 	OnEvent(IEvent evt)
	{
		return base.OnEvent(evt);
	}
	
}
