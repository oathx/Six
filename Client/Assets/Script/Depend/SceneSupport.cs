using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using org.critterai.nav;
using org.critterai.nav.u3d;
using System.IO;

public class SceneFlag {
	public const int SCENE_LOGIN		= 1;
	public const int SCENE_CHARACTER 	= 2;
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
	protected NavGroup			m_Group;

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
	/// Gets the scene I.
	/// </summary>
	/// <returns>The scene I.</returns>
	public int 				GetSceneID()
	{
		return SceneID;
	}

	/// <summary>
	/// Gets the nav group.
	/// </summary>
	/// <returns>The nav group.</returns>
	public NavGroup			GetNavGroup()
	{
		return m_Group;
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
	protected void 			Update()
	{
		if (Async != default(AsyncOperation) && !Async.isDone)
		{
			SendEvent(SceneID, CmdEvent.CMD_SCENE_LOADING, Async.progress);
		}
	}

	/// <summary>
	/// Loads the nav mesh.
	/// </summary>
	/// <returns><c>true</c>, if nav mesh was loaded, <c>false</c> otherwise.</returns>
	public virtual bool		LoadNavMesh(string szFileName)
	{
		m_ResourceManager.LoadAssetBundleFromStream(szFileName, 
		                                            delegate(string szAssetName, AssetBundleResource abResource) {

			TextAsset asset = abResource.GetAsset<TextAsset>(szAssetName);
			if (asset)
			{
				m_Group = LoadNavMesh(asset.bytes);
			}

			return true;
		});

		return true;
	}
	
	/// <summary>
	/// Loads the nav mesh.
	/// </summary>
	/// <returns><c>true</c>, if nav mesh was loaded, <c>false</c> otherwise.</returns>
	/// <param name="byMemory">By memory.</param>
	public virtual NavGroup		LoadNavMesh(byte[] byMemory)
	{
		MemoryStream stream = new MemoryStream ();
		stream.Write (
			byMemory, 0, byMemory.Length
			);
		
		// reset file position
		stream.Position = 0;
		
		// deserialize name mesh data
		BinaryFormatter formatter = new BinaryFormatter();
		System.Object bytes = formatter.Deserialize(stream);
		
		NavStatus status = NavStatus.Failure;
		
		// create navigate mesh
		Navmesh navmesh;
		status = Navmesh.Create((byte[]) bytes, out navmesh);
		if (!NavUtil.Succeeded (status))
			throw new System.NullReferenceException ();
		
		int nMaxTitle = navmesh.GetMaxTiles ();
#if UNITY_EDITOR
		Debug.Log(" navmesh title count " + nMaxTitle);
#endif
		NavmeshQuery query;
		status = NavmeshQuery.Create(navmesh, 1024, out query);
		if (!NavUtil.Succeeded (status))
			throw new System.NullReferenceException ();
		
		CrowdManager crowd = CrowdManager.Create(20, 0.5f, navmesh);
		return new NavGroup(navmesh,
		                    query, 
		                    crowd, 
		                    crowd.QueryFilter, 
		                    Vector3.one * 3, 
		                    false);
	}
	
	/// <summary>
	/// Gets the random position.
	/// </summary>
	/// <returns>The random position.</returns>
	/// <param name="vStart">V start.</param>
	/// <param name="fRadius">F radius.</param>
	public virtual bool	GetRandomPosition(Vector3 vStart, float fRadius, ref Vector3 vOut)
	{
		// set query filter
		NavmeshQueryFilter filter = new NavmeshQueryFilter();
		filter.IncludeFlags = 0xffff;
		filter.ExcludeFlags = 0;
		
		NavStatus status = NavStatus.Failure;
		NavmeshPoint startPoint;
		
		// query random start point
		status = m_Group.query.GetNearestPoint (vStart, Vector3.one * 2, filter, out startPoint);
		if (!NavUtil.Succeeded (status))
			return false;
		
		// get a random point
		NavmeshPoint randPoint;
		status = m_Group.query.GetRandomPoint (startPoint, fRadius, filter, out randPoint);
		if (!NavUtil.Succeeded (status))
			return false;
		
		Vector3 vDirection = randPoint.point - vStart;
		vDirection.Normalize ();
		
		Vector3 vTarget = vStart + vDirection * fRadius;
		if (!PtInNavmesh (vTarget))
			vTarget = GetRaycastHit (vStart, vTarget);
		
		vOut = vTarget;
		
		return true;
	}
	
	/// <summary>
	/// Gets the random position.
	/// </summary>
	/// <returns>The random position.</returns>
	/// <param name="vStart">V start.</param>
	/// <param name="fRadius">F radius.</param>
	public virtual Vector3 GetRandomPosition(Vector3 vStart, float fRadius)
	{
		Vector3 vResult = Vector3.zero;
		
		// get a random point
		bool bResult = GetRandomPosition (vStart, fRadius, ref vResult);
		if (!bResult)
			vResult = vStart;
		
		return vResult;
	}
	
	/// <summary>
	/// Points the in navmesh.
	/// </summary>
	/// <returns>The in navmesh.</returns>
	/// <param name="vStart">V start.</param>
	public virtual bool	PtInNavmesh(Vector3 vStart)
	{
		NavmeshQueryFilter filter = new NavmeshQueryFilter();
		filter.IncludeFlags = 0xffff;
		filter.ExcludeFlags = 0;
		
		NavStatus 		status = NavStatus.Failure;
		NavmeshPoint 	startPoint;
		
		// Finds the nearest point on the surface of the navigation mesh.
		return NavUtil.Succeeded (
			m_Group.query.GetNearestPoint (vStart, Vector3.one * 2, filter, out startPoint)
			);
	}
	
	/// <summary>
	/// Gets the raycast hit.
	/// </summary>
	/// <returns>The raycast hit.</returns>
	/// <param name="vStart">V start.</param>
	/// <param name="vEnd">V end.</param>
	public virtual Vector3	GetRaycastHit(Vector3 vStart, Vector3 vEnd)
	{
		Vector3 vHit = Vector3.zero;
		
		NavmeshQueryFilter filter = new NavmeshQueryFilter();
		filter.IncludeFlags = 0xffff;
		filter.ExcludeFlags = 0;
		
		NavStatus 		status = NavStatus.Failure;
		NavmeshPoint 	startPoint;
		
		// Finds the nearest point on the surface of the navigation mesh.
		status = m_Group.query.GetNearestPoint (vStart, Vector3.one * 2, filter, out startPoint);
		if (!NavUtil.Succeeded (status))
			return vHit;
		
		NavmeshPoint endPoint;
		status = m_Group.query.GetNearestPoint (vEnd, Vector3.one * 2, filter, out endPoint);
		if (!NavUtil.Succeeded (status))
			return vHit;
		
		uint[] aryBuffer 	= new uint[32];
		float fHit 			= 0.0f;
		int nResult 		= 0;
		status = m_Group.query.Raycast(startPoint, endPoint.point, filter, 
		                               out fHit, out vHit, aryBuffer, out nResult);
		
		return vHit;
	}
	
	
	/// <summary>
	/// Finds the path.
	/// </summary>
	/// <returns><c>true</c>, if path was found, <c>false</c> otherwise.</returns>
	/// <param name="vStart">V start.</param>
	/// <param name="vEnd">V end.</param>
	/// <param name="nFlag">N flag.</param>
	/// <param name="vResult">V result.</param>
	public virtual bool	FindPath (Vector3 vStart, Vector3 vEnd, int nFlag, ref List<Vector3> vResult)
	{
		NavmeshQueryFilter filter = new NavmeshQueryFilter();
		filter.IncludeFlags = 0xffff;
		filter.ExcludeFlags = 0;
		
		NavStatus 		status = NavStatus.Failure;
		NavmeshPoint 	startPoint;
		
		// Finds the nearest point on the surface of the navigation mesh.
		status = m_Group.query.GetNearestPoint (vStart, Vector3.one * 20, filter, out startPoint);
		if (!NavUtil.Succeeded (status))
			return false;
		
		NavmeshPoint endPoint;
		status = m_Group.query.GetNearestPoint (vEnd, Vector3.one * 20, filter, out endPoint);
		if (!NavUtil.Succeeded (status))
			return false;
		
		uint[] 	aryResultPath = new uint[64];
		int 	nPathCount;
		status = m_Group.query.FindPath(startPoint, endPoint, filter, 
		                                aryResultPath, out nPathCount);
		if (!NavUtil.Succeeded (status))
			return false;
		
		int nStartPoly = -1;
		for (int i=0; i < nPathCount; i++)
		{
			if (startPoint.polyRef == aryResultPath[i])
			{
				nStartPoly = i;
				break;
			}
		}
		
		Vector3[] 	buffer = new Vector3[16];
		int 		nResultCount = 0;
		
		int nTarget = Mathf.Max(0, nStartPoly);
		status = m_Group.query.GetStraightPath(startPoint.point, endPoint.point, aryResultPath, nTarget, nPathCount - nTarget,
		                                       buffer, null, null, out nResultCount);
		if (!NavUtil.Succeeded (status))
			return false;
		
		for (int i=0; i<nResultCount; i++)
			vResult.Add (buffer [i]);
		
		return true;
	}

}

