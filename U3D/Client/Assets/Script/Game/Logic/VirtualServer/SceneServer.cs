using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

/// <summary>
/// Login server.
/// </summary>
public class SceneServer : VirtualServer
{
	/// <summary>
	/// Awake this instance.
	/// </summary>
	protected override void Awake ()
	{
		base.Awake();
	}
	
	// Use this for initialization
	void Start ()
	{
		SubscribeEvent (TcpEvent.CMD_REQ_ENTER_WORLD, new EventCallback (OnReqEnterWorld));
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	/// <summary>
	/// Raises the req enter world event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	private bool	OnReqEnterWorld(IEvent evt)
	{
		UICharList charList = UISystem.GetSingleton().Query<UICharList>(
			ResourceDef.UI_CHARLIST
			);
		if (charList)
		{
			UICharItem item = charList.GetSelectItem();
			if (!item)
				throw new System.NullReferenceException();

			VirtualNetPackage vp = evt.Args as VirtualNetPackage;

			TcpEvent.SCNetEnterWorldReply spawn = new TcpEvent.SCNetEnterWorldReply ();
			spawn.Position 	= new Vector3 (-81.0f, 2.0f, 100.0f);
			spawn.Level 	= (short)item.Level;
			spawn.MapID		= 3100;
			spawn.Job 		= (sbyte)item.Job;
			spawn.Name 		= GlobalUserInfo.MapID.ToString();
			
			m_Plugin.SendEvent (
				new IEvent (EngineEventType.EVENT_NET, TcpEvent.CMD_REPLY_ENTER_WORLD, spawn)
				);

		}


		return true;
	}
}


