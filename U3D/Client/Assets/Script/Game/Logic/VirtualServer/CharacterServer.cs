using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Login server.
/// </summary>
public class CharacterServer : VirtualServer
{
	// Use this for initialization
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start ()
	{
		SubscribeEvent (TcpEvent.CMD_REQ_CHARACTER_LIST, OnReqCharacterInfo);
	}

	/// <summary>
	/// Raises the req character info event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool 	OnReqCharacterInfo(IEvent evt)
	{
		TcpEvent.SCNetCharacterListReply rep = new TcpEvent.SCNetCharacterListReply ();

		List<SqlJob> aryJob = GameSqlLite.GetSingleton().QueryTable<SqlJob>();
		for(int i=0; i<aryJob.Count; i++)
		{
			if (aryJob[i].State != 0)
			{
				TcpEvent.CharacterStruct info = new TcpEvent.CharacterStruct ();
				info.Job		= (sbyte)aryJob[i].ID;
				info.Name		= aryJob[i].Name;
				info.Rank		= 1;
				info.PlayerID	= i;
				info.EquipCount	= 0;

				rep.List.Add(info);
			}
		}

		m_Plugin.SendEvent (
			new IEvent (EngineEventType.EVENT_NET, TcpEvent.CMD_REPLY_CHARACTER_LIST, rep)
			);

		return true;
	}
}
