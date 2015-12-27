using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using KSNET;

/// <summary>
/// I net message.
/// </summary>
public class IPackageArgs : IEventArgs
{
	/// <summary>
	/// Decode the specified package.
	/// </summary>
	/// <param name="package">Package.</param>
	public virtual void 	Decode(IPackage package)
	{

	}
}
