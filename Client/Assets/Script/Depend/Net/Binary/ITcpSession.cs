using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using KSNET;
using System;

/// <summary>
/// TcpEvent session.
/// </summary>
///
public class ITcpSession : IGamePlugin 
{
	// tcp connected fnish
	public const int TCP_CONNECTFINISH 	= -11001;
	public const int TCP_CONNECTERROR 	= -11002;
	public const int TCP_DISCONNECTED 	= -11003;

	/// <summary>
	/// TcpEvent connect fnished.
	/// </summary>
	public class TcpConnectArgs : IEventArgs
	{
		public TNSocket.Stage	State
		{ get; set; }

		public TNErrorCode		ErrorCode
		{ get; set; }

		public string			Message
		{ get; set; }

		public string			Name
		{ get; set; }
	}
	
	/// <summary>
	/// The d factory.
	/// </summary>
	protected Dictionary<int,
		IPackageFactory> m_dFactory = new Dictionary<int, IPackageFactory>();

	/// <summary>
	/// The tcp socket.
	/// </summary>
	protected TNSocket		m_dSocket;

	/// <summary>
	/// Install this instance.
	/// </summary>
	public override void 	Install()
	{
		m_dSocket = new TNSocket ();
	}
	
	/// <summary>
	/// Unstall this instance.
	/// </summary>
	public override void 	Uninstall()
	{
		if (m_dSocket != null)
		{
			m_dSocket.Close(true);
		}
	}
	
	/// <summary>
	/// Startup this instance.
	/// </summary>
	public override void 	Startup()
	{

	}
	
	/// <summary>
	/// Shutdown this instance.
	/// </summary>
	public override void 	Shutdown()
	{

	}

	/// <summary>
	/// Fixeds the update.
	/// </summary>
	void FixedUpdate()
	{
		TranslatPostEvent ();
		
		if (Connected)
		{
			RecvNetStream ();
		}
	}

	/// <summary>
	/// Registers the net message factory.
	/// </summary>
	/// <param name="nID">N I.</param>
	/// <param name="factory">Factory.</param>
	public void 	RegisterPackageFactory(int nID, IPackageFactory factory)
	{
		if (!m_dFactory.ContainsKey(nID))
		{
#if OPEN_DEBUG_LOG
			Debug.Log("Register package factory " + nID);
#endif
			m_dFactory.Add(nID, factory);
		}
	}
	
	/// <summary>
	/// Unregisters the net message factory.
	/// </summary>
	/// <param name="nID">N I.</param>
	public void 	UnregisterPackageFactory(int nID)
	{
		if (m_dFactory.ContainsKey(nID))
		{
#if OPEN_DEBUG_LOG
			Debug.Log("Unregister package factory " + nID);
#endif
			m_dFactory.Remove(nID);
		}
	}
	
	/// <summary>
	/// Connect the specified ipAddress and nPort.
	/// </summary>
	/// <param name="ipAddress">Ip address.</param>
	/// <param name="nPort">N port.</param>
	public void		Connect(string ipAddress, int nPort)
	{
		m_dSocket.serverName 		= ipAddress;
		m_dSocket.ip 				= ipAddress;
		m_dSocket.port 				= nPort;

		m_dSocket.onConnected 		= OnConnected;
		m_dSocket.onDisconnected 	= OnDisconnected;
		m_dSocket.onError 			= OnError;

		m_dSocket.Connect();
	}

	/// <summary>
	/// Gets a value indicating whether this <see cref="TcpSession"/> is connected.
	/// </summary>
	/// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
	public bool		Connected
	{
		get{
			return m_dSocket.isConnected;
		}
	}
	
	/// <summary>
	/// Disconnect this instance.
	/// </summary>
	public void 	Disconnect ()
	{
		if (m_dSocket != null)
		{
			m_dSocket.Release();
		}
	}

	/// <summary>
	/// Send the specified nMainID and nSubID.
	/// </summary>
	/// <param name="nMainID">N main I.</param>
	/// <param name="nSubID">N sub I.</param>
	public override void 	SendEvent(int nID, params object[] args)
	{
#if OPEN_DEBUG_LOG
		string szMessageString = string.Empty;
		for(int idx=0; idx<args.Length; idx++)
		{
			szMessageString += args[idx].ToString() + " ";
		}

		Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Send Socket Event : " + nID + " params count " + args.Length + " text : " + szMessageString);
#endif

		m_dSocket.SendTcpPacket (nID, args);
	}

	/// <summary>
	/// Recvs the net stream.
	/// </summary>
	public virtual void 	RecvNetStream()
	{
		TNBuffer buffer = null;
		while(m_dSocket.ReceivePacket(out buffer))
		{
			IPackage package = new IPackage(buffer);

			package.StartRead();
			Receive(package);
			package.EndRead();
		}
	}

	/// <summary>
	/// Receive the specified package.
	/// </summary>
	/// <param name="package">Package.</param>
	private void 			Receive(IPackage package)
	{
		int nCmdID = package.GetCmdID();

#if OPEN_DEBUG_LOG
		Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> Recv net message : " + nCmdID + " Length:" + package.GetLength());
#endif

		if (m_dFactory.ContainsKey(nCmdID))
		{
			DispatchEvent (
				new IEvent (EngineEventType.EVENT_NET, nCmdID, m_dFactory[nCmdID].Create(package))
				);
		}
		else
		{
#if OPEN_DEBUG_LOG
			Debug.LogWarning(">>>> Can't find net message factory : " + nCmdID + " Length:" + package.GetLength());
#endif
		}
	}

	
	/// <summary>
	/// Raises the connected event.
	/// </summary>
	/// <param name="socketName">Socket name.</param>
	private void 			OnConnected(string szSocketName)
	{
		TcpConnectArgs v = new TcpConnectArgs ();
		v.ErrorCode = TNErrorCode.EMPTY;
		v.Message 	= string.Empty;
		v.Name 		= szSocketName;
		v.State 	= TNSocket.Stage.Connected;

		m_dSocket.noDelay = true;

		PostEvent (new IEvent (EngineEventType.EVENT_USER, TCP_CONNECTFINISH, v));
	}
	
	/// <summary>
	/// Raises the disconnected event.
	/// </summary>
	/// <param name="socketName">Socket name.</param>
	/// <param name="errorCode">Error code.</param>
	private void 			OnDisconnected(string szSocketName, TNErrorCode errorCode)
	{
		TcpConnectArgs v = new TcpConnectArgs ();
		v.ErrorCode = errorCode;
		v.Message 	= string.Empty;
		v.Name 		= szSocketName;
		v.State 	= TNSocket.Stage.NotConnected;
		
		PostEvent (new IEvent (EngineEventType.EVENT_USER, TCP_DISCONNECTED, v));
	}
	
	/// <summary>
	/// Raises the error event.
	/// </summary>
	/// <param name="socketName">Socket name.</param>
	private void 			OnError(string szSocketName, string message)
	{
		TcpConnectArgs v = new TcpConnectArgs ();
		v.ErrorCode = TNErrorCode.DISCONNECT;
		v.Message 	= message;
		v.Name 		= szSocketName;
		v.State 	= TNSocket.Stage.NotConnected;
		
		PostEvent (new IEvent (EngineEventType.EVENT_USER, TCP_CONNECTERROR, v));
	}
}



