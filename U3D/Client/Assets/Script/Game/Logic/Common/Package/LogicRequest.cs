using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Login request.
/// </summary>
public class LogicRequest : SimpleSingleton<LogicRequest>
{
	/// <summary>
	/// The m_ session.
	/// </summary>
	protected LogicPlugin	m_pPlugin;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="LoginRequest"/> class.
	/// </summary>
	public LogicRequest()
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
			TcpEvent.CMD_REQ_CHARACTER_LIST, bVirtual, nUserID, GlobalUserInfo.LoginTime, GlobalUserInfo.LoginCode
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
	/// Requests the enter level.
	/// </summary>
	public void RequestEnterLevel(int nLevelID)
	{
		m_pPlugin.SendEvent(TcpEvent.CMD_REQ_ENTER_LEVEL, !m_pPlugin.Connected, nLevelID);
	}
	
	/// <summary>
	/// Requests the leave.
	/// </summary>
	public void RequestLeaveLevel()
	{
		m_pPlugin.SendEvent(TcpEvent.CMD_REQ_LEAVE_LEVEL, !m_pPlugin.Connected);
	}

	/// <summary>
	/// Reqs the change scene.
	/// </summary>
	/// <param name="nSceneID">N scene I.</param>
	/// <param name="nScriptID">N script I.</param>
	public void ReqChangeScene(int nSceneID, int nScriptID)
	{
		m_pPlugin.SendEvent(TcpEvent.CMD_REQ_CHANGE_SCENE, !m_pPlugin.Connected, nSceneID, nScriptID);
	}

	/// <summary>
	/// Reqs the team.
	/// </summary>
	/// <param name="nPlayerID">N player I.</param>
	public void ReqTeam(int nPlayerID)
	{
		m_pPlugin.SendEvent(TcpEvent.CMD_REQ_TEAM, !m_pPlugin.Connected, nPlayerID);
	}
}

