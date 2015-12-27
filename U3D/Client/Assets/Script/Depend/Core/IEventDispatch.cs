using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;
using System.Text;
using LitJson;

// event type define
public enum EngineEventType
{
	EVENT_NET,		// net work event
	EVENT_UI,		// ui event
	EVENT_USER,		// user event
	EVENT_AI,		// ai event
	EVENT_PLOT, 	// plot Events
	EVENT_GLOBAL, 	// global Event.
	EVENT_SQL,		// sql event
}

/// <summary>
/// I null object.
/// </summary>
public class INullObject
{
	/// <param name="ep">Ep.</param>
	public static implicit operator bool(INullObject ep)
	{return ep != null;}
}

/// <summary>
/// I event arguments.
/// </summary>
public class IEventArgs : INullObject
{

}

/// <summary>
/// Virtual net package.
/// </summary>
public class VirtualNetPackage : IEventArgs
{
	/// <summary>
	/// Sets the buffer.
	/// </summary>
	/// <value>The buffer.</value>
	public object[]			buffer
	{ get; set; }
	
	/// <summary>
	/// Initializes a new instance of the <see cref="VirtualNetPackage"/> class.
	/// </summary>
	/// <param name="buffer">Buffer.</param>
	public VirtualNetPackage(object[] buf)
	{
		buffer = buf;
	}
}

/// <summary>
/// Dispatch type.
/// </summary>
public class IEvent
{
	/// <param name="ep">Ep.</param>
	public static implicit operator bool(IEvent ep)
	{return ep != null;}
	
	/// <summary>
	/// Gets or sets the dispatcher.
	/// </summary>
	/// <value>The dispatcher.</value>
	public IEventDispatch	Dispatcher
	{get; set;}
	
	/// <summary>
	/// Gets or sets the I.
	/// </summary>
	/// <value>The I.</value>
	public int 				ID
	{get; set;}

	/// <summary>
	/// Gets or sets the sub I.
	/// </summary>
	/// <value>The sub I.</value>
	public int 				SubID
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the delay time.
	/// </summary>
	/// <value>The delay time.</value>
	public double			DelayTime
	{get; set;}
	
	/// <summary>
	/// Gets or sets the text.
	/// </summary>
	/// <value>The text.</value>
	public IEventArgs		Args
	{ get; set;}
	
	/// <summary>
	/// Gets or sets the handle.
	/// </summary>
	/// <value>The handle.</value>
	public object			Handle
	{get; set;}
	
	/// <summary>
	/// Gets or sets the type.
	/// </summary>
	/// <value>The type.</value>
	public EngineEventType	Type
	{get; set;}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="IEvent"/> class.
	/// </summary>
	/// <param name="nID">N I.</param>
	/// <param name="szText">Size text.</param>
	public IEvent(EngineEventType type, int nID)
	{
		Type		= type;
		ID 			= nID;
		DelayTime 	= 0.0f;
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="EventPackage"/> class.
	/// </summary>
	/// <param name="nID">N I.</param>
	/// <param name="szText">Size text.</param>
	public IEvent(int nID, IEventArgs args)
	{
		Type		= EngineEventType.EVENT_USER;
		ID 			= nID;
		DelayTime	= 0.0f;
		Args 		= args;
		SubID 		= 0;
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="IEvent"/> class.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="nID">N I.</param>
	/// <param name="szText">Size text.</param>
	public IEvent(EngineEventType type, int nID, IEventArgs args)
	{
		Type		= type;
		ID 			= nID;
		Args 		= args;
		DelayTime	= 0.0f;
		SubID 		= 0;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="IEvent"/> class.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="nID">N I.</param>
	/// <param name="fDelayTime">F delay time.</param>
	/// <param name="args">Arguments.</param>
	public IEvent(EngineEventType type, int nID, float fDelayTime, IEventArgs args)
	{
		Type		= type;
		ID 			= nID;
		Args 		= args;
		DelayTime	= fDelayTime;
		SubID 		= 0;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="IEvent"/> class.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="nID">N I.</param>
	/// <param name="nSubID">N sub I.</param>
	/// <param name="fDelayTime">F delay time.</param>
	/// <param name="args">Arguments.</param>
	public IEvent(EngineEventType type, int nID, int nSubID, float fDelayTime, IEventArgs args)
	{
		Type		= type;
		ID 			= nID;
		Args 		= args;
		DelayTime	= fDelayTime;
		SubID 		= nSubID;
	}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="IEvent"/> class.
	/// </summary>
	/// <param name="nID">N I.</param>
	/// <param name="szText">Size text.</param>
	/// <param name="dispather">Dispather.</param>
	/// <param name="fDelayTime">F delay time.</param>
	public IEvent(EngineEventType type, int nID,  IEventArgs args, IEventDispatch dispather, float fDelayTime, object handle)
	{
		Type		= type;
		Dispatcher	= dispather;
		ID 			= nID;
		DelayTime	= fDelayTime;
		Args 		= args;
		SubID 		= 0;
	}
}

// event callback define
public delegate bool 	EventCallback(IEvent evt);

/// <summary>
/// Net protocol.
/// </summary>
public class EventConvert
{
	/// <summary>
	/// Structs to bytes.
	/// </summary>
	/// <returns>The to bytes.</returns>
	/// <param name="pStruct">P struct.</param>
	public static byte[]			StructToBytes(object curStruct)
	{
		// get the struct size
		int nSize = Marshal.SizeOf(curStruct);
		
		// struct convert
		System.IntPtr pStruct = Marshal.AllocHGlobal(nSize);
		Marshal.StructureToPtr(curStruct, pStruct, false);
		
		// alloc struct memory
		byte[] bytes = new byte[nSize];
		Marshal.Copy(pStruct, bytes, 0, nSize);
		
		// free the struct
		Marshal.FreeHGlobal(pStruct);
		
		return bytes;
	}
	
	/// <summary>
	/// Byteses to struct.
	/// </summary>
	/// <returns>The to struct.</returns>
	/// <param name="bytes">Bytes.</param>
	/// <param name="type">Type.</param>
	public static object			BytesToStruct(byte[] bytes, System.Type type)
	{
		int nSize = Marshal.SizeOf(type);
		if (nSize > bytes.Length)
			throw new System.ArgumentNullException();
		
		System.IntPtr pStruct = Marshal.AllocHGlobal(nSize);
		Marshal.Copy(bytes, 0, pStruct, nSize);
		
		object ret = Marshal.PtrToStructure(pStruct, type);
		Marshal.FreeHGlobal(pStruct);
		
		return ret;
	}
	
	/// <summary>
	/// Byteses to struct.
	/// </summary>
	/// <returns>The to struct.</returns>
	/// <param name="bytes">Bytes.</param>
	public static T					BytesToStruct<T>(byte[] bytes)
	{
		int nSize = Marshal.SizeOf(typeof(T));
		if (nSize > bytes.Length)
			throw new System.ArgumentNullException();
		
		System.IntPtr pStruct = Marshal.AllocHGlobal(nSize);
		Marshal.Copy(bytes, 0, pStruct, nSize);
		
		T ret = (T)Marshal.PtrToStructure(pStruct, typeof(T));
		Marshal.FreeHGlobal(pStruct);
		
		return ret;
	}
}

/// <summary>
/// I event dispatch.
/// </summary>
public class IEventDispatch : MonoBehaviour 
{
	/// <summary>
	/// The event observer.
	/// </summary>
	protected Dictionary<string, 
		IEventObserver> m_dObserver = new Dictionary<string, IEventObserver> ();

	/// <summary>
	/// The m_d buffer event.
	/// </summary>
	protected List<IEvent>	m_dPostEvent = new List<IEvent>();

	/// <summary>
	/// Loads the observer.
	/// </summary>
	/// <param name="type">Type.</param>
	public virtual IEventObserver 	LoadObserver(System.Type type)
	{
		if (m_dObserver.ContainsKey (type.Name))
			return m_dObserver [type.Name];

		GameObject child = new GameObject (type.Name);
		if (!child)
			throw new System.NullReferenceException (type.Name);
		
		child.transform.parent = transform;

		IEventObserver observer = (IEventObserver)child.AddComponent (type);
		if (!observer)
			throw new System.NullReferenceException (type.Name);

		observer.Dispatcher = this;

#if UNITY_EDITOR
		Debug.Log("Load observer : " + type.Name);
#endif
		m_dObserver.Add (type.Name, observer);

		return observer;
	}

	/// <summary>
	/// Loads the observer.
	/// </summary>
	/// <returns>The observer.</returns>
	/// <param name="szObserverName">Size observer name.</param>
	public virtual	IEventObserver 	LoadObserver(string szObserverName)
	{
		Assembly assembly = Assembly.GetExecutingAssembly();

		// get observer type
		System.Type type = assembly.GetType(szObserverName);
		return LoadObserver (type);
	}

	/// <summary>
	/// Registers the observer.
	/// </summary>
	/// <returns>The observer.</returns>
	/// <param name="szObserverName">Size observer name.</param>
	/// <param name="bActive">If set to <c>true</c> b active.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public virtual T		RegisterObserver<T>(string szObserverName, bool bActive) where T : IEventObserver
	{
		T observer = RegisterObserver<T>(szObserverName);

		// auto active the observer
		if (bActive)
			observer.Active();

		return observer;
	}

	/// <summary>
	/// Registers the observer.
	/// </summary>
	/// <param name="szObserverName">Size observer name.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public virtual T 		RegisterObserver<T>(string szObserverName) where T : IEventObserver
	{
		if (m_dObserver.ContainsKey (szObserverName))
			return m_dObserver[szObserverName] as T;

		GameObject child = new GameObject (szObserverName);
		if (!child)
			throw new System.NullReferenceException (szObserverName);

		child.transform.parent = transform;

		T observer = child.AddComponent<T> ();
		if (!observer)
			throw new System.NullReferenceException (szObserverName);

		observer.Dispatcher = this;
				
#if UNITY_EDITOR
		Debug.Log("Register observer : " + szObserverName);
#endif
		m_dObserver.Add (szObserverName, observer);

		return observer;
	}
	

	/// <summary>
	/// Queries the observer.
	/// </summary>
	/// <returns>The observer.</returns>
	/// <param name="szObserverName">Size observer name.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public virtual T		QueryObserver<T>(string szObserverName) where T : IEventObserver
	{
		if (!m_dObserver.ContainsKey (szObserverName))
			throw new System.NullReferenceException (szObserverName);

		return m_dObserver [szObserverName] as T;
	}

	/// <summary>
	/// Queries the observer.
	/// </summary>
	/// <returns>The observer.</returns>
	/// <param name="szObserverName">Size observer name.</param>
	public IEventObserver	QueryObserver(string szObserverName)
	{
		return m_dObserver[szObserverName];
	}
	
	/// <summary>
	/// Unregisters the observer.
	/// </summary>
	/// <param name="szObserverName">Size observer name.</param>
	public virtual void 	UnregisterObserver(string szObserverName)
	{
		try{
			if (m_dObserver.ContainsKey(szObserverName))
			{			
	#if UNITY_EDITOR
				Debug.Log("Unregister observer : " + szObserverName);
	#endif
				m_dObserver[szObserverName].Detive();

				GameObject.Destroy(
					m_dObserver[szObserverName].gameObject
					);

				m_dObserver.Remove(szObserverName);
			}
		}
		catch(System.Exception e)
		{
			Debug.LogWarning(e.Message + ":" + e.StackTrace);
		}
	}
	
	/// <summary>
	/// Dispatchs the event.
	/// </summary>
	/// <param name="cmd">CmdEvent.</param>
	/// <param name="text">Text.</param>
	/// <param name="nLength">N length.</param>
	public virtual bool 	DispatchEvent(IEvent evt)
	{
		if (!evt.Dispatcher)
			evt.Dispatcher = this;

		foreach(KeyValuePair<string, IEventObserver> it in m_dObserver)
		{
			if (it.Value.HasEvent(evt.ID))
			{
#if OPEN_DEBUG_LOG
				Debug.Log("Fire Event to : " + it.Key + " id : " + evt.ID);
#endif
				if (it.Value.FireEvent(evt))
					return true;
			}
			else
			{
#if OPEN_DEBUG_LOG
				Debug.LogWarning(it.Key + " Not subscribe to events " + evt.ID);
#endif
			}
		}

		return false;
	}
	
	/// <summary>
	/// Sends the event.
	/// </summary>
	/// <param name="nID">N I.</param>
	/// <param name="evt">Evt.</param>
	public virtual bool 	SendEvent(IEvent evt)
	{
		return DispatchEvent(evt);
	}
	
	/// <summary>
	/// Posts the event.
	/// </summary>
	/// <param name="nID">N I.</param>
	/// <param name="evt">Evt.</param>
	public virtual void 	PostEvent(IEvent evt)
	{
		m_dPostEvent.Add(evt);
	}
	
	/// <summary>
	/// Sends the event.
	/// </summary>
	/// <param name="nID">N I.</param>
	/// <param name="args">Arguments.</param>
	public virtual void 	SendEvent(int nID, params object[] args)
	{
		throw new System.NullReferenceException("No yet");
	}
	
	/// <summary>
	/// Sends the event.
	/// </summary>
	/// <param name="szRecvName">Size recv name.</param>
	/// <param name="evt">Evt.</param>
	public virtual void 	SendEvent(string szRecvName, IEvent evt)
	{
		IEventObserver observer = QueryObserver<IEventObserver> (szRecvName);
		if (observer.HasEvent(evt.ID))
			observer.FireEvent (evt);
	}
	
	/// <summary>
	/// Translats the buffer event.
	/// </summary>
	public virtual void 	TranslatPostEvent()
	{
		for(int idx=0; idx<m_dPostEvent.Count; idx++)
		{
			if (m_dPostEvent[idx].DelayTime <= 0.0f)
			{
				// dispatch post event
				DispatchEvent(m_dPostEvent[idx]);
				
				// remove the event
				m_dPostEvent.RemoveAt(idx);
			}
			else
			{
				// delay the event
				m_dPostEvent[idx].DelayTime -= Time.deltaTime;
				
				// reset event time
				if (m_dPostEvent[idx].DelayTime <= 0.0f)
					m_dPostEvent[idx].DelayTime = 0.0f;
			}
		}
	}

	/// <summary>
	/// Active this instance.
	/// </summary>
	public virtual void 	Activation()
	{
		gameObject.SetActive (true);
	}

	/// <summary>
	/// Determines whether this instance is activation.
	/// </summary>
	/// <returns><c>true</c> if this instance is activation; otherwise, <c>false</c>.</returns>
	public virtual bool		IsActive()
	{
		return gameObject.activeSelf;
	}

	/// <summary>
	/// Dective this instance.
	/// </summary>
	public virtual void 	Deactivate()
	{
		gameObject.SetActive (false);
	}

	/// <summary>
	/// Clears the event.
	/// </summary>
	public virtual void 	ClearEvent()
	{
		m_dPostEvent.Clear ();
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	protected virtual void  		FixedUpdate ()
	{
		// update post event
		TranslatPostEvent();
	}
}

