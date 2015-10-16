using UnityEngine;
using System.Collections;
using UnityThreading;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using UnityEngine.UI;

/// <summary>
/// App main.
/// </summary>
public class AppMain : MonoBehaviour {

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake ()
	{
#if UNITY_EDITOR
		Caching.CleanCache();
#endif
		// start game engine
		GameEngine.GetSingleton().Startup();
		GameScript.GetSingleton().Startup();
		
#if UNITY_EDITOR
		// open sqlite and register database parse factory
		OnOpenAndRegisterSqlFactory ();
#endif
		
		RegisterEntityCreateFactory ();
	}

	// Use this for initialization
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start () 
	{
		GameEngine.GetSingleton().LoadPlugin<ServerPlugin>();
		
		// install version update observer
		IGlobalPlugin global = GameEngine.GetSingleton().QueryPlugin<IGlobalPlugin>();
		if (global)
		{
			global.RegisterObserver<StatusObserver>(typeof(StatusObserver).Name);
			global.RegisterObserver<VersionObserver>(typeof(VersionObserver).Name);
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
		GameSqlLite.GetSingleton ().RegisterSqlPackageFactory (
			typeof(SqlTooltip).Name, new DefaultSqlPackageFactory<SqlTooltip> ()
			);
		GameSqlLite.GetSingleton ().RegisterSqlPackageFactory (
			typeof(SqlScene).Name, new DefaultSqlPackageFactory<SqlScene> ()
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
		}
	}
}
