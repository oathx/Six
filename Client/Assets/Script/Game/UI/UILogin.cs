using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// User interface version.
/// </summary>
public class UILogin : IUIWidget
{
	public const string UL_USERNAME = "UL_USERNAME";
	public const string UL_PASSWORD = "UL_PASSWORD";
	public const string UL_LOGIN	= "UL_LOGIN";
	public const string UL_REGISTER = "UL_REGISTER";

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Install(new string[]{
			UL_USERNAME,
			UL_PASSWORD,
			UL_LOGIN,
			UL_REGISTER,
		});
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		RegisterClickEvent(UL_LOGIN, 	OnLoginClicked);
		RegisterClickEvent(UL_REGISTER, OnRegisterClicked);
	}

	/// <summary>
	/// Gets the name of the user.
	/// </summary>
	/// <returns>The user name.</returns>
	public string	GetUserName()
	{
		return GetInputText(UL_USERNAME);
	}

	/// <summary>
	/// Gets the password.
	/// </summary>
	/// <returns>The password.</returns>
	public string	GetPassword()
	{
		return GetInputText(UL_PASSWORD);
	}

	/// <summary>
	/// Raises the login clicked event.
	/// </summary>
	/// <param name="goSend">Go send.</param>
	/// <param name="evtData">Evt data.</param>
	protected void OnLoginClicked(GameObject goSend, BaseEventData evtData)
	{
		CmdEvt.UILoginEventArgs v = new CmdEvt.UILoginEventArgs();
		v.Widget 	= this;
		v.UserName	= GetUserName();
		v.Password	= GetPassword();

		GameEngine.GetSingleton().SendEvent(
			new IEvent(EngineEventType.EVENT_UI, CmdEvt.CMD_UI_LOGIN, v)
			);
	}

	/// <summary>
	/// Raises the register clicked event.
	/// </summary>
	/// <param name="goSend">Go send.</param>
	/// <param name="evtData">Evt data.</param>
	protected void OnRegisterClicked(GameObject goSend, BaseEventData evtData)
	{
		CmdEvt.UIClickEventArgs v = new CmdEvt.UIClickEventArgs();
		v.Widget = this;
		
		GameEngine.GetSingleton().SendEvent(
			new IEvent(EngineEventType.EVENT_UI, CmdEvt.CMD_UI_REGISTER, v)
			);
	}
}
