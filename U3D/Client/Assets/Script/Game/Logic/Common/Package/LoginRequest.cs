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
	/// Gets the device info.
	/// </summary>
	/// <returns>The device info.</returns>
	protected string 	GetDeviceInfo()
	{
		return SystemInfo.operatingSystem  + "$" +  SystemInfo.processorCount + "$" +  SystemInfo.processorType + "$" +  SystemInfo.systemMemorySize;
	}
	
	/// <summary>
	/// Gets the device identifier.
	/// </summary>
	/// <returns>The device identifier.</returns>
	protected string 	GetDeviceID()
	{
		return SystemInfo.deviceUniqueIdentifier  + "$" +  SystemInfo.deviceModel + "$" +  SystemInfo.deviceType;
	}
	
	/// <summary>
	/// Requests the login.
	/// </summary>
	/// <param name="szUserName">Size user name.</param>
	/// <param name="szPassword">Size password.</param>
	/// <param name="nServerID">N server I.</param>
	public void RequestLogin(int nServerVersion, int nServerID, string szUserName, string szPassword)
	{
		m_pPlugin.SendEvent(TcpEvent.CMD_REQ_LOGIN, nServerVersion, szUserName, szPassword, nServerID);
	}	

	/// <summary>
	/// Registers the account.
	/// </summary>
	/// <param name="szUserName">Size user name.</param>
	/// <param name="szPassword">Size password.</param>
	public bool RequestRegisterAccount(string szUserName, string szPassword, int nServerID)
	{
		string deviceId 	= GetDeviceID();
		string deviceInfo 	= GetDeviceInfo();
		
		m_pPlugin.SendEvent (TcpEvent.CMD_REQ_REGISTER_ACCOUNT, szUserName, szPassword,
		                     deviceId, deviceInfo, nServerID);

		return true;
	}
}

