using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// I joystick touch.
/// </summary>
[RequireComponent(typeof(EasyTouch))]
public class IJoystickPlugin : IGamePlugin
{
	public const int JOYSTICK_START = -12000;
	/// <summary>
	/// Joystick moving event.
	/// </summary>
	public const int JOYSTICK_MOVE 	= -12001;
	/// <summary>
	/// Joystick move end event.
	/// </summary>
	public const int JOYSTICK_END 	= -12002;
	
	/// <summary>
	/// Joystick move arguments.
	/// </summary>
	public class JoystickMoveArgs : IEventArgs
	{
		/// <summary>
		/// Joystick direction.
		/// </summary>
		public Vector3 direction;
		
		/// <summary>
		/// Joystick depth. [0 - 1]
		/// </summary>
		public float depth;
	}
	
	/// <summary>
	/// The easy touch object.
	/// </summary>
	protected EasyTouch			m_dEasyTouch;
	protected Vector3			m_TouchPosition = Vector3.zero;
	protected IJoystickShape	m_JoystickShape;
	protected int 				m_FingerIndex = int.MinValue;
	
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		// add easy touch component
		m_dEasyTouch = GetComponent<EasyTouch> ();
		if (!m_dEasyTouch)
			throw new System.NullReferenceException ();
		
		m_dEasyTouch.enableReservedArea 	= true;
		m_dEasyTouch.useBroadcastMessage 	= false;
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{
		if (m_JoystickShape)
		{
			if (m_JoystickShape.GetVisible())
			{
				m_JoystickShape.UpdateJoystick (m_TouchPosition);
			}
		}
	}
	
	/// <summary>
	/// Startup this instance.
	/// </summary>
	public override void 	Startup()
	{
		Activation();
	}
	
	/// <summary>
	/// Shutdown this instance.
	/// </summary>
	public override void 	Shutdown()
	{
		if (m_JoystickShape)
		{
			UISystem.GetSingleton().UnloadWidget(m_JoystickShape.name);
		}

		Deactivate();
	}

	/// <summary>
	/// Active this instance.
	/// </summary>
	public override void 			Activation()
	{
		if (m_JoystickShape)
			m_JoystickShape.SetVisible(true);

		base.Activation();
		
		// add Touch event
		EasyTouch.On_TouchStart += OnTouchStart;
		EasyTouch.On_TouchDown 	+= OnTouchMove;
		EasyTouch.On_TouchUp 	+= OnTouchUp;
	}
	
	/// <summary>
	/// Dective this instance.
	/// </summary>
	public override void 			Deactivate()
	{
		if (m_JoystickShape)
			m_JoystickShape.SetVisible(false);

		base.Deactivate();

		EasyTouch.On_TouchStart -= OnTouchStart;
		EasyTouch.On_TouchDown 	-= OnTouchMove;
		EasyTouch.On_TouchUp 	-= OnTouchUp;
	}
	
	/// <summary>
	/// Sets the joysticy shape.
	/// </summary>
	/// <param name="szResourceName">Size resource name.</param>
	public virtual void 	SetJoysticyShape(IJoystickShape joystickShape, Vector3 vOffset)
	{
		// destroy current shape
		if (m_JoystickShape)
		{
			UISystem.GetSingleton().UnloadWidget(m_JoystickShape.name);
		}

		m_JoystickShape = joystickShape;

		if (joystickShape)
			joystickShape.SetJoystickPosition(vOffset);
	}

	/// <summary>
	/// Gets the joystick shape.
	/// </summary>
	/// <returns>The joystick shape.</returns>
	public IJoystickShape	GetJoystickShape()
	{
		return m_JoystickShape;
	}
	
	/// <summary>
	/// Sets the visible.
	/// </summary>
	/// <param name="bVisible">If set to <c>true</c> b visible.</param>
	public virtual void 	SetVisible(bool bVisible)
	{
		if (m_JoystickShape)
			m_JoystickShape.SetVisible (bVisible);
		
		gameObject.SetActive (bVisible);
	}
	
	/// <summary>
	/// Raises the touch start event.
	/// </summary>
	/// <param name="gesture">Gesture.</param>
	private void 			OnTouchStart(Gesture gesture)
	{
		if (m_JoystickShape)
		{
			m_FingerIndex	= gesture.fingerIndex;
			m_TouchPosition = m_JoystickShape.GetTouchPosition(gesture.position);
		
			// send joystick move end
			SendEvent(
				new IEvent(EngineEventType.EVENT_USER, JOYSTICK_START, new JoystickMoveArgs())
				);

			// begin move shape down button
			m_JoystickShape.Begin(m_TouchPosition);
		}
	}
	
	/// <summary>
	/// Raises the touch down event.
	/// </summary>
	/// <param name="gesture">Gesture.</param>
	private void	 		OnTouchMove(Gesture gesture)
	{
		if (m_JoystickShape && m_FingerIndex == gesture.fingerIndex)
		{
			m_TouchPosition 			= m_JoystickShape.GetTouchPosition(gesture.position);

			Vector3 joystickButtonPos 	= m_JoystickShape.GetDownPosition();
			Vector3 joystickAreaPos 	= m_JoystickShape.GetAreaPosition();
			
			Vector3 vDirection = Vector3.zero;
			if (joystickButtonPos != joystickAreaPos)
			{
				vDirection = (joystickButtonPos - joystickAreaPos).normalized;
			}
			
			float fJoystickDepth = Mathf.Clamp(
				Vector3.Distance(joystickButtonPos, joystickAreaPos) / m_JoystickShape.GetRadius(), 0, 1
				);
			vDirection *= fJoystickDepth;
			
			// send move event
			JoystickMoveArgs v = new JoystickMoveArgs();
			v.direction = vDirection;
			v.depth		= fJoystickDepth;
			
			SendEvent(
				new IEvent(EngineEventType.EVENT_USER, JOYSTICK_MOVE, v)
				);
		}
	}
	
	/// <summary>
	/// Raises the touch up event.
	/// </summary>
	/// <param name="gesture">Gesture.</param>
	private void 			OnTouchUp(Gesture gesture)
	{
		if (m_JoystickShape && m_FingerIndex == gesture.fingerIndex)
		{
			m_TouchPosition 	= Vector3.zero;
			m_FingerIndex		= int.MinValue;

			m_JoystickShape.End();
			
			// send joystick move end
			SendEvent(
				new IEvent(EngineEventType.EVENT_USER, JOYSTICK_END, new JoystickMoveArgs())
				);
		}
	}
}

