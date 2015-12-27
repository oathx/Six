using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AI;

/// <summary>
/// Login server.
/// </summary>
public class AIServer : VirtualServer
{
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{
		AIBehaviourTreeManager.GetSingleton().Update();
	}
}
