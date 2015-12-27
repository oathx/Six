using UnityEngine;
using System.Collections;

/// <summary>
/// I entity movable.
/// </summary>
[RequireComponent(typeof(IEntityMotor))]
public class IEntityMovable : IEntityShape
{
	public CharacterController	MoveableController
	{ get; private set; }

	/// <summary>
	/// Gets the motor.
	/// </summary>
	/// <value>The motor.</value>
	public IEntityMotor			Motor
	{ get; private set; }

	/// <summary>
	/// Gets the last position.
	/// </summary>
	/// <value>The last position.</value>
	public Vector3				LastPosition
	{ get; private set; }

	/// <summary>
	/// Gets the velocity.
	/// </summary>
	/// <value>The velocity.</value>
	public Vector3				Velocity
	{ get; private set; }

	/// <summary>
	/// Awake this instance.
	/// </summary>
	protected override void Awake()
	{
		MoveableController = GetComponent<CharacterController>();
		if (!MoveableController)
			MoveableController = gameObject.AddComponent<CharacterController>();

		// init entity controller motor
		Motor = GetComponent<IEntityMotor>();
		if (!Motor)
			throw new System.NullReferenceException();
		// open fixed update,
		Motor.useFixedUpdate = true;

		base.Awake();
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	protected override void 	Start()
	{
		Motor.movement.maxGroundAcceleration = Motor.movement.maxAirAcceleration = float.MaxValue;
		Motor.useFixedUpdate 			= false;
		Motor.jumping.enabled 			= false;
		Motor.movingPlatform.enabled 	= false;
		Motor.sliding.enabled 			= false;
		Motor.movement.gravity 			= 90.0f;
		Motor.movement.maxFallSpeed 	= 100.0f;
		
		LastPosition = GetPosition();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	protected override void 	Update()
	{
		// calc velocity
		Vector3 vPosition 	= GetPosition ();
		
		// update velocity
		Velocity 			= (vPosition - LastPosition) / Time.deltaTime;
		LastPosition 		= vPosition;
		
		base.Update ();
	}
	/// <summary>
	/// Gets the move speed.
	/// </summary>
	/// <returns>The move speed.</returns>
	public Vector3		GetMoveSpeed()
	{
		return Velocity;
	}

	/// <summary>
	/// Sets the enabled.
	/// </summary>
	/// <param name="enabled">If set to <c>true</c> enabled.</param>
	public void 		SetMoveable(bool enabled)
	{
		MoveableController.enabled = enabled;
	}

	/// <summary>
	/// Gets the move angle.
	/// </summary>
	/// <returns>The move angle.</returns>
	public float		GetMoveAngle()
	{
		return Mathf.DeltaAngle(MathfEx.Direction2Angle(transform.forward), MathfEx.Direction2Angle(GetMoveSpeed()));
	}
	
	/// <summary>
	/// Get horizontal move speed.
	/// </summary>
	public float		GetHorizontalSpeed()
	{
		Vector3 vSpeed = GetMoveSpeed ();
		return new Vector3 (vSpeed.x, 0, vSpeed.y).magnitude;
	}
	
	/// <summary>
	/// Get vertical move speed.
	/// </summary>
	public float		GetVerticalSpeed()
	{
		return GetMoveSpeed().y;
	}

	/// <summary>
	/// Gets the size of the bound.
	/// </summary>
	/// <returns>The bound size.</returns>
	public Vector3		GetBoundSize()
	{
		return MoveableController.bounds.size;
	}

	/// <summary>
	/// Gets the distance.
	/// </summary>
	/// <returns>The distance.</returns>
	/// <param name="target">Target.</param>
	public float		GetDistance(IEntity target)
	{
		return Vector3.Distance(GetPosition(), target.GetPosition());
	}
	
	/// <summary>
	/// Adds the velocity.
	/// </summary>
	/// <param name="velocity">Velocity.</param>
	public void 		AddVelocity(Vector3 velocity)
	{
		if (Motor)
		{
			Motor.SetVelocity (velocity);
		}
	}

	/// <summary>
	/// Move the specified vPosition and fSpeed.
	/// </summary>
	/// <param name="vPosition">V position.</param>
	/// <param name="fSpeed">F speed.</param>
	public virtual void Move(Vector3 vPosition, float fSpeed)
	{
		Vector3 direction = vPosition - GetPosition();
		
		if (Vector3.zero == direction || 0 == fSpeed)
		{
			Motor.inputMoveDirection = Vector3.zero;
		}
		else
		{
			Motor.inputMoveDirection = direction.normalized;
			Motor.movement.maxForwardSpeed = Motor.movement.maxSidewaysSpeed = Motor.movement.maxBackwardsSpeed = fSpeed;
			
			if (Vector3.zero == GetMoveSpeed())
			{
				MoveableController.enabled = false;
				MoveableController.enabled = true;
			}
		}
	}
}
