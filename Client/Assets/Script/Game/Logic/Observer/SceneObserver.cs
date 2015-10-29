using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using System.Reflection;

/// <summary>
/// Status observer.
/// </summary>
public class SceneObserver : IEventObserver
{
	public UILoading	LoadUI
	{ get; private set; }

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		LoadUI = UISystem.GetSingleton().LoadWidget<UILoading>(ResourceDef.UI_LOADING, false);
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		SubscribeEvent(CmdEvent.CMD_LOGIC_LOADSCENE, 	OnLoadingScene);
		SubscribeEvent(CmdEvent.CMD_SCENE_LOADING, 		OnSceneLoading);
		SubscribeEvent(CmdEvent.CMD_SCENE_LOADFINISH,	OnSceneFinished);
	}

	/// <summary>
	/// Raises the load scene event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool	OnLoadingScene(IEvent evt)
	{
		CmdEvent.SceneLoadEventArgs v = evt.Args as CmdEvent.SceneLoadEventArgs;
		try{
		
			SqlScene sqlScene = GameSqlLite.GetSingleton().Query<SqlScene>(v.SceneID);
			if (!sqlScene)
				throw new System.NullReferenceException();

			// display loading ui
			if (v.SceneID != (int)SceneFlag.SCENE_LOGIN)
				LoadUI.Show();

			if (v.SceneID > (int)SceneFlag.SCENE_CHARACTER)
			{
				LogicPlugin plugin = GameEngine.GetSingleton().QueryPlugin<LogicPlugin> ();
				if (plugin)
				{
					// deative observer
					string[] aryDetive = {
						typeof(CharacterObserver).Name
					};
					foreach(string detive in aryDetive)
					{
						IEventObserver observer = plugin.QueryObserver(detive);
						if (observer)
							observer.Detive();
					}
				}

			}

			// If this scenario is a stream file
			if (sqlScene.Stream)
			{
				if (!string.IsNullOrEmpty(sqlScene.Url))
				{
					SceneSupport.GetSingleton().LoadScene(sqlScene.ID, sqlScene.Url, 
					                                      new List<string>(), new SceneLoadingCallback(OnSceneLoadingCallback));
				}
			}
			else
			{
				Application.LoadLevel(sqlScene.Url);
			}

		}
		catch(System.Exception e)
		{
			UISystem.GetSingleton().Box(e.Message);

#if OPEN_DEBUG_LOG
			Debug.LogException(e);
#endif
		}

		return true;
	}

	/// <summary>
	/// Raises the scene loading event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool	OnSceneLoading(IEvent evt)
	{
		CmdEvent.SceneLoadEventArgs v = evt.Args as CmdEvent.SceneLoadEventArgs;

		// display loading progress
		if (LoadUI)
			LoadUI.Progress = v.Progress;

		return true;
	}

	/// <summary>
	/// Raises the scene finished event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool	OnSceneFinished(IEvent evt)
	{
		CmdEvent.SceneLoadEventArgs result = evt.Args as CmdEvent.SceneLoadEventArgs;

		// wait a frame, destroy loading ui and ready oather resource
		StartCoroutine(
			OnSceneLoadFinished(result)
			);
		
		return true;
	}

	/// <summary>
	/// Raises the scene load finished event.
	/// </summary>
	IEnumerator		OnSceneLoadFinished(CmdEvent.SceneLoadEventArgs result)
	{
		// display loading progress
		if (LoadUI)
		{
			LoadUI.Progress = result.Progress;
		}

		yield return new WaitForEndOfFrame();

		if (LoadUI)
			LoadUI.Hide();

		Debug.Log((SceneFlag)result.SceneID);

		switch((SceneFlag)result.SceneID)
		{
		case SceneFlag.SCENE_LOGIN:

			// destroy version update server
			Dispatcher.UnregisterObserver(typeof(VersionObserver).Name);

			// load login plugin
			LoginPlugin loginPlugin = GameEngine.GetSingleton ().LoadPlugin<LoginPlugin> ();
			if (loginPlugin)
				loginPlugin.Startup ();

			break;

		case SceneFlag.SCENE_CHARACTER:

			LogicPlugin logicPlugin = GameEngine.GetSingleton().QueryPlugin<LogicPlugin> ();
			if (logicPlugin)
			{
				CharacterObserver observer = logicPlugin.QueryObserver<CharacterObserver>(
					typeof(CharacterObserver).Name
					);
				if (observer)
					observer.Active();
			}

			break;

		default:
			/*
			 * install game observer, load game scene 
			 */
			Install(result);

			break;
		}
	}

	/// <summary>
	/// Raises the scene loading callback event.
	/// </summary>
	/// <param name="fProgress">F progress.</param>
	/// <param name="szAssetName">Size asset name.</param>
	protected bool	OnSceneLoadingCallback(float fProgress, string szAssetName)
	{
		return true;
	}

	/// <summary>
	/// Install this instance.
	/// </summary>
	protected bool	Install(CmdEvent.SceneLoadEventArgs result)
	{
		LogicPlugin plugin = GameEngine.GetSingleton().QueryPlugin<LogicPlugin> ();
		if (plugin)
		{
			// deative observer
			string[] aryDetive = {
				typeof(CharacterObserver).Name
			};
			foreach(string detive in aryDetive)
			{
				IEventObserver observer = plugin.QueryObserver(detive);
				if (observer)
					observer.Detive();
			}

			// install open system
			InstallSystem();
		}

		return true;
	}

	/// <summary>
	/// Installs the system function.
	/// </summary>
	/// <returns><c>true</c>, if system function was installed, <c>false</c> otherwise.</returns>
	protected bool	InstallSystem()
	{
		DataTable table = GameSqlLite.GetSingleton ().QueryTable (typeof(SqlSpread).Name);
		foreach(DataRow row in table.Rows)
		{
			SqlSpread spread = new SqlSpread();
			spread.Fill(row);
			
			if (spread.State != 0 && !string.IsNullOrEmpty(spread.Observer))
			{
				CmdEvent.OpenSystemEventArys v = new CmdEvent.OpenSystemEventArys();
				v.ID 	= spread.ID;
				
				GameEngine.GetSingleton().SendEvent(
					new IEvent(EngineEventType.EVENT_USER, CmdEvent.EVT_LOGIC_OPENSYSTEM, v)
					);
			}
		}
		
		return true;
	}

}


