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
		RegisterObserver<CharacterServer>(typeof(CharacterServer).Name);
		RegisterObserver<SceneServer>(typeof(SceneServer).Name);
		RegisterObserver<SyncServer>(typeof(SyncServer).Name);
	}
	
	/// <summary>
	/// Shutdown this instance.
	/// </summary>
	public override void 	Shutdown()
	{
		UnregisterObserver(typeof(CharacterServer).Name);
		UnregisterObserver(typeof(SceneServer).Name);
		UnregisterObserver(typeof(SyncServer).Name);
	}
}


