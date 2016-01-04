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
		LoginPlugin plugin = GameEngine.GetSingleton().QueryPlugin<LoginPlugin>();
		if (plugin)
		{
			plugin.RegisterPackageFactory(TcpEvent.CMD_REPLY_LOGIN_SUCCESS, 
			                              new DefaultNetMessageFactory<TcpEvent.SCNetLoginReply>());
			plugin.RegisterPackageFactory(TcpEvent.CMD_REPLY_REGISTER_SUCCESS, 
			                              new DefaultNetMessageFactory<TcpEvent.SCNetRegisterSuccess>());
		}

		SubscribeEvent(ITcpSession.TCP_CONNECTERROR, OnTcpConnectError);
		SubscribeEvent(ITcpSession.TCP_CONNECTFINISH,OnTcpConnectSuccess);
		SubscribeEvent(ITcpSession.TCP_DISCONNECTED, OnTcpDisconnected);
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		SubscribeEvent(CmdEvent.CMD_UI_LOGIN, 		OnUILoginClicked);
		SubscribeEvent(CmdEvent.CMD_UI_REGISTER,	OnUIRegisterClicked);
		SubscribeEvent(CmdEvent.CMD_UI_REGBUTTON,	OnUIRegisterButtonClicked);
		SubscribeEvent(CmdEvent.CMD_UI_REGRETURN,	OnUIRegisterReturnClicked);

		// subscribe all net protocol
		SubscribeEvent(TcpEvent.CMD_REPLY_LOGIN_SUCCESS,
		               OnNetLoginSuccess);
		SubscribeEvent(TcpEvent.CMD_REPLY_REGISTER_SUCCESS,
		               OnNetRegisterSuccess);
	}

	/// <summary>
	/// Active this instance.
	/// </summary>
	public override void 	Active()
	{
		LoginPlugin plugin = GameEngine.GetSingleton().QueryPlugin<LoginPlugin>();
		if (plugin)
		{
			if (!plugin.Connected)
			{
				SqlSystem sqlSystem = GameSqlLite.GetSingleton().Query<SqlSystem>(0);

				// connect to login server
				plugin.Connect(
					sqlSystem.IPAddress, sqlSystem.Port
					);

				UISystem.GetSingleton().ShowStatus(
					LogicHelper.GetErrorText(ErrorCode.ERR_CONNECTING)
					);
			}
			else
			{
				OpenLoginUI();
			}
		}
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
	/// Opens the login U.
	/// </summary>
	/// <returns><c>true</c>, if login U was opened, <c>false</c> otherwise.</returns>
	protected bool			OpenLoginUI()
	{
		// create login ui resource
		if (!LoginUI)
			LoginUI = UISystem.GetSingleton().LoadWidget<UILogin>(ResourceDef.UI_LOGIN);
		
		// query system config info
		SqlSystem sqlSystem = GameSqlLite.GetSingleton().Query<SqlSystem>(0);
		if (!string.IsNullOrEmpty(sqlSystem.UserName) && !string.IsNullOrEmpty(sqlSystem.Password))
		{
			LoginUI.SetUserName(sqlSystem.UserName);
			LoginUI.SetPassword(sqlSystem.Password);
		}

		return true;
	}


	/// <summary>
	/// Raises the login event event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool 			OnUILoginClicked(IEvent evt)
	{
		CmdEvent.UILoginEventArgs v = evt.Args as CmdEvent.UILoginEventArgs;
		if (string.IsNullOrEmpty(v.UserName) || string.IsNullOrEmpty(v.Password))
		{
			LogicHelper.Error(ErrorCode.ERR_USERNAME);
		}
		else
		{
			SqlSystem sqlSystem = GameSqlLite.GetSingleton().Query<SqlSystem>(0);
			if (!sqlSystem)
				throw new System.NullReferenceException();

#if LOCAL_SERVER
			TcpEvent.SCNetLoginReply rep = new TcpEvent.SCNetLoginReply();
			rep.GateIP		= string.Empty;
			rep.Port		= 0;
			rep.LoginCode	= string.Empty;
			rep.LoginTime	= 0;
			rep.UserID		= 0;

			Dispatcher.SendEvent(
				new IEvent(TcpEvent.CMD_REPLY_LOGIN_SUCCESS, rep)
				);
#else
			LoginRequest.GetSingleton().RequestLogin(sqlSystem.ServerVersion, 
			                                         sqlSystem.ServerID, v.UserName, v.Password);

#endif
		}

		return true;
	}

	/// <summary>
	/// Raises the register event event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool 			OnUIRegisterButtonClicked(IEvent evt)
	{
		LoginPlugin plugin = GameEngine.GetSingleton().QueryPlugin<LoginPlugin>();
		if (plugin)
		{
			if (!plugin.Connected)
			{
				UISystem.GetSingleton().Box(BoxStyle.YES, LogicHelper.GetErrorText(ErrorCode.ERR_DISCONNECT), 0,
				                            delegate(bool bFlag, object args) {

					LoginUI.Show(); return true;
				});
			}
			else
			{
				UIRegister reg = UISystem.GetSingleton().LoadWidget<UIRegister>(ResourceDef.UI_REGISTER);
				if (!reg)
					throw new System.NullReferenceException();

				LoginUI.Hide();
			}
		}

		return true;
	}

	/// <summary>
	/// Raises the register return event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool			OnUIRegisterReturnClicked(IEvent evt)
	{
		UISystem.GetSingleton().UnloadWidget(
			ResourceDef.UI_REGISTER
			);

		return LoginUI.Show();
	}
	
	/// <summary>
	/// Raises the register clicked event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool			OnUIRegisterClicked(IEvent evt)
	{
		CmdEvent.UIRegisterEventArgs v = evt.Args as CmdEvent.UIRegisterEventArgs;

		SqlSystem sqlSystem = GameSqlLite.GetSingleton().Query<SqlSystem>(0);
		if (!sqlSystem)
			throw new System.NullReferenceException();

		return LoginRequest.GetSingleton().RequestRegisterAccount(v.UserName, v.Password, sqlSystem.ServerID);
	}

	/// <summary>
	/// Raises the tcp connect error event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool			OnTcpConnectError(IEvent evt)
	{
		UISystem.GetSingleton().HideStatus();

		// try connect to login server
		UISystem.GetSingleton().Box(BoxStyle.YES, LogicHelper.GetErrorText(ErrorCode.ERR_LOCALCONNECT), 0,
		                            delegate(bool bFlag, object args) {

#if LOCAL_SERVER
			OpenLoginUI();
#else
			Active();
#endif
			return true; 
		});

		return true;
	}

	/// <summary>
	/// Raises the tcp connect success event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool			OnTcpConnectSuccess(IEvent evt)
	{
		UISystem.GetSingleton().HideStatus();

		// create login ui resource
		if (!LoginUI)
			LoginUI = UISystem.GetSingleton().LoadWidget<UILogin>(ResourceDef.UI_LOGIN);
		
		// query system config info
		SqlSystem sqlSystem = GameSqlLite.GetSingleton().Query<SqlSystem>(0);
		if (!string.IsNullOrEmpty(sqlSystem.UserName) && !string.IsNullOrEmpty(sqlSystem.Password))
		{
			LoginUI.SetUserName(sqlSystem.UserName);
			LoginUI.SetPassword(sqlSystem.Password);
		}

		return true;
	}

	/// <summary>
	/// Raises the tcp disconnected event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool			OnTcpDisconnected(IEvent evt)
	{
		UISystem.GetSingleton().HideStatus();

		return OnTcpConnectError(evt);
	}

	/// <summary>
	/// Raises the login success event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool			OnNetLoginSuccess(IEvent evt)
	{
		TcpEvent.SCNetLoginReply v = evt.Args as TcpEvent.SCNetLoginReply;

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

	/// <summary>
	/// Raises the register success event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool			OnNetRegisterSuccess(IEvent evt)
	{
		UIRegister reg = UISystem.GetSingleton().LoadWidget<UIRegister>(ResourceDef.UI_REGISTER);
		if (!reg)
			throw new System.NullReferenceException();

		// register account success
		UISystem.GetSingleton().Box(BoxStyle.YES, 
		                            LogicHelper.GetErrorText(ErrorCode.ERR_REGISTEROK), 0, delegate(bool bFlag, object args) {

			// destroy register gui
			UISystem.GetSingleton().UnloadWidget(ResourceDef.UI_REGISTER);

			LoginUI.SetUserName(reg.GetUserName());
			LoginUI.SetPassword(reg.GetPassword());

			// open login gui
			LoginUI.Show();

			return true;
		});

		return true;
	}
}
