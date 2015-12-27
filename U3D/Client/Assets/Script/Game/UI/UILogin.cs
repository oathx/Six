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
	public const string UL_USERNAME 	= "UL_USERNAME";
	public const string UL_PASSWORD 	= "UL_PASSWORD";
	public const string UL_LOGIN		= "UL_LOGIN";
	public const string UL_REGISTER 	= "UL_REGISTER";
	public const string UL_FASTREGISTER = "UL_FASTREGISTER";

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
			UL_FASTREGISTER,
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
	/// Sets the name of the user.
	/// </summary>
	/// <returns>The user name.</returns>
	/// <param name="szUserName">Size user name.</param>
	public void		SetUserName(string szUserName)
	{
		SetInputText(UL_USERNAME, szUserName);
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
	/// Sets the password.
	/// </summary>
	/// <returns>The password.</returns>
	/// <param name="szPassword">Size password.</param>
	public void		SetPassword(string szPassword)
	{
		SetInputText(UL_PASSWORD, szPassword);
	}

	/// <summary>
	/// Raises the login clicked event.
	/// </summary>
	/// <param name="goSend">Go send.</param>
	/// <param name="evtData">Evt data.</param>
	protected void OnLoginClicked(GameObject goSend, BaseEventData evtData)
	{
		CmdEvent.UILoginEventArgs v = new CmdEvent.UILoginEventArgs();
		v.Widget 	= this;
		v.UserName	= GetUserName();
		v.Password	= GetPassword();

		GameEngine.GetSingleton().SendEvent(
			new IEvent(EngineEventType.EVENT_UI, CmdEvent.CMD_UI_LOGIN, v)
			);
	}

	/// <summary>
	/// Raises the register clicked event.
	/// </summary>
	/// <param name="goSend">Go send.</param>
	/// <param name="evtData">Evt data.</param>
	protected void OnRegisterClicked(GameObject goSend, BaseEventData evtData)
	{
		CmdEvent.UIClickEventArgs v = new CmdEvent.UIClickEventArgs();
		v.Widget = this;
		
		GameEngine.GetSingleton().SendEvent(
			new IEvent(EngineEventType.EVENT_UI, CmdEvent.CMD_UI_REGBUTTON, v)
			);
	}
}
