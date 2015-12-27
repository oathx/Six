using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// AOI transform frame.
/// </summary>
public class SyncTransformFrame
{
	/// <summary>
	/// Gets or sets the position.
	/// </summary>
	/// <value>The position.</value>
	public Vector3		Position
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the angle.
	/// </summary>
	/// <value>The angle.</value>
	public float		Angle
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the timestamp.
	/// </summary>
	/// <value>The timestamp.</value>
	public float		Timestamp
	{ get; set; }
}

/// <summary>
/// AOI state frame.
/// </summary>
public class SyncStateFrame
{
	/// <summary>
	/// Gets or sets the state I.
	/// </summary>
	/// <value>The state I.</value>
	public int 			StateID
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the timestamp.
	/// </summary>
	/// <value>The timestamp.</value>
	public float		Timestamp
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the state event.
	/// </summary>
	/// <value>The state event.</value>
	public IEvent		StateEvent
	{ get; set; }
}

/// <summary>
/// AOI sync entity.
/// </summary>
public class ISyncEntity
{
	/// <param name="ep">Ep.</param>
	public static implicit operator bool(ISyncEntity t)
	{return t != null;}
	
	/// <summary>
	/// The game entity.
	/// </summary>
	protected IEntity	m_Entity;
	
	/// <summary>
	/// The transform frame list.
	/// </summary>
	protected List<SyncTransformFrame> 
		m_dTransformFrame = new List<SyncTransformFrame>();
	
	/// <summary>
	/// The state frame list.
	/// </summary>
	protected List<SyncStateFrame>
		m_dStateFrame = new List<SyncStateFrame>();
	
	/// <summary>
	/// Initializes a new instance of the <see cref="AOISyncEntity"/> class.
	/// </summary>
	/// <param name="entity">Entity.</param>
	public ISyncEntity(IEntity entity)
	{
		m_Entity = entity;
	}
	
	/// <summary>
	/// Registers the transform frame.
	/// </summary>
	/// <param name="vPosition">V position.</param>
	/// <param name="fTimestamp">F timestamp.</param>
	public virtual bool 	RegisterTransformFrame(Vector3 vPosition, float fAngle, float fTimestamp)
	{
		if (m_dTransformFrame.Count > 0)
		{
			SyncTransformFrame last = m_dTransformFrame[m_dTransformFrame.Count - 1];
			if (last.Position == vPosition)
			{
				// update last transform frame timestamp
				last.Timestamp = fTimestamp;
				
				return true;
			}
		}
		
		SyncTransformFrame aff = new SyncTransformFrame ();
		aff.Position 	= vPosition;
		aff.Timestamp 	= fTimestamp;
		aff.Angle 		= fAngle;
		
		// add new transform frame
		m_dTransformFrame.Add (aff);
		
		return true;
	}
	
	/// <summary>
	/// Registers the state frame.
	/// </summary>
	/// <returns><c>true</c>, if state frame was registered, <c>false</c> otherwise.</returns>
	/// <param name="nStateID">N state I.</param>
	/// <param name="fTimestamp">F timestamp.</param>
	public virtual void		RegisterStateFrame(int nStateID, float fTimestamp)
	{
		SyncStateFrame asf = new SyncStateFrame ();
		asf.StateID 	= nStateID;
		asf.Timestamp 	= fTimestamp;
		
		m_dStateFrame.Add (asf);
	}
	
	/// <summary>
	/// Registers the state frame.
	/// </summary>
	/// <returns><c>true</c>, if state frame was registered, <c>false</c> otherwise.</returns>
	/// <param name="nStateID">N state I.</param>
	/// <param name="fTimestamp">F timestamp.</param>
	/// <param name="evt">Evt.</param>
	public virtual void		RegisterStateFrame(int nStateID, float fTimestamp, IEvent evt)
	{
		SyncStateFrame asf = new SyncStateFrame ();
		asf.StateID 	= nStateID;
		asf.Timestamp 	= fTimestamp;
		asf.StateEvent 	= evt;
		
		m_dStateFrame.Add (asf);
	}
	
	/// <summary>
	/// Gets the entity.
	/// </summary>
	/// <returns>The entity.</returns>
	public virtual IEntity	GetEntity()
	{
		return m_Entity;
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
	public virtual void 	OnTargetProgress(Vector3 vSource, Vector3 vTarget,
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
			
			float fDistance = Vector3.Distance(vLinerPos, vTarget);
#if OPEN_DEBUG_LOG
			Debug.Log("Sync target progress distance " + fDistance + " Liner pos " + vLinerPos.ToString() + " Target pos " + vTarget.ToString());
#endif
			// get the entity current position
			if (fDistance >= 0.2f)
			{
				IAIState curState = machine.GetCurrentState();
				if (curState.StateID != AITypeID.AI_MOVE)
					machine.ChangeState(AITypeID.AI_MOVE);
				
				m_Entity.SetPosition(vLinerPos);
				m_Entity.SetRotateTo(vLinerRot);
				
			}
			else
			{
				machine.ChangeState(AITypeID.AI_IDLE);
			}
		}
	}
	
	/// <summary>
	/// Raises the arrival target event.
	/// </summary>
	/// <param name="vPosition">V position.</param>
	/// <param name="fAngle">F angle.</param>
	public virtual void 	OnArrivalTarget(Vector3 vPosition, float fAngle)
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
	public virtual void 	OnTargetState(int nStateID, IEvent stateEvent)
	{
#if UNITY_EDITOR
		Debug.Log("Execute state " + nStateID);
#endif
	}
	
	/// <summary>
	/// Updates the AO.
	/// </summary>
	/// <param name="fTimeServerSupposed">F time server supposed.</param>
	/// <param name="fDelayTime">F delay time.</param>
	public virtual void 	UpdateSync(float fTimeServerSupposed, float fDelayTime)
	{
		int i = 0;
		int j = 0;
		
		// Transform queue requires at least two frames
		if (m_dTransformFrame.Count >= 2)
		{
			for(i=0,j=0; i<m_dTransformFrame.Count - 1; i++)
			{
				SyncTransformFrame source = m_dTransformFrame[i + 0];
				SyncTransformFrame target = m_dTransformFrame[i + 1];
				
				if (target.Timestamp >= fTimeServerSupposed &&
				    source.Timestamp <= fTimeServerSupposed)
				{
					// calc lerp time
					float fLerpTime = (fTimeServerSupposed - source.Timestamp) / (target.Timestamp - source.Timestamp);
					
					// update linear speed
					if (m_Entity)
					{
						float fDistance = Vector3.Distance(m_Entity.GetPosition(), target.Position);
						float fSpeed 	= fDistance / (target.Timestamp - fTimeServerSupposed);
						
						OnTargetProgress(source.Position, target.Position, source.Angle, target.Angle, fLerpTime, fSpeed);
					}
					
					j = i;
					break;
				}
				else if ( target.Timestamp < fTimeServerSupposed )
				{
					j = (i == 0 ? 1 : i);
				}
			}
			
			// Remove the processed frames
			m_dTransformFrame.RemoveRange(0, j);
			
			if (m_dTransformFrame.Count == 1)
			{
				OnArrivalTarget(
					m_dTransformFrame[0].Position, m_dTransformFrame[0].Angle
					);
			}
		}
		
		// Processing Player Stats
		for (i = 0, j = 0; i < m_dStateFrame.Count; i++, j++)
		{
			SyncStateFrame sf = m_dStateFrame[i];
			if (sf.Timestamp > fTimeServerSupposed)
				break;
			
			OnTargetState(sf.StateID, sf.StateEvent);
		}
		
		// Remove the processed state frames
		m_dStateFrame.RemoveRange(0, j);
	}
}

/// <summary>
/// AOI manager.
/// </summary>
public class ISyncPlugin : IGamePlugin
{
	protected float m_fDelayTime		= 0.50f;
	protected float m_fTimeServer 		= -1.0f;
	protected float m_fPerSandTime 		= 60.0f;
	protected float m_fSandTime 		= 60.0f;
	protected float m_fElasped 			= 0.00f;
	protected float m_fClientTime		= 0.00f;
	protected float m_fServerElapsed	= 0.00f;
	
	/// <summary>
	/// The AOI Entity list
	/// </summary>
	protected List<ISyncEntity>
		m_dEntity = new List<ISyncEntity>();
	
	/// <summary>
	/// Install this instance.
	/// </summary>
	public override void 	Install()
	{}
	
	/// <summary>
	/// Unstall this instance.
	/// </summary>
	public override void 	Uninstall()
	{}
	
	/// <summary>
	/// Startup this instance.
	/// </summary>
	public override void 	Startup()
	{}
	
	/// <summary>
	/// Shutdown this instance.
	/// </summary>
	public override void 	Shutdown()
	{
		
	}
	
	/// <summary>
	/// client synchonrizes the server time.
	/// </summary>
	/// <param name="fServerTime">F server time.</param>
	public virtual void SynchonrizeTime(float fCurrentServerTime)
	{
		float fTimeDiff = fCurrentServerTime - m_fTimeServer + m_fDelayTime;
		
		if (fTimeDiff >= 0.0f && fTimeDiff <= m_fDelayTime * 2.0f)
		{
			m_fSandTime += m_fPerSandTime;
		}
		else
		{
			m_fTimeServer = fCurrentServerTime - m_fDelayTime;
			m_fSandTime = m_fPerSandTime;
			m_fElasped = 0.0f;
		}
	}
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		m_fServerElapsed = Time.deltaTime;
	}
	
	// Update is called once per frame
	void Update()
	{
		//float fCurElapsed 	= Mathf.Min (m_fServerElapsed, Time.deltaTime);
		float fCurrentTime 	= m_fTimeServer + m_fElasped;
		float fElapsedReal 	= Mathf.Min(fCurrentTime + Time.deltaTime, m_fTimeServer + m_fSandTime) - fCurrentTime;
		
		m_fElasped += fElapsedReal;
		
		if (m_fTimeServer > 0.0f)
		{
			// Supposed server time, convert to client current time
			m_fClientTime = m_fTimeServer + m_fElasped;
			
			// update all AOI object
			for (int i = 0; i < m_dEntity.Count; i++)
			{
				m_dEntity[i].UpdateSync(m_fClientTime, m_fDelayTime);
			}
		}
	}
	
#if UNITY_EDITOR
	/// <summary>
	/// Raises the GU event.
	/// </summary>
	void OnGUI()
	{
		GUI.Label (new Rect (0, 20, Screen.width, 20),
		           "ClientDeltaTime:" + Time.deltaTime.ToString("f3") + " ServerDeltaTime:" + m_fServerElapsed.ToString("f3"));
	}
#endif
	
	/// <summary>
	/// Sets the dealy time.
	/// </summary>
	/// <param name="fDelay">F delay.</param>
	public void 			SetDealyTime(float fDelay)
	{
		m_fDelayTime = fDelay;
	}
	
	/// <summary>
	/// Gets the dealy time.
	/// </summary>
	/// <returns>The dealy time.</returns>
	public float 			GetDealyTime()
	{
		return m_fDelayTime;
	}
	
	/// <summary>
	/// Registers the AO.
	/// </summary>
	/// <param name="aoi">Aoi.</param>
	public virtual void 	RegisterEntity(ISyncEntity entity)
	{
		// ref the aoi object
		m_dEntity.Add(entity);
	}
	
	/// <summary>
	/// Gets the current server time.
	/// </summary>
	/// <returns>The current server time.</returns>
	public float			GetServerSupposedTime()
	{
		return m_fClientTime;
	}
	
	/// <summary>
	/// Sets the server elapsed.
	/// </summary>
	/// <param name="fElapsed">F elapsed.</param>
	public void 			SetServerElapsed(float fElapsed)
	{
		m_fServerElapsed = fElapsed;
	}
	
	/// <summary>
	/// Queries the entity.
	/// </summary>
	/// <returns>The entity.</returns>
	/// <param name="nID">N I.</param>
	public ISyncEntity 	QueryEntity(long nID)
	{
		for(int idx=0; idx<m_dEntity.Count; idx++)
		{
			IEntity entity = m_dEntity[idx].GetEntity();
			if (entity && entity.GetID() == nID)
				return m_dEntity[idx];
		}
		
		return null;
	}
	
	/// <summary>
	/// Unregisters the entity.
	/// </summary>
	/// <param name="entity">Entity.</param>
	public void 			UnregisterEntity(ISyncEntity entity)
	{
		m_dEntity.Remove(entity);
	}
	
	/// <summary>
	/// Unregisters the entity.
	/// </summary>
	/// <param name="nEntityID">N entity I.</param>
	public void 			UnregisterEntity(long nEntityID)
	{
		for(int idx=0; idx<m_dEntity.Count; idx++)
		{
			IEntity entity = m_dEntity[idx].GetEntity();
			if (entity && entity.GetID() == nEntityID)
			{
				m_dEntity.RemoveAt(idx);
				break;
			}
		}
	}
}
