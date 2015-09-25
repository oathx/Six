using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// IAI state.
/// </summary>
abstract public class IGamePlugin : IEventDispatch
{
	/// <summary>
	/// Install this instance.
	/// </summary>
	public abstract void 	Install();

	/// <summary>
	/// Unstall this instance.
	/// </summary>
	public abstract void 	Uninstall();

	/// <summary>
	/// Startup this instance.
	/// </summary>
	public abstract void 	Startup();
	
	/// <summary>
	/// Shutdown this instance.
	/// </summary>
	public abstract void 	Shutdown();
}

