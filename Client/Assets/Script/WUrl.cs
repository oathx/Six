using UnityEngine;

/// <summary>
/// W URL.
/// </summary>
public class WUrl
{
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
	public static string	PersistentDataURL
	{
		get
		{
#if UNITY_STANDALONE || UNITY_EDITOR
			return "file://" + Application.persistentDataPath + "/Win";
#elif UNITY_ANDROID
			return "file://" + Application.persistentDataPath + "/Android";
#elif UNITY_IPHONE
			return "file://" + Application.persistentDataPath + "/iOS";
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
			return string.Format("{0}/{1}.pkg", Url, typeof(AssetBundle).Name);
		}
	}

	public static string	SqlitePathWin32
	{
		get{
			return string.Format("{0}/Design/Design.db", Application.dataPath);
		}
	}
}
