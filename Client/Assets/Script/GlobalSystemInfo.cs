using UnityEngine;

/// <summary>
/// Version.
/// </summary>
public class Version
{
	public static int 	MainVersion 	= 0;
	public static int	PkgVersion		= 0;
	public static int	CfgVersion		= 0;
	
	/// <summary>
	/// Gets the version.
	/// </summary>
	/// <returns>The version.</returns>
	public static string	GetVersion()
	{
		return string.Format("{0}.{1}.{0}", MainVersion, PkgVersion, CfgVersion);
	}
}

/// <summary>
/// Global system info.
/// </summary>
public class GlobalSystemInfo
{
	public const string	IPAddress 	= "14.17.65.231";
	public const int 	Port		= 20002;

	/// <summary>
	/// Gets or sets the current version.
	/// </summary>
	/// <value>The current version.</value>
	public static string CurrentVersion
	{
		get{
			return PlayerPrefs.GetString("kingkong_client_version", Version.GetVersion());
		}
		set{
			PlayerPrefs.SetString("kingkong_client_version", value);
		}
	}
}