using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// User interface mission.
/// </summary>
public class UIMission : IUIWidget
{
	public const string UM_TRACE	= "UM_TRACE";

	/// <summary>
	/// The m_ trace.
	/// </summary>
	public List<UITraceItem> 
		Trace = new List<UITraceItem>();

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Install(new string[]{
			UM_TRACE
		});
	}
	
	/// <summary>
	/// Adds the trace.
	/// </summary>
	/// <param name="nID">N I.</param>
	public UITraceItem Add(int nID, string szName, string szDesc)
	{
		int index = Trace.FindIndex(delegate(UITraceItem obj) {
			return obj.ID == nID;
		});
		if (index >= 0)
			return Trace[index];

		GameObject resource = Resources.Load<GameObject>(ResourceDef.UI_TRACEITEM);
		if (!resource)
			throw new System.NullReferenceException();

		UITraceItem traceItem = AddChild<UITraceItem>(UM_TRACE, resource);
		if (!traceItem)
			throw new System.NullReferenceException();

		traceItem.ID 	= nID;
		traceItem.Name	= szName;
		traceItem.Desc	= szDesc;

		Trace.Add(traceItem);

		return traceItem;
	}

	/// <summary>
	/// Gets the trace count.
	/// </summary>
	/// <returns>The trace count.</returns>
	public int 	GetTraceCount()
	{
		return Trace.Count;
	}

	/// <summary>
	/// Removes the trace.
	/// </summary>
	/// <param name="nID">N I.</param>
	public void Remove(int nID)
	{
		int index = Trace.FindIndex(delegate(UITraceItem obj) {
			return obj.ID == nID;
		});
		if (index >= 0)
		{
			GameObject.Destroy(
				Trace[index].gameObject
				);

			Trace.RemoveAt(index);
		}
	}
}

