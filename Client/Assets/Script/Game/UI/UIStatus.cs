using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// User interface version.
/// </summary>
public class UIStatus : IUIBox
{
	public const string	US_TEXT = "US_TEXT";
	
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Install(new string[]{
			US_TEXT,
		});
	}
	
	/// <summary>
	/// Sets the text.
	/// </summary>
	/// <value>The text.</value>
	public override void SetText(string text)
	{
		SetText(US_TEXT, text);
	}
}

