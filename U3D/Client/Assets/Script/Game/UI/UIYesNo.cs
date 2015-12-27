using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// User interface version.
/// </summary>
public class UIYesNo : IUIBox
{
	public const string	UY_TEXT = "UY_TEXT";
	public const string UY_YES	= "UY_YES";
	public const string UY_NO	= "UY_NO";

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Install(new string[]{
			UY_TEXT,
			UY_YES,
			UY_NO
		});
	}
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		RegisterClickEvent(UY_YES, 	OnYesClicked);
		RegisterClickEvent(UY_NO, 	OnNoClicked);
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

	/// <summary>
	/// Raises the no clicked event.
	/// </summary>
	/// <param name="go">Go.</param>
	/// <param name="eventData">Event data.</param>
	protected void 		OnNoClicked (GameObject go, BaseEventData eventData)
	{
		Enter(false, Args);
		
		UISystem.GetSingleton().UnloadWidget(name);
	}
}

