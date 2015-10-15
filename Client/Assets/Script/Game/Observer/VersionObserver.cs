using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LitJson;

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
	{ get; private set;}

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
		// load version ui widget
		if (!VerUI)
			VerUI = UISystem.GetSingleton().LoadWidget<UIVersion>(ResourceDef.UI_VERSION);

		// set current version
		VerUI.Version = Version.GetVersion();

#if UNITY_EDITOR
		string text = File.ReadAllText(Application.dataPath + "/Debug/Version.txt");
		if (!string.IsNullOrEmpty(text))
			OnVersionUpdate(text);
#endif
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
	/// Raises the parse version update event.
	/// </summary>
	/// <param name="text">Text.</param>
	public bool				OnVersionUpdate(string text)
	{
		VersionStruct v = JsonMapper.ToObject<VersionStruct>(text);
		if (v.Package == null)
			return Startup();

		List<HttpWork> 
			aryWork = new List<HttpWork>();

		foreach(VersionStruct.VersionPackage p in v.Package)
		{
			string szName = HttpDownloadManager.GetSingleton().GetFileName(p.Url);
			string szPath = string.Format("{0}/{1}", 
			                              Application.persistentDataPath, szName);

			aryWork.Add(
				new HttpWork(p.Url, szPath, p.Version, HttpFileType.HFT_ZIP, 0)
				 );
		}

		HttpDownloadManager.GetSingleton().Download(aryWork, new HttpWorkEvent(OnHttpWorkDownload));

		return true;
	}

	/// <summary>
	/// Raises the http work download event.
	/// </summary>
	/// <param name="curState">Current state.</param>
	/// <param name="szUrl">Size URL.</param>
	/// <param name="szPath">Size path.</param>
	/// <param name="nPosition">N position.</param>
	/// <param name="nReadSpeed">N read speed.</param>
	/// <param name="nFilength">N filength.</param>
	/// <param name="szVersion">Size version.</param>
	public bool 	OnHttpWorkDownload(WorkState curState, string szUrl, string szPath,
	                    int nPosition, int nReadSpeed, int nFilength, string szVersion)
	{
		switch(curState)
		{
		case WorkState.HS_FAILURE:
			UISystem.GetSingleton().Box(szUrl);
			break;

		case WorkState.HS_DECOMPRE:
			OnDecompression(szPath, nPosition, nReadSpeed, nFilength);
			break;

		case WorkState.HS_COMPLETED:
			break;

		case WorkState.HS_DOWNLOAD:
			OnDownloading(szPath, nPosition, nReadSpeed, nFilength);
			break;

		case WorkState.HS_FINISHED:
			OnFinished(szPath, nPosition, nReadSpeed, nFilength, szVersion);
			break;
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
	/// Raises the decompression event.
	/// </summary>
	/// <param name="szPath">Size path.</param>
	/// <param name="nPosition">N position.</param>
	/// <param name="nReadSpeed">N read speed.</param>
	/// <param name="nFileLength">N file length.</param>
	public bool		OnDecompression(string szPath, int nPosition, int nReadSpeed, int nFileLength)
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
	/// Raises the downloading event.
	/// </summary>
	/// <param name="szPath">Size path.</param>
	/// <param name="nPosition">N position.</param>
	/// <param name="nReadSpeed">N read speed.</param>
	/// <param name="nFileLength">N file length.</param>
	public bool		OnDownloading(string szPath, int nPosition, int nReadSpeed, int nFileLength)
	{
		SqlTooltip tooltip = GameSqlLite.GetSingleton().Query<SqlTooltip>(TooltipCode.TC_DOWNLOAD);
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
	/// Raises the finished event.
	/// </summary>
	/// <param name="szPath">Size path.</param>
	/// <param name="nPosition">N position.</param>
	/// <param name="nReadSpeed">N read speed.</param>
	/// <param name="nFileLength">N file length.</param>
	public bool		OnFinished(string szPath, int nPosition, int nReadSpeed, int nFileLength, string szVersion)
	{
		SqlTooltip tooltip = GameSqlLite.GetSingleton().Query<SqlTooltip>(TooltipCode.TC_LOADING);
		if (!string.IsNullOrEmpty(tooltip.Text))
		{
			VerUI.Text 		= tooltip.Text;
			VerUI.Version	= szVersion;
		}

		return Startup();
	}

	/// <summary>
	/// Startup this instance.
	/// </summary>
	protected bool	Startup()
	{
		IResourceManager resMgr = GameEngine.GetSingleton().QueryPlugin<IResourceManager>();
		if (resMgr)
		{
			resMgr.RegisterAssetBundlePackage(WUrl.AssetBundlePath, delegate(string szUrl, AssetBundle abFile) {
				return SceneSupport.GetSingleton().LoadScene(1);
			});
		}

		return true;
	}	
}
