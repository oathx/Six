using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using KSNET;

/// <summary>
/// Virtual net package.
/// </summary>
public class VirtualNetPackage : IEventArgs
{
	/// <summary>
	/// Sets the buffer.
	/// </summary>
	/// <value>The buffer.</value>
	public object[]			buffer
	{ get; set; }
	
	/// <summary>
	/// Initializes a new instance of the <see cref="VirtualNetPackage"/> class.
	/// </summary>
	/// <param name="buffer">Buffer.</param>
	public VirtualNetPackage(object[] buf)
	{
		buffer = buf;
	}
}

/// <summary>
/// Server plugin.
/// </summary>
public class ServerPlugin : IGamePlugin
{
	/// <summary>
	/// Install this instance.
	/// </summary>
	public override void 	Install()
	{
		RegisterObserver<LoginServer>(typeof(LoginServer).Name);
	}
	
	/// <summary>
	/// Unstall this instance.
	/// </summary>
	public override void 	Uninstall()
	{
		UnregisterObserver(typeof(LoginServer).Name);
	}
	
	/// <summary>
	/// Startup this instance.
	/// </summary>
	public override void 	Startup()
	{
		
	}
	
	/// <summary>
	/// Shutdown this instance.
	/// </summary>
	public override void 	Shutdown()
	{
		
	}
}

