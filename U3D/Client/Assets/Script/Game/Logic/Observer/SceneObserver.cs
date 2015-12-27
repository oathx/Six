using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using System.Reflection;
using System.IO;
using NLua;

/// <summary>
/// Status observer.
/// </summary>
public class SceneObserver : IEventObserver
{
	public class ScriptDefaultFunc {
		public const string OnSceneStart 	= "OnSceneStart";
		public const string OnSceneUpdate 	= "OnSceneUpdate";
		public const string OnSceneDestroy 	= "OnSceneDestroy";
		public const string OnSceneDialog 	= "OnSceneDialog";
		public const string OnSceneTrigger 	= "OnSceneTrigger";
	}
	
	/// <summary>
	/// The lua function table.
	/// </summary>
	public Dictionary<string, LuaFunction> 
		LuaFunc = new Dictionary<string, LuaFunction>();

	/// <summary>
	/// Gets the loading UI.
	/// </summary>
	/// <value>The loading U.</value>
	public UILoading	LoadingUI
	{ get; private set; }

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		LoadingUI = UISystem.GetSingleton().LoadWidget<UILoading>(ResourceDef.UI_LOADING, false);
		if (!LoadingUI)
			throw new System.NullReferenceException();
	
		SubscribeEvent(CmdEvent.CMD_SCENE_LOADSTART, 	OnSceneStart);
		SubscribeEvent(CmdEvent.CMD_SCENE_LOADING, 		OnSceneLoading);
		SubscribeEvent(CmdEvent.CMD_SCENE_LOADFINISH,	OnSceneFinished);
		SubscribeEvent(CmdEvent.CMD_SCENE_TRIGGER,		OnSceneTrigger);
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		SubscribeEvent(
			CmdEvent.CMD_LOGIC_LOADSCENE, OnLoadingScene
			);
		SubscribeEvent(
			CmdEvent.CMD_UI_DIALOG, OnDialogText
			);
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{
		ExecuteScriptFunction (ScriptDefaultFunc.OnSceneUpdate);
	}
	
	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	void OnDestroy()
	{
		ExecuteScriptFunction (ScriptDefaultFunc.OnSceneDestroy);
	}

	/// <summary>
	/// Raises the dialog text event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool	OnDialogText(IEvent evt)
	{
		CmdEvent.UIDialogTextEventArgs v = evt.Args as CmdEvent.UIDialogTextEventArgs;

		// call lua script
		return ExecuteScriptFunction(
			ScriptDefaultFunc.OnSceneDialog, v
			);
	}
	
	/// <summary>
	/// Raises the load scene event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool	OnLoadingScene(IEvent evt)
	{
		CmdEvent.SceneLoadEventArgs v = evt.Args as CmdEvent.SceneLoadEventArgs;
		try{
			// get the scene config data
			SqlScene sqlScene = GameSqlLite.GetSingleton().Query<SqlScene>(v.SceneID);
			if (!sqlScene)
				throw new System.NullReferenceException();

			if (!string.IsNullOrEmpty(sqlScene.Url))
			{
				List<string> preLoad = new List<string>();
				foreach(string pre in sqlScene.PreLoad)
				{
					preLoad.Add(WUrl.GetUnity3dURL(pre));
				}
				
				string szURL = WUrl.GetUnity3dURL(sqlScene.Url);
				if (!string.IsNullOrEmpty(szURL))
				{
					SceneSupport.GetSingleton().LoadScene(sqlScene.ID, sqlScene.Stream, szURL, sqlScene.AssetName, 
					                                      preLoad, delegate(float fProgress, string szAssetName) {
						return true;
					});
				}
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
	/// Raises the scene start event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool	OnSceneStart(IEvent evt)
	{
		CmdEvent.SceneLoadEventArgs v = evt.Args as CmdEvent.SceneLoadEventArgs;
		try{
			// clear all timer
			GameTimer.GetSingleton().Clearup();

			// call scnen exit event
			ExecuteScriptFunction (ScriptDefaultFunc.OnSceneDestroy);

			// clear all lua function
			if (LuaFunc.Count > 0)
				LuaFunc.Clear();

			// get the scene config data
			SqlScene sqlScene = GameSqlLite.GetSingleton().Query<SqlScene>(v.SceneID);
			if (!sqlScene)
				throw new System.NullReferenceException();

			if (sqlScene.Type == SceneType.SCENE_CITY || 
			    sqlScene.Type == SceneType.SCENE_SINGLE)
			{
				OnUnloadGameObserver(sqlScene);
			}

			// display loading ui
			if (sqlScene.Type != SceneType.SCENE_LOGIN)
				LoadingUI.Show();
		}
		catch(System.Exception e)
		{
			Debug.LogException(e);
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
		if (LoadingUI)
		{
			LoadingUI.Progress = v.Progress;
			LoadingUI.Text		= string.Format("{0}{1}%", LogicHelper.GetErrorText(ErrorCode.ERR_LOADING), (int)(v.Progress * 100));
		}

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
	/// Raises the scene trigger event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool	OnSceneTrigger(IEvent evt)
	{
		CmdEvent.SceneTriggerEventArgs v = evt.Args as CmdEvent.SceneTriggerEventArgs;

		// execute trigger callback
		return ExecuteScriptFunction (
			ScriptDefaultFunc.OnSceneTrigger, v);
	}

	/// <summary>
	/// Raises the scene load finished event.
	/// </summary>
	IEnumerator		OnSceneLoadFinished(CmdEvent.SceneLoadEventArgs result)
	{
		LoadingUI.Progress = result.Progress;
		yield return new WaitForEndOfFrame();
		LoadingUI.Hide();

		OneTimeInit(result.SceneID);
	}

	/// <summary>
	/// Raises the time init event.
	/// </summary>
	/// <param name="nSceneID">N scene I.</param>
	protected bool	OneTimeInit(int nSceneID)
	{
		SqlScene sqlScene = GameSqlLite.GetSingleton().Query<SqlScene>(nSceneID);
		if (!sqlScene)
			throw new System.NullReferenceException();

		switch(sqlScene.Type)
		{
			// if the scene is login, start login plugin, see OnLoginScene
		case SceneType.SCENE_LOGIN:
			OnLoginScene(sqlScene);
			break;

			// if the scene is select character, start login plugin
		case SceneType.SCENE_CHARACTER:
			OnCharacterScene(sqlScene);
			break;

		default:
			OnLoadGameObserver(sqlScene);
			break;
		}

		// load current scene navmesh, If there is navmesh
		if (!string.IsNullOrEmpty(sqlScene.Navmesh))
			SceneSupport.GetSingleton().LoadNavMesh(sqlScene.Navmesh);

		// Start this scenario configuration LUA scripts 
		return OnExecScript(sqlScene.Script);
	}

	/// <summary>
	/// Raises the login scene event.
	/// </summary>
	/// <param name="sqlScene">Sql scene.</param>
	protected bool	OnLoginScene(SqlScene sqlScene)
	{
		// destroy version update server
		Dispatcher.UnregisterObserver(typeof(VersionObserver).Name);
		
		// load login plugin
		LoginPlugin loginPlugin = GameEngine.GetSingleton ().LoadPlugin<LoginPlugin> ();
		if (loginPlugin)
			loginPlugin.Startup ();

		return true;
	}

	/// <summary>
	/// Raises the character scene event.
	/// </summary>
	/// <param name="sqlScene">Sql scene.</param>
	protected bool	OnCharacterScene(SqlScene sqlScene)
	{
		LogicPlugin logicPlugin = GameEngine.GetSingleton().QueryPlugin<LogicPlugin> ();
		if (logicPlugin)
		{
			SqlSystem sqlSystem = GameSqlLite.GetSingleton().Query<SqlSystem>(0);
			if (!string.IsNullOrEmpty(sqlSystem.MainScript))
				OnExecScript(sqlSystem.MainScript);

			CharacterObserver observer = logicPlugin.RegisterObserver<CharacterObserver>(
				typeof(CharacterObserver).Name
				);
			if (observer)
				observer.Active();
		}
		
		return true;
	}

	/// <summary>
	/// Execs the script.
	/// </summary>
	/// <returns><c>true</c>, if script was execed, <c>false</c> otherwise.</returns>
	/// <param name="sqlScene">Sql scene.</param>
	protected bool	OnExecScript(string scriptFile)
	{
		if (string.IsNullOrEmpty(scriptFile))
			return false;

		string scriptText = string.Empty;

#if UNITY_EDITOR
		scriptText = File.ReadAllText(WUrl.GetLuaScriptURL(scriptFile));
#else
		IResourceManager resMgr = GameEngine.GetSingleton().QueryPlugin<IResourceManager>();
		if (!resMgr)
			throw new System.NullReferenceException();

		TextAsset asset = resMgr.GetAsset<TextAsset>(WUrl.AssetLuaPath, scriptFile);
		if (asset)
			scriptText = asset.text;
#endif
	
		if (!string.IsNullOrEmpty(scriptText))
		{
			GameScript.GetSingleton().DoString(scriptText);

			string[] arySplit = scriptFile.Split('.');
			if (arySplit.Length > 0)
			{
				LuaTable table = GameScript.GetSingleton().GetTable(arySplit[0]);
				
				string[] aryFunc = {
					ScriptDefaultFunc.OnSceneStart, 
					ScriptDefaultFunc.OnSceneUpdate,
					ScriptDefaultFunc.OnSceneDestroy,
					ScriptDefaultFunc.OnSceneDialog,
					ScriptDefaultFunc.OnSceneTrigger
				};
				
				for(int i=0; i<aryFunc.Length; i++)
				{
					LuaFunction lf = NLua.Method.LuaClassHelper.GetTableFunction (table, aryFunc[i]);
					if (lf != null && !LuaFunc.ContainsKey(aryFunc[i]))
					{
						LuaFunc.Add(aryFunc[i], lf);
					}
				}
				
				if (LuaFunc.ContainsKey(ScriptDefaultFunc.OnSceneStart))
				{
#if OPEN_DEBUG_LOG
					Debug.Log("Execute scene script start " + scriptFile);
#endif

					GameScript.GetSingleton().Call(
						LuaFunc[ScriptDefaultFunc.OnSceneStart]
						);
				}
			}
		}
		return true;
	}

	
	/// <summary>
	/// Executes the script function.
	/// </summary>
	/// <returns>The script function.</returns>
	/// <param name="szName">Size name.</param>
	protected bool ExecuteScriptFunction(string szName)
	{
		if (!LuaFunc.ContainsKey (szName))
			return false;
		
		GameScript.GetSingleton().Call(
			LuaFunc[szName]
			);
		
		return true;
	}
	
	/// <summary>
	/// Executes the script function.
	/// </summary>
	/// <returns>The script function.</returns>
	/// <param name="szName">Size name.</param>
	/// <param name="args">Arguments.</param>
	protected bool ExecuteScriptFunction(string szName, params System.Object[] args)
	{
		if (!LuaFunc.ContainsKey (szName))
			return false;
		
		GameScript.GetSingleton().Call(
			LuaFunc[szName], args
			);
		
		return true;
	}

	/// <summary>
	/// Registers the observer.
	/// </summary>
	/// <returns><c>true</c>, if observer was registered, <c>false</c> otherwise.</returns>
	/// <param name="aryObserver">Ary observer.</param>
	protected bool RegisterObserver(string[] aryObserver)
	{
		LogicPlugin plugin = GameEngine.GetSingleton().QueryPlugin<LogicPlugin> ();
		if (plugin)
		{
			foreach(string observerName in aryObserver)
			{
				IEventObserver observer = plugin.LoadObserver(observerName);
				if (observer)
					observer.Active();
			}
		}

		return true;
	}

	/// <summary>
	/// Unregisters the observer.
	/// </summary>
	/// <returns><c>true</c>, if observer was unregistered, <c>false</c> otherwise.</returns>
	/// <param name="aryObserverName">Ary observer name.</param>
	protected bool	UnregisterObserver(string[] aryObserver)
	{
		LogicPlugin plugin = GameEngine.GetSingleton().QueryPlugin<LogicPlugin> ();
		if (plugin)
		{
			foreach(string observer in aryObserver)
			{
				plugin.UnregisterObserver(observer);
			}
		}

		return true;
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
	/// Raises the city scene event.
	/// </summary>
	/// <param name="sqlScene">Sql scene.</param>
	protected bool	OnLoadGameObserver(SqlScene sqlScene)
	{
		List<string> aryObserver = new List<string>(){
			typeof(MainObserver).Name,
			typeof(SyncObserver).Name,
			typeof(PropertyObserver).Name
		};

		return RegisterObserver(aryObserver.ToArray());
	}
	
	/// <summary>
	/// Raises the game scene shutdown event.
	/// </summary>
	/// <param name="sqlScene">Sql scene.</param>
	protected bool	OnUnloadGameObserver(SqlScene sqlScene)
	{
		List<string> aryObserver = new List<string>(){
			typeof(CharacterObserver).Name,
			typeof(MainObserver).Name,
			typeof(SyncObserver).Name,
		};

		return UnregisterObserver(aryObserver.ToArray());
	}
}


