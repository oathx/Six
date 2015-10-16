using UnityEngine;

/// <summary>
/// W URL.
/// </summary>
public class WUrl
{
	public static string	IPAddress 	= "0.0.0.0";
	public static int 		Port		= 20000;

	/// <summary>
	/// Gets the root UR.
	/// </summary>
	/// <value>The root UR.</value>
	public static string	Url
	{
		get
		{
#if UNITY_STANDALONE || UNITY_EDITOR
			return "file://" + Application.streamingAssetsPath + "/Win";
#elif UNITY_ANDROID
			return	"jar:file://" + Application.dataPath + "!/assets/android";
#elif UNITY_IPHONE
			return "file://" + Application.streamingAssetsPath + "/iOS";
#endif
		}
	}

	/// <summary>
	/// Gets the persistent data UR.
	/// </summary>
	/// <value>The persistent data UR.</value>
	public static string	DataURL
	{
		get
		{
#if UNITY_STANDALONE || UNITY_EDITOR
			return Application.persistentDataPath;
#elif UNITY_ANDROID
			return Application.persistentDataPath;
#elif UNITY_IPHONE
			return Application.persistentDataPath;
#endif
		}
	}

	/// <summary>
	/// Gets the archive path.
	/// </summary>
	/// <value>The archive path.</value>
	public static string	AssetBundlePath
	{
		get{
#if UNITY_EDITOR
			return string.Format("{0}/{1}/{2}", DataURL, typeof(AssetBundle).Name, typeof(AssetBundle).Name);
#else
			return string.Format("{0}/{1}/{2}", DataURL, typeof(AssetBundle).Name,  typeof(AssetBundle).Name);
#endif
		}
	}

	/// <summary>
	/// Gets the asset bundle package.
	/// </summary>
	/// <value>The asset bundle package.</value>
	public static string	AssetBundlePkg
	{
		get{
			return string.Format("{0}/AssetBundle.pkg", WUrl.Url);
		}
	}

	/// <summary>
	/// Gets the sqlite path win32.
	/// </summary>
	/// <value>The sqlite path win32.</value>
	public static string	SqlitePathWin32
	{
		get{
			return string.Format("{0}/Design/Design.db", Application.dataPath);
		}
	}

	/// <summary>
	/// Gets the sqlite path.
	/// </summary>
	/// <value>The sqlite path.</value>
	public static string	SqlitePath
	{
		get{
			return string.Format("{0}/Design/Design.db", Application.persistentDataPath);
		}
	}
}
