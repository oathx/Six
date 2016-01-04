using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Login observer.
/// </summary>
public class GateConnectObserver : IEventObserver
{
	protected LogicPlugin	m_pLogic;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	protected void 		Awake()
	{
		m_pLogic = GameEngine.GetSingleton ().QueryPlugin<LogicPlugin> ();
		if (m_pLogic)
		{
			m_pLogic.RegisterPackageFactory(TcpEvent.CMD_REPLY_CHARACTER_LIST, 
			                                new DefaultNetMessageFactory<TcpEvent.SCNetCharacterListReply>());
			m_pLogic.RegisterPackageFactory(TcpEvent.CMD_REPLY_ERROR, 
			                                new DefaultNetMessageFactory<TcpEvent.SCNetErrorReply>());
		}

		// register session local event
		SubscribeEvent (ITcpSession.TCP_CONNECTFINISH, 	OnTcpConnectFinish);
		SubscribeEvent (ITcpSession.TCP_DISCONNECTED, 	OnTcpConnectError);
		SubscribeEvent (ITcpSession.TCP_CONNECTERROR, 	OnTcpDisconnected);
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	protected void 		Start()
	{
		SubscribeEvent (TcpEvent.CMD_REPLY_CHARACTER_LIST,	new EventCallback(OnNetCharacterList));
		SubscribeEvent (TcpEvent.CMD_REPLY_ERROR,   		new EventCallback(OnNetError));
	}

	/// <summary>
	/// Active this instance.
	/// </summary>
	public override void Active()
	{
		UISystem.GetSingleton().ShowStatus(
			LogicHelper.GetErrorText(ErrorCode.ERR_CONNECTING)
			);

		// connect to gate server
		try
		{
			m_pLogic.Connect(GlobalUserInfo.GateIPAddress, GlobalUserInfo.GatePort);
		}
		catch(System.Exception e)
		{
			UISystem.GetSingleton().Box(e.Message);
		}
	}

	/// <summary>
	/// Requests the character list.
	/// </summary>
	/// <returns><c>true</c>, if character list was requested, <c>false</c> otherwise.</returns>
	private bool		RequestCharacterList()
	{
		LogicRequest.GetSingleton().RequesetCharacterList(GlobalUserInfo.UserID);
		return true;
	}

	/// <summary>
	/// Raises the tcp connect fnish event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	private bool 		OnTcpConnectFinish(IEvent evt)
	{
		UISystem.GetSingleton().HideStatus();

		// connect to gate server success, request all character info
		return RequestCharacterList();
	}

	/// <summary>
	/// Raises the tcp disconnected event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	private bool 		OnTcpDisconnected(IEvent evt)
	{
		UISystem.GetSingleton().HideStatus();

		// connect to gate server success, request all character info
		return RequestCharacterList();
	}

	/// <summary>
	/// Raises the tcp connect error event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	private bool 		OnTcpConnectError(IEvent evt)
	{
		UISystem.GetSingleton().HideStatus();

		// connect to gate server success, request all character info
		return RequestCharacterList();
	}

	/// <summary>
	/// Raises the push character info list event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	private bool		OnNetCharacterList(IEvent evt)
	{
		// character default resource
		List<string> 
			defaultResource = new List<string>();
		
		TcpEvent.SCNetCharacterListReply v = evt.Args as TcpEvent.SCNetCharacterListReply;
		foreach(TcpEvent.CharacterStruct info in v.List)
		{
			CharacterTable.GetSingleton().Add(info.PlayerID, info);
		}
	
		return LogicHelper.ChangeScene(SceneFlag.SCENE_CHARACTER);
	}

	/// <summary>
	/// Raises the push error event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	private bool		OnNetError(IEvent evt)
	{
		TcpEvent.SCNetErrorReply error = evt.Args as TcpEvent.SCNetErrorReply;

		UISystem.GetSingleton().Box(error.ErrorCode.ToString());
		return true;
	}
}
