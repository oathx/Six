using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

/// <summary>
/// Login server.
/// </summary>
public class TeamServer : VirtualServer
{
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		SubscribeEvent (TcpEvent.CMD_REQ_TEAM, OnTeamReplay);
	}

	/// <summary>
	/// Raises the req team event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool OnTeamReplay(IEvent evt)
	{
		TcpEvent.SCNetTeamReply rep = new TcpEvent.SCNetTeamReply();
		for(int i=1; i<=1; i++)
		{
			TcpEvent.TeamStruct team = new TcpEvent.TeamStruct();
			team.PlayerID	= i;
			team.HP			= 100;
			team.MaxHP		= 100;
			team.MP			= 100;
			team.MaxMP		= 100;
			team.JobID		= Random.Range(1, 6);
			team.Level		= 1;
			team.Name		= string.Empty;

			rep.TeamList.Add(team);
		}

		m_Plugin.PostEvent(
			new IEvent(EngineEventType.EVENT_NET, TcpEvent.CMD_REPLY_TEAM, rep)
			);

		return true;
	}
}
