using UnityEngine;
using System.Collections;
using UnityThreading;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using UnityEngine.UI;

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
		IResourceManager resMgr = GameEngine.GetSingleton().QueryPlugin<IResourceManager>();
		if (resMgr)
		{
			resMgr.RegisterAssetBundlePackage(WUrl.AssetBundlePath, delegate(string szUrl, AssetBundle abFile) {

				// install game start resource
				return Install();
			});
		}
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
	protected bool Install()
	{
#if UNITY_EDITOR
		// open sqlite and register database parse factory
		OnOpenAndRegisterSqlFactory ();
#endif

		RegisterEntityCreateFactory ();

		UISystem.GetSingleton().LoadWidget<UIVersion>("Panel");
		return true;
	}

	/// <summary>
	/// Raises the register sql factory event.
	/// </summary>
	protected void OnOpenAndRegisterSqlFactory()
	{
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
	/// Registers the factory.
	/// </summary>
	protected void RegisterEntityCreateFactory()
	{
		EntityShapeFactoryManager.GetSingleton().RegisterFactory<DefaultShapeFactory>(new DefaultShapeFactory());

		// register entity create factory
		IEntityManager entityManager = GameEngine.GetSingleton().QueryPlugin<IEntityManager>();
		if (entityManager)
		{
			entityManager.RegisterEntityFactory(
				typeof(HumanEntityFactory).Name, new HumanEntityFactory()
				);

			entityManager.CreateEntity(typeof(HumanEntityFactory).Name, EntityType.ET_ACTOR, 0, string.Empty, 
			                           Vector3.zero, Vector3.one, Vector3.zero, EntityStyle.ES_PLAYER, 10000); 
		}
	}
}
