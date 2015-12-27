using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// I event dispatch.
/// </summary>
public class IJoystickShape : IUIWidget
{
	public const string JY_DOWN	= "JY_DOWN";
	public const string JY_AREA	= "JY_AREA";
	
	// current joy state
	protected enum JoyState
	{
		JOY_IDLE	= 0,
		JOY_MOVING	= 1,
	}
	
	/// <summary>
	/// The is moveing.
	/// </summary>
	protected JoyState		m_JoyState = JoyState.JOY_IDLE;

	/// <summary>
	/// The joystick down.
	/// </summary>
	protected Image 		m_JoyDown;
	protected Image			m_JoyArea;
	protected RectTransform	m_JoyBack;
	protected bool			m_bLockArea;

	/// <summary>
	/// The joystick position.
	/// </summary>
	protected Vector3		m_JoyLock = Vector3.zero;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		m_JoyBack = GetComponent<RectTransform>();
		if (!m_JoyBack)
			throw new System.NullReferenceException();

		Install(new string[]{
			JY_DOWN, 
			JY_AREA,
		});

		m_JoyDown = GetChildComponent<Image>(JY_DOWN);
		m_JoyArea = GetChildComponent<Image>(JY_AREA);
	}
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{

	}
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	public void 	Begin(Vector3 vPosition)
	{
		m_JoyDown.transform.localPosition = vPosition;

		if (!m_bLockArea)
			m_JoyArea.transform.localPosition = vPosition;

		// set joystick state
		m_JoyState = JoyState.JOY_MOVING;
	}
	
	/// <summary>
	/// End this instance.
	/// </summary>
	public void 	End()
	{
		m_JoyState = JoyState.JOY_IDLE;
	}
	
	/// <summary>
	/// Gets the radius.
	/// </summary>
	/// <returns>The radius.</returns>
	public float	GetRadius()
	{
		return m_JoyArea.preferredWidth / 2;
	}
	
	/// <summary>
	/// Gets the axis.
	/// </summary>
	/// <returns>The axis.</returns>
	public Vector2 	GetAxis()
	{
		return (m_JoyDown.transform.localPosition - m_JoyArea.transform.localPosition).normalized
			* Mathf.Clamp(Vector3.Distance(m_JoyDown.transform.localPosition, m_JoyArea.transform.localPosition) / GetRadius(), 0, 1);
	}
	
	/// <summary>
	/// Gets the size.
	/// </summary>
	/// <returns>The size.</returns>
	public Vector2	GetSize()
	{
		return new Vector2 (m_JoyBack.rect.width, m_JoyBack.rect.height);
	}
	
	/// <summary>
	/// Gets the button position.
	/// </summary>
	/// <returns>The button position.</returns>
	public Vector3 	GetDownPosition()
	{
		return m_JoyDown.transform.localPosition;
	}
	
	/// <summary>
	/// Gets the area position.
	/// </summary>
	/// <returns>The area position.</returns>
	public Vector3 	GetAreaPosition()
	{
		return m_JoyArea.transform.localPosition;
	}

	/// <summary>
	/// Lock the specified bLockArea.
	/// </summary>
	/// <param name="bLockArea">If set to <c>true</c> b lock area.</param>
	public void 	Lock()
	{
		m_bLockArea = true;
	}

	/// <summary>
	/// Unlock this instance.
	/// </summary>
	public void 	Unlock()
	{
		m_bLockArea	= false;
	}

	/// <summary>
	/// Gets the touch position.
	/// </summary>
	/// <returns>The touch position.</returns>
	/// <param name="vPosition">V position.</param>
	public Vector2 	GetTouchPosition(Vector2 vPosition)
	{
		Vector2 vSize 	= GetSize ();
		Vector2 vPos 	= MathfEx.CoordinateConvert(new Vector2(Screen.width, Screen.height), vSize, vPosition);
		
		vPos.x -= vSize.x / 2;
		vPos.y -= vSize.y / 2;
		
		return vPos;
	}

	/// <summary>
	/// Sets the position.
	/// </summary>
	/// <param name="vPositon">V positon.</param>
	public void		SetJoystickPosition(Vector3 vOffset)
	{
		m_JoyLock = GetTouchPosition(vOffset);
	}
	
	/// <summary>
	/// Updates the joystick.
	/// </summary>
	/// <param name="joysStickPosition">Joys stick position.</param>
	public void 	UpdateJoystick(Vector3 vJoysstickPosition)
	{
		if (m_JoyState == JoyState.JOY_IDLE)
		{
			m_JoyArea.transform.localPosition = Vector3.Lerp(m_JoyArea.transform.localPosition, m_JoyLock, 15 * Time.deltaTime);
			m_JoyDown.transform.localPosition = Vector3.Lerp(m_JoyDown.transform.localPosition, m_JoyLock, 15 * Time.deltaTime);
		}
		else
		{

			float fRadius = GetRadius();
			if (m_bLockArea)
			{
				m_JoyDown.transform.localPosition = vJoysstickPosition;
			}
			else
			{
				if (Vector2.Distance(m_JoyArea.transform.localPosition, m_JoyDown.transform.localPosition) > fRadius)
				{
					m_JoyArea.transform.localPosition = Vector3.Lerp(m_JoyArea.transform.localPosition, 
					                                                 m_JoyDown.transform.localPosition + (m_JoyArea.transform.localPosition - m_JoyDown.transform.localPosition).normalized * fRadius, 
					                                                 15.0f * Time.deltaTime);
				}
			}
		}
	}
}
