using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// User interface version.
/// </summary>
public class UIYes : IUIBox
{
	public const string	UY_TEXT = "UY_TEXT";
	public const string UY_YES	= "UY_YES";

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Install(new string[]{
			UY_TEXT,
			UY_YES,
		});
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		RegisterClickEvent(UY_YES, OnYesClicked);
	}

	/// <summary>
	/// Sets the text.
	/// </summary>
	/// <value>The text.</value>
	public override void SetText(string text)
	{
		SetText(UY_TEXT, text);
	}

	/// <summary>
	/// Voids the delegate.
	/// </summary>
	/// <param name="go">Go.</param>
	/// <param name="eventData">Event data.</param>
	protected void 		OnYesClicked (GameObject go, BaseEventData eventData)
	{
		Enter(true, Args);

		UISystem.GetSingleton().UnloadWidget(name);
	}
}
