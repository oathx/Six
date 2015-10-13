using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public delegate bool	MessageBoxCallback(bool bFlag, object args);

/// <summary>
/// User interface version.
/// </summary>
public class IUIBox : IUIWidget
{
	/// <summary>
	/// Sets the text.
	/// </summary>
	/// <value>The text.</value>
	public virtual void 		SetText(string text)
	{
	}
	
	/// <summary>
	/// Gets or sets the enter.
	/// </summary>
	/// <value>The enter.</value>
	public MessageBoxCallback	Enter
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the arguments.
	/// </summary>
	/// <value>The arguments.</value>
	public object				Args
	{ get; set; }
}

