using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SceneFlag {
	SCENE_LOGIN		= 1,
	SCENE_CHARACTER = 2,
	SCENE_PLAGING	= 3,
}

public delegate bool SceneLoadingCallback(float fProgress, string szAssetName);

/// <summary>
/// Game engine.
/// </summary>
public class SceneSupport : MonoBehaviourSingleton<SceneSupport>
{
	/// <summary>
	/// The m_ resource manager.
	/// </summary>
	protected IResourceManager	m_ResourceManager;

	/// <summary>
	/// Initializes a new instance of the <see cref="SceneSupport"/> class.
	/// </summary>
	protected void Awake()
	{
		m_ResourceManager = GameEngine.GetSingleton ().QueryPlugin<IResourceManager> ();
		if (!m_ResourceManager)
			throw new System.NullReferenceException ("Can't find resource manager");
	}

	/// <summary>
	/// Sends the scene event.
	/// </summary>
	/// <param name="szName">Size name.</param>
	protected void 			SendEvent(int nSceneID, int nEventID, float fProgress)
	{
		CmdEvent.SceneLoadEventArgs v = new CmdEvent.SceneLoadEventArgs ();
		v.SceneID 	= nSceneID;
		v.Progress 	= fProgress;
		
		GameEngine.GetSingleton ().SendEvent (new IEvent (nEventID, v));
	}
	
	/// <summary>
	/// Gets the async.
	/// </summary>
	/// <value>The async.</value>
	public AsyncOperation	Async 	{get; private set;}
	public int 				SceneID	{get; private set;}

	/// <summary>
	/// Loads the scene.
	/// </summary>
	/// <returns><c>true</c>, if scene was loaded, <c>false</c> otherwise.</returns>
	/// <param name="szName">Size name.</param>
	public virtual bool		LoadScene(int nSceneID, string szName)
	{
		try{
			m_ResourceManager.LoadAssetBundleFromStream(szName.ToLower(), 
			                                            delegate(string szAssetName, AssetBundleResource abResource) {
#if OPEN_DEBUG_LOG
				string[] aryAssetName = abResource.GetAllAssetNames();
				foreach(string asset in aryAssetName)
				{
					Debug.Log("Scene asset name : " + asset);
				}
#endif
				// save current scene id
				SceneID = nSceneID;

				// start load scene
				StartCoroutine(
					OnAsyncLoadScene(szName, default(SceneLoadingCallback))
					);

				return true;
			});
		}
		catch(System.Exception e)
		{
			Debug.LogException(e);
		}

		return true;
	}
	

	/// <summary>
	/// Loads the scene.
	/// </summary>
	/// <returns><c>true</c>, if scene was loaded, <c>false</c> otherwise.</returns>
	/// <param name="nSceneID">N scene I.</param>
	/// <param name="aryPreLoad">Ary pre load.</param>
	public virtual bool		LoadScene(int nSceneID, string szName, List<string> aryPreLoad, SceneLoadingCallback callback)
	{
		SendEvent(nSceneID, CmdEvent.CMD_SCENE_LOADSTART, 0);

		// Load the resource in the pre load
		for(int i=0; i<aryPreLoad.Count; i++)
		{
			m_ResourceManager.LoadAssetBundleFromStream(aryPreLoad[i], 
			                               delegate(string szAssetName, AssetBundleResource abResource) {
				float fProgress = (float)i / (float)aryPreLoad.Count;
				return callback(fProgress, aryPreLoad[i]);
			});
		}
	
		return LoadScene(nSceneID, szName);
	}

	/// <summary>
	/// Raises the async load scene event.
	/// </summary>
	/// <param name="szName">Size name.</param>
	/// <param name="callback">Callback.</param>
	IEnumerator		OnAsyncLoadScene(string szName, SceneLoadingCallback callback)
	{
		Async = Application.LoadLevelAsync(szName);
		yield return Async;

		if (Async.isDone)
			SendEvent(SceneID, CmdEvent.CMD_SCENE_LOADFINISH, Async.progress);
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	protected void 	Update()
	{
		if (Async != default(AsyncOperation) && !Async.isDone)
		{
			SendEvent(SceneID, CmdEvent.CMD_SCENE_LOADING, Async.progress);
		}
	}
}

