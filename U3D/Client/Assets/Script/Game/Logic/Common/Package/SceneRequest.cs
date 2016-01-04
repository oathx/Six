using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Login request.
/// </summary>
public class SceneRequest : SimpleSingleton<SceneRequest>
{
	/// <summary>
	/// The m_ session.
	/// </summary>
	protected LogicPlugin	m_pPlugin;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="LoginRequest"/> class.
	/// </summary>
	public SceneRequest()
	{
		m_pPlugin = GameEngine.GetSingleton ().QueryPlugin<LogicPlugin> ();
		if (!m_pPlugin)
			throw new System.NullReferenceException ();
	}

	/// <summary>
	/// Reqs the change scene.
	/// </summary>
	/// <param name="nSceneID">N scene I.</param>
	/// <param name="nScriptID">N script I.</param>
	public void ReqChangeScene(int nSceneID, int nScriptID)
	{
		m_pPlugin.SendEvent(TcpEvent.CMD_REQ_CHANGE_SCENE, nSceneID, nScriptID);
	}
}

