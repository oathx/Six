using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Login observer.
/// </summary>
public class LoginObserver : IEventObserver
{
	public UILogin			LoginUI
	{ get; private set; }

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		SubscribeEvent(ITcpSession.TCP_CONNECTERROR, OnTcpConnectError);
		SubscribeEvent(ITcpSession.TCP_CONNECTFINISH,OnTcpConnectSuccess);
		SubscribeEvent(ITcpSession.TCP_DISCONNECTED, OnTcpDisconnected);
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		SubscribeEvent(CmdEvent.CMD_UI_LOGIN, 		OnLoginEvent);
		SubscribeEvent(CmdEvent.CMD_UI_REGISTER,	OnRegisterEvent);

		// subscribe all net protocol
		SubscribeEvent(TcpEvent.CMD_PUSH_LOGIN_SUCCESS,
		               OnLoginSuccess);
	}

	/// <summary>
	/// Active this instance.
	/// </summary>
	public override void 	Active()
	{
		// create login ui resource
		if (!LoginUI)
			LoginUI = UISystem.GetSingleton().LoadWidget<UILogin>(ResourceDef.UI_LOGIN);
	}
	
	/// <summary>
	/// Detive this instance.
	/// </summary>
	public override void 	Detive()
	{
		// destroy the module ui resource
		if (LoginUI)
			UISystem.GetSingleton().UnloadWidget(ResourceDef.UI_LOGIN);
	}

	/// <summary>
	/// Raises the login event event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool 			OnLoginEvent(IEvent evt)
	{
		CmdEvent.UILoginEventArgs v = evt.Args as CmdEvent.UILoginEventArgs;
		if (string.IsNullOrEmpty(v.UserName) || string.IsNullOrEmpty(v.Password))
		{
			LogicHelper.Error(ErrorCode.ERR_USERNAME);
		}
		else
		{
			LoginPlugin plugin = GameEngine.GetSingleton().QueryPlugin<LoginPlugin>();
			if (plugin)
			{
				plugin.Connect(WUrl.IPAddress, WUrl.Port);
			}
		}

		return true;
	}

	/// <summary>
	/// Raises the register event event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool 			OnRegisterEvent(IEvent evt)
	{
		return true;
	}

	/// <summary>
	/// Raises the tcp connect error event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool			OnTcpConnectError(IEvent evt)
	{
#if LOCAL_SERVER
		UISystem.GetSingleton().Box(BoxStyle.YES, LogicHelper.GetErrorText(ErrorCode.ERR_LOCALCONNECT), 0,
		                            delegate(bool bFlag, object args) {

			string szUserName = LoginUI.GetUserName();
			string szPassword = LoginUI.GetPassword();

			if (!string.IsNullOrEmpty(szUserName) && !string.IsNullOrEmpty(szPassword))
			{
				TcpEvent.SCNetLogin v = new TcpEvent.SCNetLogin();
				v.UserID 	= 0;
				v.GateIP 	= string.Empty;
				v.Port	 	= 0;
				v.LoginCode	= string.Empty;
				v.LoginTime = 0;
				
				Dispatcher.SendEvent(
					new IEvent(EngineEventType.EVENT_NET, TcpEvent.CMD_PUSH_LOGIN_SUCCESS, v)
					);
			}
			return true;
		});
#else
		LogicHelper.Error(ErrorCode.ERR_DISCONNECT);
#endif
		return true;
	}

	/// <summary>
	/// Raises the tcp connect success event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool			OnTcpConnectSuccess(IEvent evt)
	{
		string szUserName = LoginUI.GetUserName();
		string szPassword = LoginUI.GetPassword();

		LoginRequest.GetSingleton().RequestLogin(szUserName, szPassword, 11, 0);

		return true;
	}

	/// <summary>
	/// Raises the tcp disconnected event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool			OnTcpDisconnected(IEvent evt)
	{
		return OnTcpConnectError(evt);
	}

	/// <summary>
	/// Raises the login success event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool			OnLoginSuccess(IEvent evt)
	{
		TcpEvent.SCNetLogin v = evt.Args as TcpEvent.SCNetLogin;

		// save global user info
		GlobalUserInfo.UserID 			= v.UserID;
		GlobalUserInfo.GateIPAddress 	= v.GateIP;
		GlobalUserInfo.GatePort 		= v.Port;
		GlobalUserInfo.LoginCode 		= v.LoginCode;
		GlobalUserInfo.LoginTime 		= v.LoginTime;

		// start game logic plugin
		LogicPlugin plugin = GameEngine.GetSingleton().LoadPlugin<LogicPlugin>();
		if (plugin)
			plugin.Startup();

		// close game login observer
		Dispatcher.UnregisterObserver(typeof(LoginObserver).Name);

		return true;
	}
}
