using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// User interface version.
/// </summary>
public class UITitle : IUIWidget
{
	protected Camera		m_uiCamera;
	protected Transform		m_Target;
		
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		m_uiCamera = UISystem.GetSingleton().GetCamera();
		if (!m_uiCamera)
			throw new System.NullReferenceException();
	}

	/// <summary>
	/// Sets the target.
	/// </summary>
	/// <param name="target">Target.</param>
	public void 	SetTarget(Transform target)
	{
		m_Target = target;
	}

	/// <summary>
	/// Sets the position.
	/// </summary>
	/// <param name="vPosition">V position.</param>
	public bool 	UpdateScreenPosition(Vector2 vScreen)
	{
		if (!m_uiCamera || !m_Target)
			return false;

		Vector3 vResult = Vector3.zero;

		RectTransformUtility.ScreenPointToWorldPointInRectangle(
			transform as RectTransform, vScreen, m_uiCamera, out vResult
			);
		
		transform.position = vResult;

		return true;
	}
}
