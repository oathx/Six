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
			GameEngine.GetSingleton().SendEvent(CmdEvt.CreateErrorEvent(ErrorCode.ERR_USERNAME));
		}
		else
		{

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
}
