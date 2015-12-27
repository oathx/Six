using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Camera configurable.
/// </summary>
public class CameraSetting : IEventArgs
{
	/// <summary>
	/// Gets or sets the I.
	/// </summary>
	/// <value>The I.</value>
	public int 			ID
	{get; set;}
	
	/// <summary>
	/// Gets or sets the offset position.
	/// </summary>
	/// <value>The offset position.</value>
	public Vector3 		OffsetPosition 
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the offset position lerp.
	/// </summary>
	/// <value>The offset position lerp.</value>
	public float 		OffsetPositionLerp 
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the offset euler angles.
	/// </summary>
	/// <value>The offset euler angles.</value>
	public Vector3 		OffsetEulerAngles 
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the offset euler angles lerp.
	/// </summary>
	/// <value>The offset euler angles lerp.</value>
	public float 		OffsetEulerAnglesLerp 
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the field of view.
	/// </summary>
	/// <value>The field of view.</value>
	public float 		FieldOfView 
	{ get; set; }
	
	/// <summary>
	/// Gets or sets a value indicating whether this instance is relative.
	/// </summary>
	/// <value><c>true</c> if this instance is relative; otherwise, <c>false</c>.</value>
	public bool 		IsRelative 
	{ get; set; }
}

/// <summary>
/// Camera state.
/// </summary>
public class CameraStateIDType
{
	public const int NONE 	= -1;		//None.
	public const int NORMAL = 0;		//Unlock state.
	public const int TRACK 	= 1;		//Track state.
	public const int SHAKE 	= 2;		//Shake state.
}

/// <summary>
/// Camera base AI state.
/// </summary>
public class CameraBaseAIState : IAIState
{
	protected ICameraPlugin		m_Owner;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="CameraBaseAIState"/> class.
	/// </summary>
	/// <param name="camera">Camera.</param>
	public CameraBaseAIState(int nID, ICameraPlugin owner)
		: base(nID)
	{
		m_Owner 	= owner;
	}
	
	/// <summary>
	/// Raises the condition event.
	/// </summary>
	/// <param name="target">Target.</param>
	public override bool 	OnCondition(IAIState target)
	{return true;}
	
	/// <summary>
	/// Raises the start event.
	/// </summary>
	public override bool 	OnStart()
	{return true;}
	
	/// <summary>
	/// Raises the update event.
	/// </summary>
	public override bool 	OnUpdate(float fElapsed)
	{return true;}
	
	/// <summary>
	/// Raises the exit event.
	/// </summary>
	public override bool 	OnExit()
	{return true;}
	
	/// <summary>
	/// Raises the event event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	public override bool 	OnEvent(IEvent evt)
	{return true;}
}

/// <summary>
/// Camera lock state.
/// </summary>
public class CameraNormal : CameraBaseAIState
{
	/// <summary>
	/// Initializes a new instance of the <see cref="CameraNormal"/> class.
	/// </summary>
	/// <param name="nID">N I.</param>
	/// <param name="camera">Camera.</param>
	public CameraNormal(ICameraPlugin camera)
		: base(CameraStateIDType.NORMAL, camera)
	{
		
	}
	
	/// <summary>
	/// Raises the update event.
	/// </summary>
	/// <param name="deltaTime">Delta time.</param>
	public override bool OnUpdate(float deltaTime)
	{
		CameraSetting setting = m_Owner.GetConfigurable ();
		
		// update camera euler angle
		m_Owner.UpdateRotation (
			setting.OffsetEulerAnglesLerp * deltaTime
			);
		
		// update camera position
		m_Owner.UpdatePosition (
			setting.OffsetPositionLerp * deltaTime
			);
		
		
		return true;
	}
}

/// <summary>
/// Main camera.
/// </summary>
public class MainCamera {
}

[RequireComponent(typeof(Camera))]
public class ICameraPlugin : IGamePlugin
{
	protected CameraSetting			m_Configurable;
	protected IAIMachine			m_Machine;
	protected IEntity				m_TargetEntity;
	protected Transform				m_TargetPosition;
	protected Transform				m_TargetRotation;
	protected float					m_fResetTime = 0.0f;
	protected float					m_fResetTick = 0.0f;
	protected Camera				m_Camera;

	#if UNITY_EDITOR
	public Vector3					lockPosition 		= Vector3.zero;
	public float					lockPositionLerp 	= 1.0f;
	public Vector3					lockRotation 		= Vector3.zero;
	public float					lockRotationLerp 	= 1.0f;
	#endif
	/// <summary>
	/// Install this instance.
	/// </summary>
	public override void 			Install()
	{
		m_Machine = new IAIMachine ();
		if (!m_Machine)
			throw new System.NullReferenceException ();
		
		m_Machine.RegisterState (new CameraNormal (this));
	}
	
	/// <summary>
	/// Unstall this instance.
	/// </summary>
	public override void 			Uninstall()
	{
		Shutdown ();
	}
	
	/// <summary>
	/// Startup this instance.
	/// </summary>
	public override void 			Startup()
	{
		IAIState state = m_Machine.GetState (CameraStateIDType.NORMAL);
		if (state)
			m_Machine.ChangeState (CameraStateIDType.NORMAL);
		
		GetComponent<Camera>().farClipPlane = 10000.0f;
		GetComponent<Camera>().tag			= typeof(MainCamera).Name;
	}
	
	/// <summary>
	/// Shutdown this instance.
	/// </summary>
	public override void 			Shutdown()
	{
		m_Machine.Clearup ();
	}

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		m_Camera = GetComponent<Camera>();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{
		#if UNITY_EDITOR
		if (m_Configurable != null)
		{
			m_Configurable.OffsetPosition 			= lockPosition;
			m_Configurable.OffsetPositionLerp 		= lockPositionLerp;
			m_Configurable.OffsetEulerAngles 		= lockRotation;
			m_Configurable.OffsetEulerAnglesLerp 	= lockRotationLerp;
		}
		#endif
		if (m_fResetTime > 0)
		{
			m_fResetTick += Time.deltaTime;
			if (m_fResetTick > m_fResetTime)
			{
				m_fResetTick = m_fResetTime;
			}
		}
		
		if (m_Machine)
			m_Machine.UpdateMachine (Time.deltaTime);
		
		if (m_fResetTick >= m_fResetTime)
		{
			m_fResetTime = -1;
		}
	}

	/// <summary>
	/// Gets the camera.
	/// </summary>
	/// <returns>The camera.</returns>
	public Camera					GetCamera()
	{
		return m_Camera;
	}
	
	/// <summary>
	/// Configurable the specified config.
	/// </summary>
	/// <param name="config">Config.</param>
	public virtual void 			Configurable(CameraSetting config)
	{
		m_Configurable = config;
		
		#if UNITY_EDITOR
		lockPosition 		= m_Configurable.OffsetPosition;
		lockPositionLerp 	= m_Configurable.OffsetPositionLerp;
		lockRotation 		= m_Configurable.OffsetEulerAngles;
		lockRotationLerp 	= m_Configurable.OffsetEulerAnglesLerp;
		#endif
	}
	
	/// <summary>
	/// Gets the configurable.
	/// </summary>
	/// <returns>The configurable.</returns>
	public CameraSetting			GetConfigurable()
	{
		return m_Configurable;
	}
	
	/// <summary>
	/// Gets the euler angle.
	/// </summary>
	/// <returns>The euler angle.</returns>
	public Vector3					GetEulerAngle()
	{
		return transform.eulerAngles;
	}
	
	/// <summary>
	/// Gets the forward.
	/// </summary>
	/// <returns>The forward.</returns>
	public Vector3					GetForward()
	{
		return transform.forward;
	}

	/// <summary>
	/// Gets the direction.
	/// </summary>
	/// <returns>The direction.</returns>
	/// <param name="vDirection">V direction.</param>
	public Vector3					GetDirection(Vector3 vDirection)
	{
		Vector3 d = transform.forward * vDirection.y + transform.right * vDirection.x;
		d.y = 0;
		d.Normalize ();

		return d;
	}

	/// <summary>
	/// Gets the screen.
	/// </summary>
	/// <returns>The screen.</returns>
	/// <param name="vPosition">V position.</param>
	public Vector2					GetScreen(Vector3 vPosition)
	{
		return m_Camera.WorldToScreenPoint(vPosition);
	}
	
	/// <summary>
	/// Gets the target entity.
	/// </summary>
	/// <returns>The target entity.</returns>
	public IEntity					GetTargetEntity()
	{
		return m_TargetEntity;
	}
	
	/// <summary>
	/// Registers the state of the camera.
	/// </summary>
	/// <param name="state">State.</param>
	public void 					RegisterCameraState(IAIState state)
	{
		if (m_Machine)
			m_Machine.RegisterState (state);
	}
	
	/// <summary>
	/// Gets the state of the camera.
	/// </summary>
	/// <returns>The camera state.</returns>
	/// <param name="nStateID">N state I.</param>
	public IAIState					GetCameraState(int nStateID)
	{
		return m_Machine.GetState (nStateID);
	}
	
	/// <summary>
	/// Unregisters the state of the camera.
	/// </summary>
	/// <param name="nStateID">N state I.</param>
	public void 					UnregisterCameraState(int nStateID)
	{
		if (m_Machine)
			m_Machine.RemoveState (nStateID);
	}
	
	/// <summary>
	/// Changes the state of the camera.
	/// </summary>
	/// <param name="nStateID">N state I.</param>
	public void 					ChangeCameraState(int nStateID)
	{
		if (m_Machine)
			m_Machine.ChangeState (nStateID);
	}
	
	/// <summary>
	/// Sets the target.
	/// </summary>
	/// <param name="targetPosition">Target position.</param>
	/// <param name="targetRotation">Target rotation.</param>
	public void 					SetTarget(IEntity targetEntity, Transform targetPosition, Transform targetRotation)
	{
		// save look entity
		m_TargetEntity = targetEntity;
		
		if (m_TargetPosition != targetPosition)
			m_TargetPosition = targetPosition;
		
		if (m_TargetRotation != targetRotation)
			m_TargetRotation = targetRotation;
		
		UpdateRotation (1.0f);
		UpdatePosition (1.0f);
	}
	
	/// <summary>
	/// Updates the offset position.
	/// </summary>
	/// <param name="vOffset">V offset.</param>
	/// <param name="fLerp">F lerp.</param>
	/// <param name="bRayTest">If set to <c>true</c> b ray test.</param>
	public void 					UpdatePosition(float fLerp)
	{
		if (m_TargetPosition)
		{
			Transform target 	= m_Configurable.IsRelative ?  m_TargetRotation : transform;
			Vector3 vPos 		= m_TargetPosition.position + m_Configurable.OffsetPosition.x * target.right + m_Configurable.OffsetPosition.y * target.up - m_Configurable.OffsetPosition.z * target.forward;
			
			vPos.x = -1 == m_Configurable.OffsetPosition.x ? transform.position.x : vPos.x;
			vPos.y = -1 == m_Configurable.OffsetPosition.y ? transform.position.y : vPos.y;
			vPos.z = -1 == m_Configurable.OffsetPosition.z ? transform.position.z : vPos.z;
			
			//RayHitTest(targetPosition.position, ref position);
			transform.position = Vector3.Lerp(transform.position, vPos, CalcResetTime(fLerp));
		}
	}
	
	/// <summary>
	/// Calculates the reset time.
	/// </summary>
	/// <returns>The reset time.</returns>
	/// <param name="fLerp">F lerp.</param>
	public float 					CalcResetTime(float fLerp)
	{
		if (m_fResetTime < 0)
			return fLerp;
		
		if (m_fResetTick >= m_fResetTime)
			return 1.0f;
		
		return m_fResetTick / m_fResetTime;
	}
	
	/// <summary>
	/// Raies the hit test.
	/// </summary>
	/// <returns>The hit test.</returns>
	/// <param name="vPosition">V position.</param>
	/// <param name="vOffset">V offset.</param>
	/// <param name="nLayer">N layer.</param>
	public Vector3  				RayHitTest(Vector3 vPosition, Vector3 vOffset, int nLayer)
	{
		RaycastHit hit;
		if (Physics.Raycast(vPosition, vOffset, out hit, vPosition.magnitude, nLayer))
		{
			return vOffset.normalized * (Vector3.Distance(vPosition, hit.point) - 2 * GetComponent<Camera>().nearClipPlane);
		}
		
		return vOffset;
	}
	
	/// <summary>
	/// Updates the offset rotation.
	/// </summary>
	/// <param name="vOffset">V offset.</param>
	/// <param name="fLerp">F lerp.</param>
	public void 					UpdateRotation(float fLerp)
	{
		Vector3 eulerAngles = m_Configurable.OffsetEulerAngles
			+ (m_Configurable.IsRelative ? m_TargetRotation.eulerAngles : Vector3.zero);
		
		eulerAngles.x = -1 == m_Configurable.OffsetEulerAngles.x ? transform.eulerAngles.x : eulerAngles.x;
		eulerAngles.y = -1 == m_Configurable.OffsetEulerAngles.y ? transform.eulerAngles.y : eulerAngles.y;
		eulerAngles.z = -1 == m_Configurable.OffsetEulerAngles.z ? transform.eulerAngles.z : eulerAngles.z;
		
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(eulerAngles), CalcResetTime(fLerp));
	}
	
	/// <summary>
	/// Reset the specified time.
	/// </summary>
	/// <param name="time">Time.</param>
	public void 					Reset(float fTime)
	{
		m_fResetTime = fTime;
		m_fResetTick = 0.0f;
	}
	
	/// <summary>
	/// Looks the around.
	/// </summary>
	/// <param name="target">Target.</param>
	public void 					LookAround(Vector3 vTarget)
	{
		float fDeltaAngle = Mathf.DeltaAngle(transform.eulerAngles.y, vTarget.y);
		
		if (m_fResetTime < 0)
		{
			float fRotateSpeed = 0;
			CurveTable.GetSingleton().OnCameraLookAroundCurve(fDeltaAngle, ref fRotateSpeed);
			
			fRotateSpeed = (fDeltaAngle > 0 ? 1.0f : -1.0f) * fRotateSpeed;
			transform.eulerAngles += Vector3.up * fRotateSpeed * Time.deltaTime;
		}
		else
		{
			transform.rotation = Quaternion.Lerp(transform.rotation,
			                                     Quaternion.Euler(transform.eulerAngles + Vector3.up * fDeltaAngle), m_fResetTick / m_fResetTime);
		}
	}
}
