using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LitJson;
using UnityThreading;

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
		public int		Size;
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

	/// <summary>
	/// Gets the size.
	/// </summary>
	/// <returns>The size.</returns>
	public int 			GetSize()
	{
		int nLength = 0;

		foreach(VersionPackage v in Package)
		{
			nLength += v.Size;
		}

		return nLength;
	}
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
	/// Start this instance.
	/// </summary>
	protected void 			Start() 
	{
		Active();
	}
	
	/// <summary>
	/// Active this instance.
	/// </summary>
	public override void 	Active()
	{
		PersistentDataPath = Application.persistentDataPath;

		// load version ui widget
		if (!VerUI)
			VerUI = UISystem.GetSingleton().LoadWidget<UIVersion>(ResourceDef.UI_VERSION);

		VerUI.Version = GlobalSystemInfo.CurrentVersion;

		// check need update
		CheckVersionAndUpdate();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	protected void 			Update()
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
	public void				CheckVersionAndUpdate()
	{
#if UNITY_EDITOR
		string text = File.ReadAllText(string.Format("{0}/Version.txt", Application.dataPath));
		if (!string.IsNullOrEmpty(text))
		{
			UpdateVersion(text);
		}
#else
		// To do http request version update
#endif
	}

	/// <summary>
	/// Updates the version.
	/// </summary>
	/// <param name="text">Text.</param>
	public void 		UpdateVersion(string text)
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

			string message = string.Format(Tooltip.QueryText(ErrorCode.ERR_VERSION), v.Package[v.Package.Count - 1].Version, ToMB(v.GetSize()));
			UISystem.GetSingleton().Box(BoxStyle.YES, message, 0, 
			                            delegate(bool bFlag, object args) {

				if (bFlag)
				{
					Download(workQueue);
				}

				return true;
			});
		}
		else
		{
			// If the current is the latest version, start the game
			Startup();
		}
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
				while(workQueue.Count > 0)
				{
					WorkState curState = HttpDownloadManager.GetSingleton().Download(workQueue.Dequeue(), OnDownloading);
					if (curState == WorkState.HS_FAILURE)
						break;
				}

				UnityThreadHelper.Dispatcher.Dispatch( () => {

					// update finished
					string text = LogicHelper.GetErrorText(ErrorCode.ERR_LOADING);
					if (!string.IsNullOrEmpty(text))
						Text = text;

					// update finished, start the game
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
		string text = LogicHelper.GetErrorText(ErrorCode.ERR_DOWNLOAD);
		if (!string.IsNullOrEmpty(text))
		{
			float fProg = (float)nPosition / (float)nPosition;
			Progress 	= fProg;
			Text 		= string.Format("{0} {1}MB/{2}MB {3}KB ({4})%", text, ToMB(nPosition), 
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
		string text = LogicHelper.GetErrorText(ErrorCode.ERR_DECOMPRESS);
		if (!string.IsNullOrEmpty(text))
		{
			float fProg = (float)nPosition / (float)nPosition;
			Progress 	= fProg;
			Text 		= string.Format("{0} {1}MB/{2}MB {3}KB ({4})%", text, ToMB(nPosition), 
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
	protected void 		Startup()
	{
		// load assetbundle index file
		IResourceManager resMgr = GameEngine.GetSingleton().QueryPlugin<IResourceManager>();
		if (resMgr)
		{
			resMgr.RegisterAssetBundlePackage(WUrl.AssetBundlePath,
			                                  delegate(string szAssetName, AssetBundleResource abResource) {
				
				// register sql database, all design table
				OpenAndRegisterSqlFactory();
				
				// register all entity create factory
				RegisterUnitEntityFactory();
				
				// notify global plugin change to login
				return LogicHelper.ChangeScene((int)SceneFlag.SCENE_LOGIN);
			});
		}
	}

	/// <summary>
	/// Raises the register sql factory event.
	/// </summary>
	protected void OpenAndRegisterSqlFactory()
	{
#if UNITY_EDITOR
		GameSqlLite.GetSingleton ().OpenDB (WUrl.SqlitePathWin32, true);
#else
		GameSqlLite.GetSingleton ().OpenDB (WUrl.SqlitePath, true);
#endif
		
		GameSqlLite.GetSingleton ().RegisterSqlPackageFactory (
			typeof(SqlShape).Name, new DefaultSqlPackageFactory<SqlShape> ()
			);
		GameSqlLite.GetSingleton ().RegisterSqlPackageFactory (
			typeof(SqlScene).Name, new DefaultSqlPackageFactory<SqlScene> ()
			);
		GameSqlLite.GetSingleton ().RegisterSqlPackageFactory (
			typeof(SqlItem).Name, new DefaultSqlPackageFactory<SqlItem>()
			);
	}

	/// <summary>
	/// Registers the factory.
	/// </summary>
	protected void RegisterUnitEntityFactory()
	{
		EntityShapeFactoryManager.GetSingleton().RegisterFactory<DefaultShapeFactory>(new DefaultShapeFactory());

		// register player create factory
		PlayerManager playerManager = GameEngine.GetSingleton().LoadPlugin<PlayerManager>();
		if (playerManager)
		{
			playerManager.RegisterEntityFactory (
				EntityType.ET_MAIN.ToString (),new HumanEntityFactory ()
				);
			playerManager.RegisterEntityFactory (
				EntityType.ET_PLAYER.ToString (), 	new HumanEntityFactory ()
				);
		}

		// register monster create factory
		MonsterManager monsterManager = GameEngine.GetSingleton ().LoadPlugin<MonsterManager> ();
		if (monsterManager) 
		{
			monsterManager.RegisterEntityFactory (
				EntityType.ET_MONSTER.ToString (), new HumanEntityFactory ()
				);
		}
	}
}
