using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Logic plugin.
/// </summary>
public class LoginPlugin : ITcpSession
{
	/// <summary>
	/// Install this instance.
	/// </summary>
	public override void 		Install()
	{
		// init socket object
		base.Install ();
	}
	
	/// <summary>
	/// Unstall this instance.
	/// </summary>
	public override void 		Uninstall()
	{
		// clear the plugin resource and observer
		Shutdown ();

		// supper call
		base.Uninstall ();
	}
	
	/// <summary>
	/// Startup this instance.
	/// </summary>
	public override void 		Startup()
	{
		RegisterObserver<LoginObserver>(typeof(LoginObserver).Name, true);
	}
	
	/// <summary>
	/// Shutdown this instance.
	/// </summary>
	public override void 		Shutdown()
	{
		UnregisterObserver(typeof(LoginObserver).Name);
	}

}