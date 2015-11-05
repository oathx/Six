using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Logic plugin.
/// </summary>
public class LogicPlugin : ITcpSession
{
	// local logic server plugin
	protected ServerPlugin		m_LogicServer;
	
	/// <summary>
	/// Install this instance.
	/// </summary>
	public override void 		Install()
	{
		// start virtual server
		m_LogicServer = GameEngine.GetSingleton().LoadPlugin<ServerPlugin>();
		if (!m_LogicServer)
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
		// start logic server
		if (m_LogicServer)
			m_LogicServer.Startup();

		// register gate connect observer
		GateConnectObserver observer = RegisterObserver<GateConnectObserver> (typeof(GateConnectObserver).Name);
		if (observer)
			observer.Active();

		// connect to game server
		if (!string.IsNullOrEmpty(GlobalUserInfo.GateIPAddress))
		{
			Connect(GlobalUserInfo.GateIPAddress, GlobalUserInfo.GatePort);
		}
	}
	
	/// <summary>
	/// Shutdown this instance.
	/// </summary>
	public override void 		Shutdown()
	{
		UnregisterObserver (typeof(GateConnectObserver).Name);
	}
	
	/// <summary>
	/// Sends the event.
	/// </summary>
	/// <param name="">.</param>
	/// <param name="args">Arguments.</param>
	public virtual bool 		SendEvent(int nID, bool bLocalServer, params object[] args)
	{
		if (bLocalServer || !Connected)
		{
			// If the current start the virtual server, then the network message is sent to the virtual server processing 
			if (m_LogicServer)
			{
				m_LogicServer.SendEvent(
					new IEvent(EngineEventType.EVENT_NET, nID, new VirtualNetPackage(args))
					);
			}
		}
		else 
		{
			base.SendEvent(nID, args);
		}
	
		return true;
	}
}
