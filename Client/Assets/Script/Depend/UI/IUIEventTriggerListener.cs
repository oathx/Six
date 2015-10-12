using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/// <summary>
/// Event trigger listener.
/// </summary>
public class IUIEventTriggerListener : UnityEngine.EventSystems.EventTrigger
{
	public delegate void VoidDelegate (GameObject go, BaseEventData eventData);
	
	public VoidDelegate		onClick;
	public VoidDelegate 	onDown;
	public VoidDelegate 	onEnter;
	public VoidDelegate 	onExit;
	public VoidDelegate 	onUp;
	public VoidDelegate 	onSelect;
	public VoidDelegate 	onUpdateSelect;

	/// <summary>
	/// Get the specified go.
	/// </summary>
	/// <param name="go">Go.</param>
	static public IUIEventTriggerListener Get (GameObject go)
	{
		IUIEventTriggerListener listener = go.GetComponent<IUIEventTriggerListener>();
		if (!listener) 
			listener = go.AddComponent<IUIEventTriggerListener>();

		return listener;
	}

	/// <summary>
	/// Raises the pointer click event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public override void OnPointerClick(PointerEventData eventData)
	{
		if(onClick != null) 	
			onClick(gameObject, eventData);
	}

	/// <summary>
	/// Raises the pointer down event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public override void OnPointerDown (PointerEventData eventData)
	{
		if(onDown != null) 
			onDown(gameObject, eventData);
	}

	/// <summary>
	/// Raises the pointer enter event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public override void OnPointerEnter (PointerEventData eventData)
	{
		if(onEnter != null) 
			onEnter(gameObject, eventData);
	}

	/// <summary>
	/// Raises the pointer exit event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public override void OnPointerExit (PointerEventData eventData)
	{
		if(onExit != null)
			onExit(gameObject, eventData);
	}

	/// <summary>
	/// Raises the pointer up event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public override void OnPointerUp (PointerEventData eventData)
	{
		if(onUp != null) 
			onUp(gameObject, eventData);
	}

	/// <summary>
	/// Raises the select event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public override void OnSelect (BaseEventData eventData)
	{
		if(onSelect != null) 
			onSelect(gameObject, eventData);
	}

	/// <summary>
	/// Raises the update selected event.
	/// </summary>
	/// <param name="eventData">Event data.</param>
	public override void OnUpdateSelected (BaseEventData eventData)
	{
		if(onUpdateSelect != null) 
			onUpdateSelect(gameObject, eventData);
	}
}
