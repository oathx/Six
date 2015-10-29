using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// User interface version.
/// </summary>
public class UIVersion : IUIWidget
{
	public const string	UV_SLIDER 	= "UV_SLIDER";
	public const string	UV_TEXT 	= "UV_TEXT";
	public const string	UV_VERSION 	= "UV_VERSION";

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Install(new string[]{
			UV_SLIDER,
			UV_TEXT,
			UV_VERSION,
		});
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		Show(UV_SLIDER, false);
	}

	/// <summary>
	/// Sets the version.
	/// </summary>
	/// <value>The version.</value>
	public string	Version
	{
		set{
			SetText(UV_VERSION, value);
		}
	}

	/// <summary>
	/// Sets the progress.
	/// </summary>
	/// <value>The progress.</value>
	public string	Text
	{
		set{
			SetText(UV_TEXT, value);
		}
	}

	/// <summary>
	/// Sets the progress.
	/// </summary>
	/// <value>The progress.</value>
	public float	Progress
	{
		set{
			if (value > 0.0f)
				Show(UV_SLIDER, true);

			// update slider
			SetSlider(
				UV_SLIDER, value
				);
		}
	}
}
