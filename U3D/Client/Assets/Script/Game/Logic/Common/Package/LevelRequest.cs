using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Login request.
/// </summary>
public class LevelRequest : ScriptableSingleton<LevelRequest>
{
	/// <summary>
	/// The m_ session.
	/// </summary>
	protected LogicPlugin	m_pPlugin;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="LoginRequest"/> class.
	/// </summary>
	protected LevelRequest()
	{
		m_pPlugin = GameEngine.GetSingleton ().QueryPlugin<LogicPlugin> ();
		if (!m_pPlugin)
			throw new System.NullReferenceException ();
	}

	/// <summary>
	/// Requests the enter level.
	/// </summary>
	public void 	RequestEnterLevel(int nLevelID)
	{
		m_pPlugin.SendEvent(TcpEvent.CMD_REQ_ENTER_DUP, nLevelID);
	}

	/// <summary>
	/// Requests the leave.
	/// </summary>
	public void 	RequestLeaveLevel()
	{
		m_pPlugin.SendEvent(TcpEvent.CMD_REQ_LEAVE_DUP);
	}
}

