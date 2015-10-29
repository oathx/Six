using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KSNET;

/// <summary>
/// Server plugin.
/// </summary>
public class ServerPlugin : IGamePlugin
{
	/// <summary>
	/// Startup this instance.
	/// </summary>
	public override void 	Startup()
	{
		RegisterObserver<SceneServer>(typeof(SceneServer).Name);
	}
	
	/// <summary>
	/// Shutdown this instance.
	/// </summary>
	public override void 	Shutdown()
	{
		UnregisterObserver(typeof(SceneServer).Name);
	}
}


