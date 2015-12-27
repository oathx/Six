using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Player entity.
/// </summary>
public class BaseUnitEntity : ICharacterEntity
{
	protected ICameraPlugin	m_WorldCamera;
	protected UITitle		m_Title;
	
	/// <summary>
	/// Awake this instance.
	/// </summary>
	protected override void Awake()
	{
		base.Awake();
		
		m_WorldCamera = GameEngine.GetSingleton().QueryPlugin<ICameraPlugin>();
		if (!m_WorldCamera)
			throw new System.NullReferenceException();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	protected override void Update()
	{
		base.Update();
		
		UpdateTitle();
	}
	
	/// <summary>
	/// Creates the title.
	/// </summary>
	/// <returns><c>true</c>, if title was created, <c>false</c> otherwise.</returns>
	/// <param name="nID">N I.</param>
	/// <param name="nStyle">N style.</param>
	/// <param name="szEntityName">Size entity name.</param>
	public void				CreateTitle(int nID, int nStyle, string szEntityName)
	{
		m_Title = UISystem.GetSingleton().LoadWidget<UITitle>(
			ResourceDef.UI_TITLE, true, nID.ToString()
			);
		if (m_Title)
		{
			m_Title.SetTarget(transform);
		}
	}
	
	/// <summary>
	/// Updates the title.
	/// </summary>
	/// <returns><c>true</c>, if title was updated, <c>false</c> otherwise.</returns>
	public bool				UpdateTitle()
	{
		if (!m_WorldCamera || !m_Title)
			return false;
		
		Vector3 vPosition 	= GetPosition();
		
		Vector2 vScreen 	= m_WorldCamera.GetScreen(vPosition + Vector3.up * 2.0f);
		if (m_Title)
			m_Title.UpdateScreenPosition(vScreen);
		
		return true;
	}
	
	/// <summary>
	/// Destroies the title.
	/// </summary>
	public void				DestroyTitle()
	{
		if (m_Title)
		{
			UISystem.GetSingleton().UnloadWidget(m_Title.name);
		}
	}
}
