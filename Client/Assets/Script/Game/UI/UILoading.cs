using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// User interface version.
/// </summary>
public class UILoading : IUIWidget
{
	public const string	UL_SLIDER 	= "UL_SLIDER";
	public const string UL_TEXT		= "UL_TEXT";

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Install(new string[]{
			UL_SLIDER,
			UL_TEXT,
		});
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{

	}

	/// <summary>
	/// Sets the progress.
	/// </summary>
	/// <value>The progress.</value>
	public float	Progress
	{
		set{
			SetSlider(UL_SLIDER, value);
		}
	}

	/// <summary>
	/// Sets the text.
	/// </summary>
	/// <value>The text.</value>
	public string	Text
	{
		set{
			SetText(UL_TEXT, value);
		}
	}
}
