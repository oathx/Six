using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// User interface char item.
/// </summary>
public class UITraceItem : IUIWidget
{
	public const string UT_NAME 	= "UT_NAME";
	public const string UT_DESC 	= "UT_DESC";
	
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Install(new string[]{
			UT_NAME,
			UT_DESC,
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
			SetText(UT_NAME, value);
		}
	}
	
	/// <summary>
	/// Sets the level.
	/// </summary>
	/// <value>The level.</value>
	public string 		Desc
	{
		set{
			SetText(UT_DESC, value.ToString());
		}
	}	
}

