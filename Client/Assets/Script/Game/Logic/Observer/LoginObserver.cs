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
	/// Start this instance.
	/// </summary>
	void Start()
	{
		SubscribeEvent(ITcpSession.TCP_CONNECTERROR, 
		               OnTcpConnectError);
		SubscribeEvent(ITcpSession.TCP_CONNECTFINISH,
		               OnTcpConnectSuccess);
		SubscribeEvent(ITcpSession.TCP_DISCONNECTED, 
		               OnTcpDisconnected);

		SubscribeEvent(CmdEvt.CMD_UI_LOGIN, 	OnLoginEvent);
		SubscribeEvent(CmdEvt.CMD_UI_REGISTER, 	OnRegisterEvent);
	}

	/// <summary>
	/// Active this instance.
	/// </summary>
	public virtual void 	Active()
	{
		// create login ui resource
		if (!LoginUI)
			LoginUI = UISystem.GetSingleton().LoadWidget<UILogin>(ResourceDef.UI_LOGIN);
	}
	
	/// <summary>
	/// Detive this instance.
	/// </summary>
	public virtual void 	Detive()
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
		CmdEvt.UILoginEventArgs v = evt.Args as CmdEvt.UILoginEventArgs;
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
		LogicHelper.Error(ErrorCode.ERR_DISCONNECT);

		return true;
	}

	/// <summary>
	/// Raises the tcp connect success event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool			OnTcpConnectSuccess(IEvent evt)
	{
		return true;
	}

	/// <summary>
	/// Raises the tcp disconnected event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool			OnTcpDisconnected(IEvent evt)
	{
		LogicHelper.Error(ErrorCode.ERR_DISCONNECT);

		return true;
	}
}
