using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Login request.
/// </summary>
public class CharacterRequest : SimpleSingleton<CharacterRequest>
{
	/// <summary>
	/// The m_ session.
	/// </summary>
	protected LogicPlugin	m_pPlugin;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="LoginRequest"/> class.
	/// </summary>
	public CharacterRequest()
	{
		m_pPlugin = GameEngine.GetSingleton ().QueryPlugin<LogicPlugin> ();
		if (!m_pPlugin)
			throw new System.NullReferenceException ();
	}
	
	/// <summary>
	/// Requesets the character list.
	/// </summary>
	public void RequesetCharacterList(int nUserID)
	{
		bool bVirtual = !m_pPlugin.Connected;

		// if current no conncet,then send the message to virtual server
		m_pPlugin.SendEvent(
			TcpEvent.CMD_REQ_REGISTER_ROLES, bVirtual,  nUserID, GlobalUserInfo.LoginTime, GlobalUserInfo.LoginCode
			);
	}
	
	/// <summary>
	/// Requests the create role.
	/// </summary>
	/// <param name="szName">Size name.</param>
	/// <param name="occupationNameID">Occupation name I.</param>
	public void RequestCreateRole(string szRoleName, sbyte byOccupationNameID)
	{
		bool bVirtual = !m_pPlugin.Connected;
		
		// if current no conncet,then send the message to virtual server
		m_pPlugin.SendEvent(
			TcpEvent.CMD_REQ_CREATE_ROLE, bVirtual, szRoleName, byOccupationNameID
			);
		
	}
	
	/// <summary>
	/// Requests the enter world.
	/// </summary>
	public void RequestEnterWorld(int nPlayerID)
	{
		bool bVirtual = !m_pPlugin.Connected;

		m_pPlugin.SendEvent(
			TcpEvent.CMD_REQ_ENTER_WORLD, bVirtual, nPlayerID
			);
	}
	
	/// <summary>
	/// Requests the character poperty.
	/// </summary>
	public void RequestCharacterPoperty()
	{
		bool bVirtual = !m_pPlugin.Connected;

		m_pPlugin.SendEvent(
			TcpEvent.CMD_REQ_CHAR_ATTRIBUTE, bVirtual
			);
	}
}
