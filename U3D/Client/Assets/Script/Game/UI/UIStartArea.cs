using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// User interface start area.
/// </summary>
public class UIStartArea : IUIWidget
{
	public const string US_SKIP	= "US_SKIP";

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Install(new string[]{
			US_SKIP
		});
	}
}
