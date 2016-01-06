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
	public const int TEAM_MAXUNIT	= 4;

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
		int[] aryDefault = {
			4, 3, 2, 5
		};

		TcpEvent.SCNetTeamReply rep = new TcpEvent.SCNetTeamReply();
		for(int idx=0; idx<TEAM_MAXUNIT; idx++)
		{
			TcpEvent.TeamStruct team = new TcpEvent.TeamStruct();
			team.PlayerID	= idx + 1;
			team.HP			= 100;
			team.MaxHP		= 100;
			team.MP			= 100;
			team.MaxMP		= 100;
			team.JobID		= aryDefault[idx];
			team.Level		= idx;
			team.Name		= string.Empty;

			rep.TeamList.Add(team);
		}

		m_Plugin.PostEvent(
			new IEvent(EngineEventType.EVENT_NET, TcpEvent.CMD_REPLY_TEAM, rep)
			);

		return true;
	}
}
