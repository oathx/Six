using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Login server.
/// </summary>
public class SyncServer : VirtualServer
{
	// Use this for initialization
	void Start ()
	{
		SubscribeEvent (TcpEvent.CMD_REQ_PLAYER_SKILL, OnReqPlayerSkill);
		SubscribeEvent (TcpEvent.CMD_REQ_SYSTEM_TIME,  OnReqServerTime);
	}

	/// <summary>
	/// Raises the req player skill event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool OnReqPlayerSkill(IEvent evt)
	{
		VirtualNetPackage vp = evt.Args as VirtualNetPackage;

		int nMagicID 		= (int)vp.buffer [0];
		float fAngle 		= (float)vp.buffer [1];
		short nActionID 	= (short)vp.buffer [2];
		bool bTargetPlayer 	= (bool)vp.buffer [3];
		int nTargetID 		= (int)vp.buffer [4];
		
		SqlMagic sqlMagic 	= GameSqlLite.GetSingleton ().Query<SqlMagic> (nMagicID);
		if (sqlMagic)
		{
			TcpEvent.SCNetSkill v = new TcpEvent.SCNetSkill ();
			v.SkillID 			= nActionID;
			v.Angle 			= fAngle;
			v.AttackerID 		= GlobalUserInfo.PlayerID;
			v.IsAttackerPlayer 	= true;
			v.IsTargetPlayer 	= bTargetPlayer;
			v.MagicID 			= nMagicID;
			v.TargetID 			= nTargetID;
			v.Timestamp 		= Time.time;
			
			m_Plugin.SendEvent (
				new IEvent (EngineEventType.EVENT_NET, TcpEvent.CMD_PUSH_PLAYER_SKILL, v)
				);
		}

		return true;
	}

	/// <summary>
	/// Raises the req server time event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	private bool		OnReqServerTime(IEvent evt)
	{
		VirtualNetPackage vp = evt.Args as VirtualNetPackage;
		
		// rep server time
		TcpEvent.SCNetSystemTime rep = new TcpEvent.SCNetSystemTime ();
		rep.ClientTime = System.Convert.ToSingle((int)vp.buffer [0]) * 0.001f;
		rep.ServerTime = Time.time;
		
		m_Plugin.SendEvent (
			new IEvent (EngineEventType.EVENT_NET, TcpEvent.CMD_PUSH_SYSTEM_TIME, rep)
			);
		
		return true;
	}
}



