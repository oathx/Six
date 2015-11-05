using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Joystick observer.
/// </summary>
public class JoystickObserver : IEventObserver
{
	/// <summary>
	/// Gets the main camera.
	/// </summary>
	/// <value>The main camera.</value>
	public ICameraPlugin	MainCamera
	{ get; private set; }

	/// <summary>
	/// Gets the player mgr.
	/// </summary>
	/// <value>The player mgr.</value>
	public PlayerManager	PlayerMgr
	{ get; private set; }

	/// <summary>
	/// Gets the player.
	/// </summary>
	/// <value>The player.</value>
	public PlayerEntity		MainPlayer
	{ get; private set; }

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		MainCamera 	= GameEngine.GetSingleton().QueryPlugin<ICameraPlugin>();
		if (!MainCamera)
			throw new System.NullReferenceException();

		PlayerMgr	= GameEngine.GetSingleton().QueryPlugin<PlayerManager>();
		if (!PlayerMgr)
			throw new System.NullReferenceException();
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		MainPlayer = PlayerMgr.GetPlayer();
		if (!MainPlayer)
			throw new System.NullReferenceException();
	}

	/// <summary>
	/// Active this instance.
	/// </summary>
	public override void Active ()
	{
		SubscribeEvent(IJoystickPlugin.JOYSTICK_START, 	OnJoystickStart);
		SubscribeEvent(IJoystickPlugin.JOYSTICK_MOVE, 	OnJoystickMove);
		SubscribeEvent(IJoystickPlugin.JOYSTICK_START, 	OnJoystickEnd);
	}

	/// <summary>
	/// Raises the joystick start event.
	/// </summary>
	/// <param name="v">V.</param>
	protected bool	 	OnJoystickStart(IEvent evt)
	{
		return true;
	}

	/// <summary>
	/// Raises the joystick move event.
	/// </summary>
	/// <param name="v">V.</param>
	protected bool	 	OnJoystickMove(IEvent evt)
	{	
		IJoystickPlugin.JoystickMoveArgs v = evt.Args as IJoystickPlugin.JoystickMoveArgs;
		// get current move direction
		Vector3 vDirection 	= MainCamera.GetDirection(v.direction);
		
		float fRotateSpeed 	= MainPlayer.MaxRotateSpeed;
		float fMoveAngle	= MainPlayer.GetMoveAngle();
		
		CurveTable.GetSingleton().OnRotateCurve(fMoveAngle, v.depth, ref fRotateSpeed);

		MainPlayer.RotateTo(
			MathfEx.Direction2Angle(vDirection) * Vector3.up, fRotateSpeed
			);

		return true;
	}

	/// <summary>
	/// Raises the joystick end event.
	/// </summary>
	/// <param name="v">V.</param>
	protected bool	 	OnJoystickEnd(IEvent evt)
	{
		return true;
	}
}
