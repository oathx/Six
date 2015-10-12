using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIVersion : IUIWidget
{
	public const string	UI_BUTTON = "Button";
	public const string	UI_SLIDER = "Slider";

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Install(new string[]{
			UI_BUTTON,
			UI_SLIDER,
		});
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		RegisterClickEvent(UI_BUTTON, OnButtonClicked);
	}

	/// <summary>
	/// Raises the button clicked event.
	/// </summary>
	/// <param name="goSend">Go send.</param>
	/// <param name="eventData">Event data.</param>
	void OnButtonClicked(GameObject goSend, BaseEventData eventData)
	{
		Debug.Log(goSend.name);
	}
}
