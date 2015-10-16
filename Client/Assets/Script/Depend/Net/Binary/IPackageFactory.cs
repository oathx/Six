using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

/// <summary>
/// I net message factory.
/// </summary>
public abstract class IPackageFactory
{
	/// <summary>
	/// Creates the net message.
	/// </summary>
	/// <returns>The net message.</returns>
	/// <param name="bytes">Bytes.</param>
	public abstract IPackageArgs	Create (IPackage package);
}

/// <summary>
/// Default net message factory.
/// </summary>
public class DefaultNetMessageFactory<T> : IPackageFactory where T : IPackageArgs, new()
{
	/// <summary>
	/// Creates the net message.
	/// </summary>
	/// <returns>The net message.</returns>
	/// <param name="bytes">Bytes.</param>
	public override IPackageArgs	Create (IPackage package)
	{
		IPackageArgs args = new T ();
		args.Decode (package);

		return args;
	}
}