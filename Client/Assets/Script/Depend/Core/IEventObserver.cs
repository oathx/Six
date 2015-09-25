using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// I event dispatch.
/// </summary>
public class IEventObserver : MonoBehaviour 
{
	/// <summary>
	/// The event callback list.
	/// </summary>
	protected Dictionary<int,
		EventCallback> m_dEventCallback = new Dictionary<int, EventCallback>();
	
	/// <summary>
	/// The m_dispath.
	/// </summary>
	public IEventDispatch	Dispatcher
	{get; set;}

	/// <summary>
	/// Startup this instance.
	/// </summary>
	public virtual void 	Run()
	{
	}

	/// <summary>
	/// Shutdown this instance.
	/// </summary>
	public virtual void 	Stop()
	{
	}

	/// <summary>
	/// Active this instance.
	/// </summary>
	public virtual void 	Active()
	{

	}

	/// <summary>
	/// Detive this instance.
	/// </summary>
	public virtual void 	Detive()
	{
	}
	
	/// <summary>
	/// Subscribes the event.
	/// </summary>
	/// <param name="nID">N I.</param>
	/// <param name="callback">Callback.</param>
	public virtual void 	SubscribeEvent(int nID, EventCallback callback)
	{
		if (!m_dEventCallback.ContainsKey(nID))
		{
#if UNITY_EDITOR
			Debug.Log("Observer " + name + " subscribe event id : " + nID);
#endif
			m_dEventCallback.Add(nID, callback);
		}
	}
	
	/// <summary>
	/// Determines whether this instance has event the specified nID.
	/// </summary>
	/// <returns><c>true</c> if this instance has event the specified nID; otherwise, <c>false</c>.</returns>
	/// <param name="nID">N I.</param>
	public virtual bool		HasEvent(int nID)
	{
		return m_dEventCallback.ContainsKey(nID);
	}
	
	/// <summary>
	/// Fires the event.
	/// </summary>
	/// <param name="nID">N I.</param>
	/// <param name="args">Arguments.</param>
	public virtual	bool 	FireEvent(IEvent evt)
	{
#if UNITY_EDITOR
		//Debug.Log("execute event callback id : " + evt.ID.ToString());
#endif
		return m_dEventCallback[evt.ID](evt);
	}
	
	/// <summary>
	/// Removes the event.
	/// </summary>
	/// <param name="nID">N I.</param>
	public virtual	void	RemoveEvent(int nID)
	{
		if (m_dEventCallback.ContainsKey(nID))
		{
#if UNITY_EDITOR
			Debug.Log("Observer " + name + " remove event id : " + nID);
#endif
			m_dEventCallback.Remove(nID);
		}
	}
	
	/// <summary>
	/// Clears the event.
	/// </summary>
	public virtual	void 	ClearEvent()
	{
		m_dEventCallback.Clear();
	}	
}


