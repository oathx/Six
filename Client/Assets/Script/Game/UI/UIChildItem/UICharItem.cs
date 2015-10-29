using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// User interface char item.
/// </summary>
public class UICharItem : IUIWidget
{
	public const string UC_NAME 	= "UC_NAME";
	public const string UC_ICON 	= "UC_ICON";
	public const string UC_LEVEL	= "UC_LEVEL";

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Install(new string[]{
			UC_NAME,
			UC_ICON,
			UC_LEVEL,
		});
	}

	/// <summary>
	/// Gets or sets the I.
	/// </summary>
	/// <value>The I.</value>
	public int 			ID
	{ get; set; }
	
	/// <summary>
	/// Sets the name.
	/// </summary>
	/// <value>The name.</value>
	public string		Name
	{
		set{
			SetText(UC_NAME, value);
		}
	}
	
	/// <summary>
	/// Sets the level.
	/// </summary>
	/// <value>The level.</value>
	public int 			Level
	{
		set{
			SetText(UC_LEVEL, value.ToString());
		}
	}

	/// <summary>
	/// Sets a value indicating whether this <see cref="UICharItem"/> is select.
	/// </summary>
	/// <value><c>true</c> if select; otherwise, <c>false</c>.</value>
	public bool			Select
	{
		get{
			Button b = GetComponent<Button>();
			return b.interactable;
		}
		set{
			Button b = GetComponent<Button>();
			b.interactable = value;
		}
	}
}
