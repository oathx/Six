using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Login request.
/// </summary>
public class SyncRequest : SimpleSingleton<SyncRequest>
{
	/// <summary>
	/// The m_ session.
	/// </summary>
	protected LogicPlugin	m_pPlugin;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="LoginRequest"/> class.
	/// </summary>
	public SyncRequest()
	{
		m_pPlugin = GameEngine.GetSingleton ().QueryPlugin<LogicPlugin> ();
		if (!m_pPlugin)
			throw new System.NullReferenceException ();
	}
}
