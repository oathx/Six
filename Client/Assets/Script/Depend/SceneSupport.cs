using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum SceneFlag {
	SCENE_LOGIN	= 1,
}

/// <summary>
/// Game engine.
/// </summary>
public class SceneSupport : ScriptableSingleton<SceneSupport>
{
	protected IResourceManager	m_ResourceManager;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="SceneSupport"/> class.
	/// </summary>
	protected SceneSupport()
	{
		m_ResourceManager = GameEngine.GetSingleton ().QueryPlugin<IResourceManager> ();
		if (!m_ResourceManager)
			throw new System.NullReferenceException ("Can't find resource manager");
	}

	/// <summary>
	/// Loads the scene.
	/// </summary>
	/// <returns><c>true</c>, if scene was loaded, <c>false</c> otherwise.</returns>
	/// <param name="szName">Size name.</param>
	public bool		LoadScene(string szName)
	{
		try{
			m_ResourceManager.LoadFromFile(szName.ToLower(), delegate(string szUrl, AssetBundle abFile) {
#if OPEN_DEBUG_LOG
				string[] aryAssetName = abFile.GetAllAssetNames();
				foreach(string asset in aryAssetName)
				{
					Debug.Log("Scene asset name : " + asset);
				}
#endif
				// load current scene
				Application.LoadLevel(szName);
			
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
	public bool		LoadScene(int nSceneID)
	{
		SqlScene sqlScene = GameSqlLite.GetSingleton().Query<SqlScene>(nSceneID);
		if (!sqlScene)
			throw new System.NullReferenceException("Can't find scene id " + nSceneID);

		if (sqlScene.Stream)
		{
			if (!string.IsNullOrEmpty(sqlScene.Url))
			{
				LoadScene(sqlScene.Url);
			}
		}
		else
		{
			Application.LoadLevel(sqlScene.Url);
		}

		return true;
	}
}

