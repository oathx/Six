using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// AOI sync entity.
/// </summary>
public class BaseSyncEntity : ISyncEntity
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AOISyncEntity"/> class.
	/// </summary>
	/// <param name="entity">Entity.</param>
	public BaseSyncEntity(IEntity entity)
		: base(entity)
	{
		
	}
	
	/// <summary>
	/// Raises the target progress event.
	/// </summary>
	/// <param name="vSource">V source.</param>
	/// <param name="vTarget">V target.</param>
	/// <param name="fSource">F source.</param>
	/// <param name="fTarget">F target.</param>
	/// <param name="fLerpTime">F lerp time.</param>
	/// <param name="fLinerSpeed">F liner speed.</param>
	public override void 	OnTargetProgress(Vector3 vSource, Vector3 vTarget,
	                                       float fSource, float fTarget, float fLerpTime, float fLinerSpeed)
	{
		if (m_Entity)
		{
			// get the entity machine
			IAIMachine machine 	= m_Entity.GetMachine();
			if (!machine)
				throw new System.NullReferenceException();
			
			Vector3 vLinerPos	= Vector3.Lerp(vSource, vTarget, fLerpTime);
			Vector3 vLinerRot	= Vector3.up * Mathf.LerpAngle(fSource, fTarget, fLerpTime);
			
			// get the entity current position
			IAIState curState = machine.GetCurrentState();
			if (curState.StateID != AITypeID.AI_MOVE)
				machine.ChangeState(AITypeID.AI_MOVE);
			
			m_Entity.SetPosition(vLinerPos);
			m_Entity.SetRotateTo(vLinerRot);
			
		}
	}
	
	/// <summary>
	/// Raises the arrival target event.
	/// </summary>
	/// <param name="vPosition">V position.</param>
	/// <param name="fAngle">F angle.</param>
	public override void 	OnArrivalTarget(Vector3 vPosition, float fAngle)
	{
		if (m_Entity)
		{
			m_Entity.SetPosition(vPosition);
			m_Entity.SetRotateTo(fAngle * Vector3.up);
			
			// get the entity machine
			IAIMachine machine 	= m_Entity.GetMachine();
			if (machine)
				machine.ChangeState(AITypeID.AI_IDLE);
		}
	}
	
	/// <summary>
	/// Raises the target state event.
	/// </summary>
	/// <param name="nStateID">N state I.</param>
	/// <param name="stateEvent">State event.</param>
	public override void 	OnTargetState(int nStateID, IEvent stateEvent)
	{
#if UNITY_EDITOR
		Debug.Log("Execute state " + nStateID);
#endif
		
		IAIMachine machine = m_Entity.GetMachine ();
		if (machine)
		{
			IAIState curState = machine.GetCurrentState();
			if (curState.StateID != nStateID)
				machine.ChangeState(nStateID);
			
			machine.PostEvent(stateEvent);
		}
	}
}


