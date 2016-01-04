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
}
