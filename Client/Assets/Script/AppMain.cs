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
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () 
	{
		
#if UNITY_EDITOR
		// open sqlite and register database parse factory
		OnOpenAndRegisterSqlFactory ();
#endif

		Install();
	}
	
	// Update is called once per frame
	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update () 
	{
	
	}

	/// <summary>
	/// Installs the resource.
	/// </summary>
	protected void Install()
	{
		string szResourceFilePath = string.Format("{0}/{1}.pkg", WUrl.Url, typeof(AssetBundle).Name);

		// decompress all resource
		DoResourceDecompress(
			szResourceFilePath, Application.persistentDataPath
			);

	}

	/// <summary>
	/// Decompression this instance.
	/// </summary>
	protected void DoResourceDecompress(string szSourcePath, string szTargetPath)
	{
		UnityThreading.ActionThread thread = UnityThreadHelper.CreateThread(() => {
			string szAssetbundlPath = szSourcePath;

			string[] arySplit = szSourcePath.Split(':');
			if (arySplit.Length >= 2)
				szAssetbundlPath = arySplit[arySplit.Length - 1];

			// execute decompress resource
			WorkState curState = HttpDownloadManager.GetSingleton().Decompression(szAssetbundlPath, szTargetPath, 
			                                                                      new HttpWorkEvent(OnDecompressFinished), string.Empty);
			if (curState == WorkState.HS_SUCCESS)
			{
				UnityThreadHelper.Dispatcher.Dispatch( () => {

					// start game
					Startup();
				});
			}
		});

	}

	/// <summary>
	/// Raises the register sql factory event.
	/// </summary>
	protected void OnOpenAndRegisterSqlFactory()
	{
		Debug.Log (WUrl.SqlitePathWin32);
		
#if UNITY_EDITOR_WIN
		GameSqlLite.GetSingleton ().OpenDB (WUrl.SqlitePathWin32, true);
#else
		GameSqlLite.GetSingleton ().OpenDB (WUrl.SqlitePath, true);
#endif

		GameSqlLite.GetSingleton ().RegisterSqlPackageFactory (
			typeof(SqlShape).Name, new DefaultSqlPackageFactory<SqlShape> ()
			);

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
			string szName = typeof(AssetBundle).Name;

			resMgr.RegisterAssetBundlePackage(
				string.Format("{0}/{1}/{2}", szPath, szName, szName)
				);
		}
	}

	/// <summary>
	/// Registers the factory.
	/// </summary>
	protected void RegisterFactory()
	{
		EntityShapeFactoryManager.GetSingleton().RegisterFactory<DefaultShapeFactory>(new DefaultShapeFactory());

		IEntityManager entityManager = GameEngine.GetSingleton().QueryPlugin<IEntityManager>();
		if (!entityManager)
			throw new System.NullReferenceException();

		entityManager.RegisterEntityFactory(
			typeof(HumanEntityFactory).Name, new HumanEntityFactory()
			);
	}

	/// <summary>
	/// Startup this instance.
	/// </summary>
	protected void Startup()
	{
		RegisterResourcePackage();
		RegisterFactory();

		IEntityManager entityManager = GameEngine.GetSingleton().QueryPlugin<IEntityManager>();
		if (!entityManager)
			throw new System.NullReferenceException();

		entityManager.CreateEntity(typeof(HumanEntityFactory).Name, EntityType.ET_ACTOR, 0, "test", 
		                           Vector3.zero, Vector3.one, Vector3.zero, EntityStyle.ES_PLAYER, 10000); 
	}

}
