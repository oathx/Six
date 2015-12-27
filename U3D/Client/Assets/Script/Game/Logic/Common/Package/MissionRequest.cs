using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Mission request.
/// </summary>
public class MissionRequest : ScriptableSingleton<MissionRequest>
{
	/// <summary>
	/// The m_ session.
	/// </summary>
	protected LogicPlugin	m_pPlugin;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="LoginRequest"/> class.
	/// </summary>
	protected MissionRequest()
	{
		m_pPlugin = GameEngine.GetSingleton ().QueryPlugin<LogicPlugin> ();
		if (!m_pPlugin)
			throw new System.NullReferenceException ();
	}

	/// <summary>
	/// Reqs the mission list.
	/// </summary>
	public void ReqMissionList()
	{
		m_pPlugin.SendEvent(TcpEvent.CMD_REQ_MISSION_LIST);
	}

	/// <summary>
	/// Requests the accept mission.
	/// </summary>
	/// <param name="nMissionID">N mission I.</param>
	public void ReqAcceptMission(int nMissionID)
	{
		m_pPlugin.SendEvent(TcpEvent.CMD_REQ_ACCEPT_MISSION, nMissionID);
	}
}
