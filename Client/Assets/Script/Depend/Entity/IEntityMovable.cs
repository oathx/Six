using UnityEngine;
using System.Collections;

/// <summary>
/// I entity movable.
/// </summary>
[RequireComponent(typeof(IEntityMotor))]
public class IEntityMovable : IEntity
{
	protected IEntityMotor			m_Motor;
	protected CharacterController	m_Controller;
	protected Vector3				m_Velocity = Vector3.zero;
	protected Vector3 				m_LastPosition = Vector3.zero;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	protected virtual void 	Awake()
	{
		// get control
		m_Controller = GetComponent<CharacterController> ();
		if (!m_Controller)
			m_Controller = gameObject.AddComponent<CharacterController>();

		// move motor
		m_Motor = GetComponent<IEntityMotor> ();
		if (!m_Motor)
			throw new System.NullReferenceException ();
		
		m_Motor.useFixedUpdate = true;
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	protected virtual void 	Start ()
	{
		m_Motor.movement.maxGroundAcceleration = m_Motor.movement.maxAirAcceleration = float.MaxValue;
		m_Motor.useFixedUpdate 			= false;
		m_Motor.jumping.enabled 		= false;
		m_Motor.movingPlatform.enabled 	= false;
		m_Motor.sliding.enabled 		= false;
		m_Motor.movement.gravity 		= 90.0f;
		m_Motor.movement.maxFallSpeed 	= 100.0f;
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	protected virtual void Update()
	{

	}

	/// <summary>
	/// Gets the move speed.
	/// </summary>
	/// <returns>The move speed.</returns>
	public virtual Vector3	GetMoveSpeed()
	{
		return m_Velocity;
	}
	
	/// <summary>
	/// Gets the radius.
	/// </summary>
	/// <returns>The radius.</returns>
	public virtual float	GetRadius()
	{
		return m_Controller.radius;
	}
	
	/// <summary>
	/// Gets the size of the bound.
	/// </summary>
	/// <returns>The bound size.</returns>
	public virtual Vector3	GetBoundSize()
	{
		return m_Controller.bounds.size;
	}
	
	/// <summary>
	/// Sets the capsule.
	/// </summary>
	/// <param name="vCenter">V center.</param>
	/// <param name="fRadius">F radius.</param>
	/// <param name="fHight">F hight.</param>
	/// <param name="fSlope">F slope.</param>
	/// <param name="fStep">F step.</param>
	public virtual void 	SetCapsule(Vector3 vCenter, float fRadius, float fHight, float fSlope, float fStep)
	{
		m_Controller.center	 	= vCenter;
		m_Controller.radius 	= fRadius;
		m_Controller.height 	= fHight;
		m_Controller.slopeLimit = fSlope;
		m_Controller.stepOffset = fStep;
	}
	
	/// <summary>
	/// Move the specified vPosition and fSpeed.
	/// </summary>
	/// <param name="vPosition">V position.</param>
	/// <param name="fSpeed">F speed.</param>
	public virtual void 	Move(Vector3 vPosition, float fSpeed)
	{
		Vector3 direction = vPosition - GetPosition();
		
		if (Vector3.zero == direction || 0 == fSpeed)
		{
			m_Motor.inputMoveDirection = Vector3.zero;
		}
		else
		{
			m_Motor.inputMoveDirection = direction.normalized;
			m_Motor.movement.maxForwardSpeed = m_Motor.movement.maxSidewaysSpeed = m_Motor.movement.maxBackwardsSpeed = fSpeed;
			
			if (Vector3.zero == GetMoveSpeed())
			{
				m_Controller.enabled = false;
				m_Controller.enabled = true;
			}
		}
	}
	
	/// <summary>
	/// Gets the move angle.
	/// </summary>
	/// <returns>The move angle.</returns>
	public virtual float	GetMoveAngle()
	{
		return Mathf.DeltaAngle(MathfEx.Direction2Angle(transform.forward), MathfEx.Direction2Angle(GetMoveSpeed()));
	}
	
	/// <summary>
	/// Get horizontal move speed.
	/// </summary>
	public virtual float	GetHorizontalSpeed()
	{
		Vector3 vSpeed = GetMoveSpeed ();
		return new Vector3 (vSpeed.x, 0, vSpeed.y).magnitude;
	}
	
	/// <summary>
	/// Get vertical move speed.
	/// </summary>
	public virtual float	GetVerticalSpeed()
	{
		return GetMoveSpeed().y;
	}
	
	/// <summary>
	/// Adds the velocity.
	/// </summary>
	/// <param name="velocity">Velocity.</param>
	public virtual void 	AddVelocity(Vector3 velocity)
	{
		m_Motor.SetVelocity (velocity);
	}
	
	/// <summary>
	/// Determines whether this instance is grounded.
	/// </summary>
	/// <returns>true</returns>
	/// <c>false</c>
	public virtual bool		IsGrounded()
	{
		return m_Motor.IsGrounded();
	}
}
