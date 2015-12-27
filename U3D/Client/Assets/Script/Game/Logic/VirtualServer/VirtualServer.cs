using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Login server.
/// </summary>
public class VirtualServer : IEventObserver
{
	protected LogicPlugin	m_Plugin;
	
	/// <summary>
	/// Awake this instance.
	/// </summary>
	protected virtual void Awake ()
	{
		m_Plugin = GameEngine.GetSingleton ().QueryPlugin<LogicPlugin> ();
		if (!m_Plugin)
			throw new System.NullReferenceException ();
	}
}


