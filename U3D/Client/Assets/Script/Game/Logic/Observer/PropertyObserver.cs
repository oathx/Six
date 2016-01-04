using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Joystick observer.
/// </summary>
public class PropertyObserver : IEventObserver
{
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		LogicPlugin plugin = GameEngine.GetSingleton ().QueryPlugin<LogicPlugin> ();
		if (plugin)
		{

		}
	}
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{

	}
}
