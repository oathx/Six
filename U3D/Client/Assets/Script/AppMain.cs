using UnityEngine;
using System.Collections;
using UnityThreading;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using UnityEngine.UI;

/// <summary>
/// App main.
/// </summary>
public class AppMain : MonoBehaviour {

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake ()
	{
		Application.targetFrameRate = 60;

		Tooltip.Startup();

#if UNITY_EDITOR
		Caching.CleanCache();
#endif
		// start game engine
		GameEngine.GetSingleton().Startup();
		GameScript.GetSingleton().Startup();
	}

	// Use this for initialization
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () 
	{
		// install version update observer
		IGlobalPlugin global = GameEngine.GetSingleton().QueryPlugin<IGlobalPlugin>();
		if (global)
		{
			global.RegisterObserver<GlobalObserver>(
				typeof(GlobalObserver).Name
				);
			global.RegisterObserver<LoadingObserver>(
				typeof(LoadingObserver).Name
				);
			global.RegisterObserver<SceneObserver>(
				typeof(SceneObserver).Name
				);
			global.RegisterObserver<VersionObserver>(
				typeof(VersionObserver).Name
				);
#if UNITY_EDITOR
			global.RegisterObserver<DebugObserver>(
				typeof(DebugObserver).Name
				);
#endif
		}
	}
}
