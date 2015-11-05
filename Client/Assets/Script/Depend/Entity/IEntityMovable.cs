using UnityEngine;
using System.Collections;

/// <summary>
/// I entity movable.
/// </summary>
[RequireComponent(typeof(IEntityMotor))]
public class IEntityMovable : IEntity
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
	public virtual Vector3		GetMoveSpeed()
	{
		return Velocity;
	}

	/// <summary>
	/// Sets the enabled.
	/// </summary>
	/// <param name="enabled">If set to <c>true</c> enabled.</param>
	public void 				SetEnabled(bool enabled)
	{
		MoveableController.enabled = enabled;
	}

	/// <summary>
	/// Gets the move angle.
	/// </summary>
	/// <returns>The move angle.</returns>
	public virtual float		GetMoveAngle()
	{
		return Mathf.DeltaAngle(MathfEx.Direction2Angle(transform.forward), MathfEx.Direction2Angle(GetMoveSpeed()));
	}
	
	/// <summary>
	/// Get horizontal move speed.
	/// </summary>
	public virtual float		GetHorizontalSpeed()
	{
		Vector3 vSpeed = GetMoveSpeed ();
		return new Vector3 (vSpeed.x, 0, vSpeed.y).magnitude;
	}
	
	/// <summary>
	/// Get vertical move speed.
	/// </summary>
	public virtual float		GetVerticalSpeed()
	{
		return GetMoveSpeed().y;
	}
	
	/// <summary>
	/// Adds the velocity.
	/// </summary>
	/// <param name="velocity">Velocity.</param>
	public virtual void 		AddVelocity(Vector3 velocity)
	{
		if (Motor)
		{
			Motor.SetVelocity (velocity);
		}
	}

}
