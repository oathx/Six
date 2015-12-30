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
#if UNITY_EDITOR || UNITY_STANDALONE
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
#if UNITY_EDITOR || UNITY_STANDALONE
			return Application.dataPath + "/Temp/Win";
#elif UNITY_ANDROID
			return Application.persistentDataPath;
#elif UNITY_IPHONE
			return Application.persistentDataPath;
#endif
		}
	}

	/// <summary>
	/// Gets the asset character path.
	/// </summary>
	/// <value>The asset character path.</value>
	public static string	AssetCharacterPath
	{
		get
		{
#if UNITY_EDITOR || UNITY_STANDALONE
			return string.Format("{0}/resource/model.unity3d", DataURL);
#else
			return string.Format("{0}/resource/model.unity3d", DataURL);
#endif
		}
	}

	public static string	AssetMonsterPath
	{
		get
		{
#if UNITY_EDITOR || UNITY_STANDALONE
			return string.Format("{0}/resource/monster.unity3d", DataURL);
#else
			return string.Format("{0}/resource/monster.unity3d", DataURL);
#endif
		}
	}

	/// <summary>
	/// Gets the asset arm path.
	/// </summary>
	/// <value>The asset arm path.</value>
	public static string	AssetArmPath
	{
		get
		{
#if UNITY_EDITOR || UNITY_STANDALONE
			return string.Format("{0}/resource/weapon.unity3d", DataURL);
#else
			return string.Format("{0}/resource/weapon.unity3d", DataURL);
#endif
		}
	}

	public static string	AssetLuaPath
	{
		get
		{
#if UNITY_EDITOR || UNITY_STANDALONE
			return string.Format("{0}/resource/luascript.unity3d", DataURL);
#else
			return string.Format("{0}/resource/luascript.unity3d", DataURL);
#endif
		}
	}

	public static string	AssetNavmeshPath
	{
		get
		{
#if UNITY_EDITOR || UNITY_STANDALONE
			return string.Format("{0}/resource/navmesh.unity3d", DataURL);
#else
			return string.Format("{0}/resource/navmesh.unity3d", DataURL);
#endif
		}
	}

	/// <summary>
	/// Gets the asset pass path.
	/// </summary>
	/// <value>The asset pass path.</value>
	public static string	AssetPassPath
	{
		get
		{
#if UNITY_EDITOR || UNITY_STANDALONE
			return string.Format("{0}/resource/pass.unity3d", DataURL);
#else
			return string.Format("{0}/resource/pass.unity3d", DataURL);
#endif
		}
	}

	/// <summary>
	/// Gets the sqlite path win32.
	/// </summary>
	/// <value>The sqlite path win32.</value>
	public static string	SqlitePathWin32
	{
		get
		{
			return string.Format("{0}/Design/Design.bytes", Application.dataPath);
		}
	}

	/// <summary>
	/// Gets the sqlite path.
	/// </summary>
	/// <value>The sqlite path.</value>
	public static string	SqlitePath
	{
		get
		{
			return string.Format("{0}/Single/Design.bytes",
			                     Application.persistentDataPath);
		}
	}

	/// <summary>
	/// Gets the unity3d UR.
	/// </summary>
	/// <returns>The unity3d UR.</returns>
	/// <param name="szName">Size name.</param>
	public static string	GetUnity3dURL(string szName)
	{
#if UNITY_EDITOR || UNITY_STANDALONE
		return string.Format("{0}/{1}", DataURL, szName);
#else
		return string.Format("{0}/{1}", Application.persistentDataPath, szName);
#endif
	}

	/// <summary>
	/// Gets the lua script UR.
	/// </summary>
	/// <returns>The lua script UR.</returns>
	/// <param name="szName">Size name.</param>
	public static string	GetLuaScriptURL(string szName)
	{
		return string.Format("{0}/LuaScript/{1}", Application.dataPath, szName);
	}
}
