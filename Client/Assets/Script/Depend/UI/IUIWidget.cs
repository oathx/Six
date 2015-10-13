using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// IUI widget.
/// </summary>
public class IUIWidget : MonoBehaviour
{
	public Dictionary<string, GameObject> Child = new Dictionary<string, GameObject>();

	/// <summary>
	/// Install the specified aryName.
	/// </summary>
	/// <param name="aryName">Ary name.</param>
	public void Install(string[] aryName)
	{
		Transform[] aryTransform = GetComponentsInChildren<Transform>();
		foreach(Transform child in aryTransform)
		{
			foreach(string curName in aryName)
			{
				if (child.name == curName)
				{
#if OPEN_DEBUG_LOG
					Debug.Log("widget install child name : " + curName);
#endif
					Child.Add(
						curName, child.gameObject
						);
				}
			}
		}
	}

	/// <summary>
	/// Registers the click event.
	/// </summary>
	/// <param name="szName">Size name.</param>
	public void RegisterClickEvent(string szName, IUIEventTriggerListener.VoidDelegate callback)
	{
		IUIEventTriggerListener.Get(Child[szName]).onClick = callback;
	}

	/// <summary>
	/// Sets the text.
	/// </summary>
	/// <param name="szName">Size name.</param>
	/// <param name="text">Text.</param>
	public void SetText(string szName, string text)
	{
		Text lab = Child[szName].GetComponent<Text>();
		if (lab)
			lab.text = text;
	}
}
