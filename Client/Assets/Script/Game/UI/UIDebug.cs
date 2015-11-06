using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// User interface version.
/// </summary>
public class UIDebug : IUIWidget
{
	public const string	UD_TEXT 	= "UD_TEXT";
	public const string UD_SUBMIT	= "UD_SUBMIT";
	public const string UD_INPUT	= "UD_INPUT";

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Install(new string[]{
			UD_TEXT,
			UD_SUBMIT,
			UD_INPUT,
		});
	}

	/// <summary>
	/// Raises the sumbit clicked event.
	/// </summary>
	/// <param name="goSend">Go send.</param>
	/// <param name="evtData">Evt data.</param>
	protected void OnSumbitClicked(GameObject goSend, BaseEventData evtData)
	{

	}
}
