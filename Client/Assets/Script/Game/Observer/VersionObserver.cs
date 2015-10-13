using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LitJson;

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
			return true;

		List<HttpWork> 
			aryWork = new List<HttpWork>();

		foreach(VersionStruct.VersionPackage p in v.Package)
		{
			aryWork.Add
				(new HttpWork(p.Url, Application.persistentDataPath, p.Version, HttpFileType.HFT_ZIP, 0)
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
		if (curState == WorkState.HS_FAILURE)
		{
			UISystem.GetSingleton().Box(szUrl);
		}
		return true;
	}
}
