using UnityEngine;
using System.Collections;
using UnityThreading;
using System.IO;

/// <summary>
/// Version.
/// </summary>
public class Version
{
	public static int 	MainVersion 	= 0;
	public static int	PkgVersion		= 0;
	public static int	CfgVersion		= 0;

	public static string	GetVersion()
	{
		return string.Format("{0}.{1}.{0}", MainVersion, PkgVersion, CfgVersion);
	}
}

/// <summary>
/// App main.
/// </summary>
public class AppMain : MonoBehaviour {

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake ()
	{
		GameEngine.GetSingleton().Startup();
	}

	// Use this for initialization
	void Start () 
	{
		string szAssetbundlePath = string.Format("{0}/{1}.pkg", 
		                                         Application.dataPath, typeof(AssetBundle).Name);
		if (File.Exists(szAssetbundlePath))
		{
			WorkState curState = HttpDownloadManager.GetSingleton().Decompression(szAssetbundlePath, Application.persistentDataPath,
			                                                                      new HttpWorkEvent(OnDecompressFinished), Version.GetVersion());
			if (curState == WorkState.HS_SUCCESS)
			{
				string szDBPath = string.Format("{0}/Design.db", Application.persistentDataPath);
				if (!File.Exists(szDBPath))
				{
					File.Copy(
						string.Format("{0}/Design/Design.db", Application.dataPath), szDBPath);
				}

				GameSqlLite.GetSingleton().OpenDB(szDBPath, true);
			}
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}


	/// <summary>
	/// Raises the decompress finished event.
	/// </summary>
	/// <param name="curState">Current state.</param>
	/// <param name="szUrl">Size URL.</param>
	/// <param name="szPath">Size path.</param>
	/// <param name="nPosition">N position.</param>
	/// <param name="nReadSpeed">N read speed.</param>
	/// <param name="nFilength">N filength.</param>
	/// <param name="szVersion">Size version.</param>
	protected bool OnDecompressFinished(WorkState curState, string szUrl, string szPath,
	                          int nPosition, int nReadSpeed, int nFilength, string szVersion)
	{
		if (curState == WorkState.HS_DECOMPRESS)
		{
		}

		return true;
	}
}
