using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// User interface version.
/// </summary>
public class UIRegister : IUIWidget
{
	public const string UR_USERNAME	= "UR_USERNAME";
	public const string UR_PASSWORD = "UR_PASSWORD";
	public const string UR_VERIFY	= "UR_VERIFY";
	public const string UR_REGISTER	= "UR_REGISTER";
	public const string UR_RETURN	= "UR_RETURN";

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Install(new string[]{
			UR_USERNAME,
			UR_PASSWORD,
			UR_VERIFY,
			UR_REGISTER,
			UR_RETURN,
		});
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		RegisterClickEvent(UR_REGISTER, OnRegisterClicked);
		RegisterClickEvent(UR_RETURN,	OnReturnClicked);
	}

	/// <summary>
	/// Gets the name of the user.
	/// </summary>
	/// <returns>The user name.</returns>
	public string	GetUserName()
	{
		return GetInputText(UR_USERNAME);
	}

	/// <summary>
	/// Gets the password.
	/// </summary>
	/// <returns>The password.</returns>
	public string	GetPassword()
	{
		return GetInputText(UR_PASSWORD);
	}

	/// <summary>
	/// Raises the register clicked event.
	/// </summary>
	/// <param name="go">Go.</param>
	/// <param name="evtData">Evt data.</param>
	protected void OnRegisterClicked(GameObject go, BaseEventData evtData)
	{
		string szUserName 	= GetInputText(UR_USERNAME);
		string szPassword 	= GetInputText(UR_PASSWORD);
		string szVerify		= GetInputText(UR_VERIFY);

		if (!string.IsNullOrEmpty(szUserName) && szVerify == szPassword)
		{
			CmdEvent.UIRegisterEventArgs v = new CmdEvent.UIRegisterEventArgs();
			v.UserName 	= GetInputText(UR_USERNAME);
			v.Password	= szPassword;
			v.Widget	= this;
			
			GameEngine.GetSingleton().SendEvent(
				new IEvent(EngineEventType.EVENT_UI, CmdEvent.CMD_UI_REGISTER, v)
				);
		}
	}

	/// <summary>
	/// Raises the return clicked event.
	/// </summary>
	/// <param name="go">Go.</param>
	/// <param name="evtData">Evt data.</param>
	protected void OnReturnClicked(GameObject go, BaseEventData evtData)
	{
		CmdEvent.UIClickEventArgs v = new CmdEvent.UIClickEventArgs();
		v.Widget = this;

		
		GameEngine.GetSingleton().SendEvent(
			new IEvent(EngineEventType.EVENT_UI, CmdEvent.CMD_UI_REGRETURN, v)
			);
	}
}
