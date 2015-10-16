using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Logic plugin.
/// </summary>
public class LogicPlugin : ITcpSession
{
	protected ServerPlugin		m_Server;
	
	/// <summary>
	/// Install this instance.
	/// </summary>
	public override void 		Install()
	{
		// init local server
		m_Server = GameEngine.GetSingleton().QueryPlugin<ServerPlugin>();
		if (!m_Server)
			throw new System.NullReferenceException();
		
		// init socket object
		base.Install ();
	}
	
	/// <summary>
	/// Unstall this instance.
	/// </summary>
	public override void 		Uninstall()
	{
		// clear the plugin resource and observer
		Shutdown ();
		
		// supper call
		base.Uninstall ();
	}
	
	/// <summary>
	/// Startup this instance.
	/// </summary>
	public override void 		Startup()
	{
		
	}
	
	/// <summary>
	/// Shutdown this instance.
	/// </summary>
	public override void 		Shutdown()
	{
		
	}
	
	/// <summary>
	/// Sends the event.
	/// </summary>
	/// <param name="">.</param>
	/// <param name="args">Arguments.</param>
	public virtual bool 		SendEvent(int nID, params object[] args)
	{
#if LOCAL_SERVER
		// If the current start the virtual server, then the network message is sent to the virtual server processing 
		if (m_Server)
		{
			m_Server.SendEvent(
				new IEvent(EngineEventType.EVENT_NET, nID, new VirtualNetPackage(args))
				);
			
			return true;
		}
		
#else
		base.SendEvent(nID, args);
#endif
		
		return true;
	}
}
