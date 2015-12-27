using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// IAI state.
/// </summary>
public class IGamePlugin : IEventDispatch
{
	/// <summary>
	/// Install this instance.
	/// </summary>
	public virtual void 	Install(){}

	/// <summary>
	/// Unstall this instance.
	/// </summary>
	public virtual void 	Uninstall(){}

	/// <summary>
	/// Startup this instance.
	/// </summary>
	public virtual void 	Startup(){}
	
	/// <summary>
	/// Shutdown this instance.
	/// </summary>
	public virtual void 	Shutdown(){}
}

