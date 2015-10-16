using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Login.
/// </summary>
public class Login : MonoBehaviour
{
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		IGlobalPlugin global = GameEngine.GetSingleton().QueryPlugin<IGlobalPlugin>();
		if (global)
			global.UnregisterObserver(typeof(VersionObserver).Name);

		LoginPlugin plugin = GameEngine.GetSingleton ().LoadPlugin<LoginPlugin> ();
		if (plugin)
			plugin.Startup ();
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{

	}
}
