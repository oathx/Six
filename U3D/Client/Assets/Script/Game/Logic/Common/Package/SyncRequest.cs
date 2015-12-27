using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Login request.
/// </summary>
public class SyncRequest : SimpleSingleton<SyncRequest>
{
	/// <summary>
	/// The m_ session.
	/// </summary>
	protected LogicPlugin	m_pPlugin;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="LoginRequest"/> class.
	/// </summary>
	public SyncRequest()
	{
		m_pPlugin = GameEngine.GetSingleton ().QueryPlugin<LogicPlugin> ();
		if (!m_pPlugin)
			throw new System.NullReferenceException ();
	}

	/// <summary>
	/// Reqs the attack.
	/// </summary>
	/// <param name="nMagicID">N magic I.</param>
	/// <param name="fAangle">F aangle.</param>
	/// <param name="nActionID">N action I.</param>
	/// <param name="bTargetPlayer">If set to <c>true</c> b target player.</param>
	/// <param name="nTargetID">N target I.</param>
	public void 	ReqAttack(int nMagicID, float fAangle, int nActionID, bool bTargetPlayer, int nTargetID)
	{
		m_pPlugin.SendEvent(TcpEvent.CMD_REQ_PLAYER_SKILL,
		                    GlobalUserInfo.City == SceneType.SCENE_SINGLE, nMagicID , fAangle, (short)nActionID, bTargetPlayer, nTargetID);
	}

	/// <summary>
	/// Reqs the system time.
	/// </summary>
	public void 	ReqSystemTime()
	{
		m_pPlugin.SendEvent(TcpEvent.CMD_REQ_SYSTEM_TIME, GlobalUserInfo.City == SceneType.SCENE_SINGLE,
		                    System.Convert.ToInt32((Time.time * 1000)));
	}

	
	/// <summary>
	/// Reqs the player move.
	/// </summary>
	/// <param name="vPosition">V position.</param>
	/// <param name="fAngle">F angle.</param>
	/// <param name="isMove">If set to <c>true</c> is move.</param>
	public void 	ReqPlayerMove(Vector3 vPosition, float fAngle, bool isMove)
	{
		// send player move message
		m_pPlugin.SendEvent(TcpEvent.CMD_REQ_PLAYER_MOVE, GlobalUserInfo.City == SceneType.SCENE_SINGLE,
		                    GlobalUserInfo.MapID, vPosition.x, vPosition.y, vPosition.z, fAngle, isMove);
	}
}
