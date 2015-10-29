using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Login request.
/// </summary>
public class LoginRequest : ScriptableSingleton<LoginRequest>
{
	/// <summary>
	/// The m_ session.
	/// </summary>
	protected LoginPlugin	m_pPlugin;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="LoginRequest"/> class.
	/// </summary>
	protected LoginRequest()
	{
		m_pPlugin = GameEngine.GetSingleton ().QueryPlugin<LoginPlugin> ();
		if (!m_pPlugin)
			throw new System.NullReferenceException ();
	}
	
	/// <summary>
	/// Requests the new server.
	/// </summary>
	public void RequestNewServer()
	{
		m_pPlugin.SendEvent (TcpEvent.CMD_REQ_DEFAULT_SERER_INFO);
	}
	
	/// <summary>
	/// Requests the login.
	/// </summary>
	/// <param name="szUserName">Size user name.</param>
	/// <param name="szPassword">Size password.</param>
	/// <param name="nServerID">N server I.</param>
	public void RequestLogin(string szUserName, string szPassword, int nServerID, int nVersion)
	{
		m_pPlugin.SendEvent(TcpEvent.CMD_REQ_LOGIN, nVersion, szUserName, szPassword, nServerID);
	}	
}

