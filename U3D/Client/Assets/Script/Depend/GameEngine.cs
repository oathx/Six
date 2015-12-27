using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Game engine.
/// </summary>
public class GameEngine : ScriptableSingleton<GameEngine>
{
	/// <summary>
	/// The m_d plugin.
	/// </summary>
	protected Dictionary<string, 
		IGamePlugin> m_dPlugin = new Dictionary<string, IGamePlugin>();

	/// <summary>
	/// The m_ engine.
	/// </summary>
	protected GameObject gameEngine;

	/// <summary>
	/// The m_d hook.
	/// </summary>
	protected Dictionary<int, EventCallback> 
		m_dHook = new Dictionary<int, EventCallback>();

	/// <summary>
	/// Startup this instance.
	/// </summary>
	public void 		Startup()
	{
		gameEngine = new GameObject(typeof(GameEngine).Name);
		if (gameEngine)
			GameObject.DontDestroyOnLoad(gameEngine);

		InstallPlugin();
	}

	/// <summary>
	/// Installs the plugin.
	/// </summary>
	public void 		InstallPlugin()
	{
		LoadPlugin<IGlobalPlugin>();
		LoadPlugin<IResourceManager>();
		LoadPlugin<IEntityManager>();
		LoadPlugin<IPlayerSyncManager>();
		LoadPlugin<IMonsterSyncManager>();
	}

	/// <summary>
	/// Shutdown this instance.
	/// </summary>
	public void 		Shutdown()
	{
		UnloadPlugin<IResourceManager>();
		UnloadPlugin<IEntityManager>();
		UnloadPlugin<IGlobalPlugin>();
		UnloadPlugin<IPlayerSyncManager>();
		UnloadPlugin<IMonsterSyncManager>();
	}

	/// <summary>
	/// Loads the plugin.
	/// </summary>
	/// <returns>The plugin.</returns>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public T 			LoadPlugin<T>() where T : IGamePlugin
	{
		string szPluginName = typeof(T).Name;
		
		if (m_dPlugin.ContainsKey (szPluginName))
			return m_dPlugin [szPluginName] as T;
		
		GameObject goPlugin = new GameObject (szPluginName);
		if (!goPlugin)
			throw new System.NullReferenceException (szPluginName);
		
		goPlugin.transform.position = Vector3.zero;
		goPlugin.transform.parent	= gameEngine.transform;
		
		T cmp = goPlugin.AddComponent<T>();
		if (!cmp)
			throw new System.NullReferenceException (szPluginName);
		
		// add the plugin object
		m_dPlugin.Add(szPluginName, cmp);
		
#if UNITY_EDITOR
		Debug.Log("**************************** Load plugin : " + szPluginName);
#endif
		// call plugin first time init
		cmp.Install();
		
		return cmp;
	}

	/// <summary>
	/// Queries the plugin.
	/// </summary>
	/// <returns>The plugin.</returns>
	/// <param name="szName">Size name.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public T 			QueryPlugin<T>() where T : IGamePlugin
	{
		string szPluginName = typeof(T).Name;
		if (!m_dPlugin.ContainsKey (szPluginName))
			throw new System.NullReferenceException ("Can't find plugin : " + szPluginName);
		
		return (T)(m_dPlugin [typeof(T).Name]);
	}
	
	/// <summary>
	/// Queries the plugin.
	/// </summary>
	/// <returns>The plugin.</returns>
	/// <param name="szPluginName">Size plugin name.</param>
	public IGamePlugin	QueryPlugin(string szPluginName)
	{
		return m_dPlugin [szPluginName];
	}
	
	/// <summary>
	/// Unloads the plugin.
	/// </summary>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public void 		UnloadPlugin<T>() where T : IGamePlugin
	{
		string szPluginName = typeof(T).Name;
		
		if (m_dPlugin.ContainsKey(szPluginName))
		{
#if UNITY_EDITOR
			Debug.Log("**************************** Unload plugin : " + szPluginName);
#endif
			m_dPlugin[szPluginName].Uninstall();
			
			// destroy the plugin gameobject
			GameObject.Destroy(
				m_dPlugin[szPluginName].gameObject
				);
			
			// remove the plugin
			m_dPlugin.Remove(szPluginName);
		}
	}

	/// <summary>
	/// Registers the hook event.
	/// </summary>
	/// <param name="nID">N I.</param>
	/// <param name="hook">Hook.</param>
	public void 		RegisterHookEvent(int nID, EventCallback hook)
	{
		if (!m_dHook.ContainsKey(nID))
		{
			m_dHook.Add(nID, hook);
		}
	}
	
	/// <summary>
	/// Unregisters the hook event.
	/// </summary>
	/// <param name="nID">N I.</param>
	public void 		UnregisterHookEvent(int nID)
	{
		if (m_dHook.ContainsKey(nID))
		{
			m_dHook.Remove(nID);
		}
	}
	/// <summary>
	/// Posts the event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	public void 		PostEvent(IEvent evt)
	{
		if (m_dHook.ContainsKey(evt.ID))
		{
			m_dHook[evt.ID](evt);
		}
		else
		{
			foreach (KeyValuePair<string, IGamePlugin> it in m_dPlugin)
			{
				bool bReuslt = it.Value.SendEvent(evt);
				if (bReuslt)
					break;
			}
		}
	}
	
	/// <summary>
	/// Sends the event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	public void 		SendEvent(IEvent evt)
	{
		if (m_dHook.ContainsKey(evt.ID))
		{
			m_dHook[evt.ID](evt);
		}
		else
		{
			foreach (KeyValuePair<string, IGamePlugin> it in m_dPlugin)
			{
				bool bReuslt = it.Value.SendEvent(evt);
				if (bReuslt)
					break;
			}
		}
	}

	/// <summary>
	/// Sends the event.
	/// </summary>
	/// <param name="szPluginName">Size plugin name.</param>
	/// <param name="evt">Evt.</param>
	public void 		SendEvent(string szPluginName, IEvent evt)
	{
		if (m_dHook.ContainsKey(evt.ID))
		{
			m_dHook[evt.ID](evt);
		}
		else
		{
			if (m_dPlugin.ContainsKey(szPluginName))
			{
				m_dPlugin[szPluginName].SendEvent(evt);
			}
		}
	}

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	public void OnEnable ()
	{
		Debug.Log("**************************** Game Engine startup ****************************");
	}
	
	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	public void OnDestroy()
	{
		Debug.Log("**************************** Game Engine shutdown ****************************");
	}
}
