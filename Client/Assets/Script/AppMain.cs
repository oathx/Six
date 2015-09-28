using UnityEngine;
using System.Collections;
using UnityThreading;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

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
		Install();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	/// <summary>
	/// Installs the resource.
	/// </summary>
	protected void Install()
	{
		DoThreadDecompression(
			string.Format("{0}/{1}.pkg", WUrl.Url, typeof(AssetBundle).Name), Application.persistentDataPath
		);
	}

	/// <summary>
	/// Decompression this instance.
	/// </summary>
	protected void DoThreadDecompression(string szSourcePath, string szTargetPath)
	{
		UnityThreading.ActionThread thread = UnityThreadHelper.CreateThread(() => {
			// packag file path
			string szAssetbundlPath = szSourcePath;

			string[] arySplit = szSourcePath.Split(':');
			if (arySplit.Length >= 2)
				szAssetbundlPath = arySplit[arySplit.Length - 1];
			
			WorkState curState = HttpDownloadManager.GetSingleton().Decompression(szAssetbundlPath, szTargetPath, 
			                                                                      new HttpWorkEvent(OnDecompressFinished), string.Empty);
			if (curState == WorkState.HS_SUCCESS)
			{
				UnityThreadHelper.Dispatcher.Dispatch( () => {
					Startup();
				});
			}
		});

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

	/// <summary>
	/// Registers the resource package.
	/// </summary>
	protected void RegisterResourcePackage()
	{
		IResourceManager resMgr = GameEngine.GetSingleton().QueryPlugin<IResourceManager>();
		if (resMgr)
		{
			string szPath = WUrl.PersistentDataURL.Substring(7, WUrl.PersistentDataURL.Length - 7);
	
			resMgr.RegisterAssetBundlePackage(
				string.Format("{0}/{1}/{2}", szPath, typeof(AssetBundle).Name, typeof(AssetBundle).Name)
				);

			resMgr.LoadFromFile("cy", ResourceLoadFlag.RLF_UNITY, delegate(string szUrl, AssetBundle abFile) {
				GameObject o = abFile.LoadAsset("cy", typeof(GameObject)) as GameObject;

				GameObject.Instantiate(o);
				return true;
			});
		}
	}

	/// <summary>
	/// Startup this instance.
	/// </summary>
	protected void Startup()
	{
		RegisterResourcePackage();


	}
}
