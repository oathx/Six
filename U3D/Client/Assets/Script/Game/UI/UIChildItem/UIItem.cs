using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// User interface char item.
/// </summary>
public class UIItem : IUIWidget
{
	public const string	UI_ICON 	= "UI_ICON";
	public const string UI_COUNT	= "UI_COUNT";

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Install(new string[]{
			UI_ICON,
			UI_COUNT,
		});
	}

	/// <summary>
	/// Gets or sets the I.
	/// </summary>
	/// <value>The I.</value>
	public int 		ID
	{ get; set; }


	/// <summary>
	/// Sets the icon.
	/// </summary>
	/// <value>The icon.</value>
	public string	Icon
	{
		set{

		}
	}

	/// <summary>
	/// Sets the count.
	/// </summary>
	/// <value>The count.</value>
	public int 		Count
	{
		set{
			SetText(UI_COUNT, value.ToString());
		}
	}
}
