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
	/// Raises the login clicked event.
	/// </summary>
	/// <param name="goSend">Go send.</param>
	/// <param name="evtData">Evt data.</param>
	protected void OnLoginClicked(GameObject goSend, BaseEventData evtData)
	{

	}

	/// <summary>
	/// Raises the register clicked event.
	/// </summary>
	/// <param name="goSend">Go send.</param>
	/// <param name="evtData">Evt data.</param>
	protected void OnRegisterClicked(GameObject goSend, BaseEventData evtData)
	{
		
	}
}
