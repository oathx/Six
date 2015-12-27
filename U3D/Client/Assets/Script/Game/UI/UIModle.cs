using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

/// <summary>
/// Modle alignment style.
/// </summary>
public enum ModleAlignmentStyle
{
	MAS_RIGHTUP		= 0,
	MAS_RIGHT		= 1,
	MAS_RIGHTDOWN	= 2,
}

/// <summary>
/// User interface version.
/// </summary>
public class UIModle : IUIWidget
{
	public const string UM_RIGHTUP = "UM_RIGHTUP";

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Install( new string[]{
			UM_RIGHTUP,
		});
	}
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{

	}

	/// <summary>
	/// Gets the alignment.
	/// </summary>
	/// <returns>The alignment.</returns>
	/// <param name="style">Style.</param>
	public GameObject	GetAlignment(ModleAlignmentStyle style)
	{
		switch(style)
		{
		case ModleAlignmentStyle.MAS_RIGHTUP:
			return Child[UM_RIGHTUP];
		}

		return default(GameObject);
	}

	/// <summary>
	/// Load the specified prefab and nID.
	/// </summary>
	/// <param name="prefab">Prefab.</param>
	/// <param name="nID">N I.</param>
	public bool			Load(GameObject prefab, int nID, ModleAlignmentStyle style)
	{
		GameObject alignment = GetAlignment(style);
		if (!alignment)
			throw new System.NullReferenceException("can't set style " + style.ToString());

		// create modle icon
		GameObject icon = GameObject.Instantiate(prefab) as GameObject;
		if (icon)
		{
			icon.transform.parent 			= alignment.transform;
			icon.transform.name				= nID.ToString();
			icon.transform.localRotation	= alignment.transform.localRotation;

			RegisterClickEvent(icon, OnIconClicked);
		}

		return true;
	}

	/// <summary>
	/// Raises the icon clicked event.
	/// </summary>
	/// <param name="go">Go.</param>
	/// <param name="eventData">Event data.</param>
	protected void 		OnIconClicked(GameObject go, BaseEventData eventData)
	{
		Debug.Log(go);
	}
}

