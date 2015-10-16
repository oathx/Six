using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LitJson;
using UnityThreading;

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
/// Version struct.
/// </summary>
public class VersionStruct
{
	/// <summary>
	/// Version struct.
	/// </summary>
	public class VersionPackage
	{
		public string 	Version;
		public string	Log;
		public string	Url;
	}

	/// <summary>
	/// The status.
	/// </summary>
	public int 			Status;

	/// <summary>
	/// The package.
	/// </summary>
	public List<VersionPackage>
		Package = new List<VersionPackage>();
}

/// <summary>
/// Version observer.
/// </summary>
public class VersionObserver : IEventObserver
{
	/// <summary>
	/// Gets the version.
	/// </summary>
	/// <value>The version.</value>
	public UIVersion		VerUI
	{ get; private set; }

	/// <summary>
	/// Gets the text.
	/// </summary>
	/// <value>The text.</value>
	public string			Text
	{ get; private set; }

	/// <summary>
	/// Gets the persistent data path.
	/// </summary>
	/// <value>The persistent data path.</value>
	public string			PersistentDataPath
	{ get; private set; }

	/// <summary>
	/// Gets the progress.
	/// </summary>
	/// <value>The progress.</value>
	public float			Progress
	{ get; private set; }

	/// <summary>
	/// Active this instance.
	/// </summary>
	public override void 	Active()
	{
		PersistentDataPath = Application.persistentDataPath;

		// load version ui widget
		if (!VerUI)
			VerUI = UISystem.GetSingleton().LoadWidget<UIVersion>(ResourceDef.UI_VERSION);

		// set current version
		VerUI.Version = Version.GetVersion();

		bool bResult = CheckUpdate();
		if (bResult)
		{

		}
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	protected void Update()
	{
		if (VerUI)
		{
			VerUI.Text 		= Text;
			VerUI.Progress	= Progress;
		}
	}
	
	/// <summary>
	/// Detive this instance.
	/// </summary>
	public override void 	Detive()
	{
		UISystem.GetSingleton().UnloadWidget(ResourceDef.UI_VERSION);
	}

	/// <summary>
	/// Checks the update.
	/// </summary>
	/// <returns><c>true</c>, if update was checked, <c>false</c> otherwise.</returns>
	public bool				CheckUpdate()
	{
#if UNITY_EDITOR
		string text = File.ReadAllText(string.Format("{0}/Version.txt", Application.dataPath));
		if (!string.IsNullOrEmpty(text))
		{
			VersionStruct v = JsonMapper.ToObject<VersionStruct>(text);
			if (v.Package != default(List<VersionStruct.VersionPackage>))
			{
				Queue<HttpWork> 
					workQueue = new Queue<HttpWork>();

				foreach(VersionStruct.VersionPackage vp in v.Package)
				{
					workQueue.Enqueue(
						new HttpWork(vp.Url, Application.persistentDataPath, vp.Version, HttpFileType.HFT_ZIP, 0)
						);
				}

				Download(workQueue);
			}
		}
#else
		// To do http request version update
#endif

		return true;
	}

	/// <summary>
	/// Download the specified workQueue and evtCallback.
	/// </summary>
	/// <param name="workQueue">Work queue.</param>
	/// <param name="evtCallback">Evt callback.</param>
	public WorkState	Download(Queue<HttpWork> workQueue)
	{
		if (workQueue.Count <= 0)
			return WorkState.HS_SUCCESS;

		try{
			UnityThreading.ActionThread thread = UnityThreadHelper.CreateThread( ()=> {
				do{
					WorkState curState = HttpDownloadManager.GetSingleton().Download(workQueue.Dequeue(), OnDownloading);
					if (curState != WorkState.HS_FAILURE)
						break;

				}while(workQueue.Count <= 0);

				UnityThreadHelper.Dispatcher.Dispatch( () => {

					SqlTooltip tooltip = GameSqlLite.GetSingleton().Query<SqlTooltip>(TooltipCode.TC_LOADING);
					if (!string.IsNullOrEmpty(tooltip.Text))
					{
						Text = tooltip.Text;
					}

					// start game
					Startup();
				});
			});

		}
		catch(System.Exception e) 
		{
			UISystem.GetSingleton().Box(e.Message);
		}

		return WorkState.HS_SUCCESS;
	}

	/// <summary>
	/// Raises the download finished event.
	/// </summary>
	/// <param name="curState">Current state.</param>
	/// <param name="szUrl">Size URL.</param>
	/// <param name="szPath">Size path.</param>
	/// <param name="nPosition">N position.</param>
	/// <param name="nReadSpeed">N read speed.</param>
	/// <param name="nFilength">N filength.</param>
	/// <param name="szVersion">Size version.</param>
	public bool			OnDownloading(WorkState curState, string szUrl, string szPath,
	                                 int nPosition, int nReadSpeed, int nFileLength, string szVersion)
	{
		SqlTooltip tooltip = GameSqlLite.GetSingleton().Query<SqlTooltip>(TooltipCode.TC_DOWNLOAD);
		if (!string.IsNullOrEmpty(tooltip.Text))
		{
			float fProg = (float)nPosition / (float)nPosition;
			Progress 	= fProg;
			Text 		= string.Format("{0} {1}MB/{2}MB {3}KB ({4})%", tooltip.Text, ToMB(nPosition), 
			                       ToMB(nFileLength), ToKB(nReadSpeed), (int)(fProg * 100));
		}

		// download finished descompress the package to local file system
		if (curState == WorkState.HS_COMPLETED)
		{
			HttpDownloadManager.GetSingleton().Decompression(szPath, 
			                                                 PersistentDataPath, szVersion, OnDecompression);
		}

		return true;
	}

	/// <summary>
	/// Raises the decompression event.
	/// </summary>
	/// <param name="curState">Current state.</param>
	/// <param name="szUrl">Size URL.</param>
	/// <param name="szPath">Size path.</param>
	/// <param name="nPosition">N position.</param>
	/// <param name="nReadSpeed">N read speed.</param>
	/// <param name="nFileLength">N file length.</param>
	/// <param name="szVersion">Size version.</param>
	public bool			OnDecompression(WorkState curState, string szUrl, string szPath,
	                            int nPosition, int nReadSpeed, int nFileLength, string szVersion)
	{
		SqlTooltip tooltip = GameSqlLite.GetSingleton().Query<SqlTooltip>(TooltipCode.TC_DECOMPRESS);
		if (!string.IsNullOrEmpty(tooltip.Text))
		{
			float fProg = (float)nPosition / (float)nPosition;
			Progress 	= fProg;
			Text 		= string.Format("{0} {1}MB/{2}MB {3}KB ({4})%", tooltip.Text, ToMB(nPosition), 
			                       ToMB(nFileLength), ToKB(nReadSpeed), (int)(fProg * 100));
			
		}

		return true;
	}

	/// <summary>
	/// Tos the K.
	/// </summary>
	/// <returns>The K.</returns>
	/// <param name="nBytes">N bytes.</param>
	protected string	ToKB(int nBytes)
	{
		return string.Format("{0:F}", (float)nBytes / 1024);
	}
	
	/// <summary>
	/// Tos the M.
	/// </summary>
	/// <returns>The M.</returns>
	/// <param name="nBytes">N bytes.</param>
	protected string	ToMB(int nBytes)
	{
		return string.Format("{0:F}", 
		                     (float)nBytes / 1024 / 1024);
	}

	/// <summary>
	/// Startup this instance.
	/// </summary>
	protected bool		Startup()
	{
		IResourceManager resMgr = GameEngine.GetSingleton().QueryPlugin<IResourceManager>();
		if (resMgr)
		{
			resMgr.RegisterAssetBundlePackage(WUrl.AssetBundlePath, delegate(string szUrl, AssetBundle abFile) {
				return SceneSupport.GetSingleton().LoadScene((int)SceneFlag.SCENE_LOGIN);
			});
		}

		return true;
	}	
}
