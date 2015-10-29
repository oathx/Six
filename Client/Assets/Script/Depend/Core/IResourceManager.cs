using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityThreading;
using System.IO;

/// <summary>
/// I resource.
/// </summary>
public class IResource<T> where T : class, new()
{
	/// <summary>
	/// Gets or sets the handle.
	/// </summary>
	/// <value>The handle.</value>
	public T					Handle
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the reference count.
	/// </summary>
	/// <value>The reference count.</value>
	public int 					RefCount
	{ get; set; }
	
	/// <summary>
	/// Initializes a new instance of the <see cref="IResource`1"/> class.
	/// </summary>
	/// <param name="handle">Handle.</param>
	public IResource(T handle)
	{
		RefCount = 1; Handle = handle;
	}
	
	/// <summary>
	/// Grab this instance.
	/// </summary>
	public virtual int 			Grab()
	{
		return ++RefCount;
	}
	
	/// <summary>
	/// Gets the reference count.
	/// </summary>
	/// <returns>The reference count.</returns>
	public int 					GetRefCount()
	{
		return RefCount;
	}
	
	/// <summary>
	/// Drop this instance.
	/// </summary>
	public virtual int 			Drop()
	{
		return --RefCount;
	}
	
	/// <summary>
	/// Dump this instance.
	/// </summary>
	public virtual void			Dump()
	{
		Debug.Log("Resourct Type " + typeof(T).Name + " ref count " + RefCount);
	}
}

/// <summary>
/// I asset bundle resource.
/// </summary>
public class AssetBundleResource : IResource<AssetBundle>
{
	/// <summary>
	/// Gets the depend.
	/// </summary>
	/// <value>The depend.</value>
	public string[]			Depend
	{ get; private set; }
	
	/// <summary>
	/// Initializes a new instance of the <see cref="IAssetBundleResource"/> class.
	/// </summary>
	/// <param name="handle">Handle.</param>
	public AssetBundleResource(AssetBundle handle, string[] aryDepend)
		: base(handle)
	{
		Depend = aryDepend;
	}
	
	/// <summary>
	/// Drop this instance.
	/// </summary>
	public override int 	Drop()
	{
		if ( --RefCount <= 0 )
		{
			Handle.Unload(false);
		}
		
		return RefCount;
	}
	
	/// <summary>
	/// Dump this instance.
	/// </summary>
	public override void 	Dump()
	{
		base.Dump();
		
		string[] aryAssetName = Handle.GetAllAssetNames();
		foreach(string asset in aryAssetName)
		{
			Debug.Log(" >> Aasset bundle name " + Handle.name + " asset : " + asset);
		}
	}
	
	/// <summary>
	/// Gets all asset names.
	/// </summary>
	/// <returns>The all asset names.</returns>
	public string[]			GetAllAssetNames()
	{
		return Handle.GetAllAssetNames();
	}

	/// <summary>
	/// Gets the asset.
	/// </summary>
	/// <returns>The asset.</returns>
	/// <param name="szAssetName">Size asset name.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public T				GetAsset<T>(string szAssetName) where T : UnityEngine.Object
	{
		if (!Handle.Contains(szAssetName))
			throw new System.NullReferenceException("Can't find asset " + szAssetName);

		return Handle.LoadAsset<T>(szAssetName);
	}
}

/// <summary>
/// I resource package.
/// </summary>
public class ResourceManifest : AssetBundleResource
{
	/// <summary>
	/// Gets or sets the manifest.
	/// </summary>
	/// <value>The manifest.</value>
	public AssetBundleManifest Manifest
	{ get; set; }
	
	/// <summary>
	/// Initializes a new instance of the <see cref="IResourcePackage"/> class.
	/// </summary>
	/// <param name="handle">Handle.</param>
	/// <param name="manifest">Manifest.</param>
	public ResourceManifest(AssetBundle handle)
		: base(handle, new string[]{})
	{
		Manifest = handle.LoadAsset<AssetBundleManifest>(typeof(AssetBundleManifest).Name);
		if (!Manifest)
			throw new System.NullReferenceException();
	}
	
	/// <summary>
	/// Dump this instance.
	/// </summary>
	public override	void 	Dump()
	{
		base.Dump();
		
		string[] aryAssetBundle = Manifest.GetAllAssetBundles();
		foreach(string s in aryAssetBundle)
		{
			Debug.Log(typeof(ResourceManifest).Name + " >> " + s);
		}
	}
	
	/// <summary>
	/// Gets all dependencies.
	/// </summary>
	/// <returns>The all dependencies.</returns>
	/// <param name="szAssetName">Size asset name.</param>
	public string[]			GetAllDependencies(string szAssetName)
	{
		return Manifest.GetAllDependencies(szAssetName);
	}
	
	/// <summary>
	/// Gets the asset bundle hash.
	/// </summary>
	/// <returns>The asset bundle hash.</returns>
	/// <param name="">.</param>
	public Hash128 			GetAssetBundleHash(string szAssetName)
	{
		return Manifest.GetAssetBundleHash(szAssetName);
	}
}


/// <summary>
/// I asset bundle task.
/// </summary>
public abstract class IAssetBundleTask : IEnumerator
{
	public object 			Current
	{
		get{
			return default(object);
		}
	}
	
	/// <summary>
	/// Moves the next.
	/// </summary>
	/// <returns><c>true</c>, if next was moved, <c>false</c> otherwise.</returns>
	public bool 			MoveNext()
	{
		return !IsDone();
	}
	
	/// <summary>
	/// Reset this instance.
	/// </summary>
	public void 			Reset()
	{
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	abstract public bool 	Update ();
	
	/// <summary>
	/// Determines whether this instance is done.
	/// </summary>
	/// <returns><c>true</c> if this instance is done; otherwise, <c>false</c>.</returns>
	abstract public bool 	IsDone ();
}

public enum ResourceLoadFlag
{
	RLF_UNITY			= 1 << 1,
	RLF_MEMORY			= 1 << 2,
	RLF_THREAD			= 1 << 3,
}

public delegate bool		AssetbundleFileCallback(string szAssetName, AssetBundleResource abResource);

/// <summary>
/// resource manager.
/// </summary>
public class IResourceManager : IGamePlugin
{
	/// <summary>
	/// Gets the resource manifest.
	/// </summary>
	/// <value>The resource manifest.</value>
	public ResourceManifest	Manifest
	{ get; private set ;}
	
	/// <summary>
	/// The reference resource.
	/// </summary>
	public Dictionary<string, 
	AssetBundleResource> RefResource = new Dictionary<string, AssetBundleResource>();
	
	public class Work
	{
		/// <summary>
		/// Gets the name of the asset.
		/// </summary>
		/// <value>The name of the asset.</value>
		public string							AssetName
		{ get; private set; }
		
		/// <summary>
		/// The callback.
		/// </summary>
		public List<AssetbundleFileCallback>	
			Callback = new List<AssetbundleFileCallback>();
		
		/// <summary>
		/// Gets the download.
		/// </summary>
		/// <value>The download.</value>
		public WWW								Download
		{ get; private set; }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="IResourceManager+Work"/> class.
		/// </summary>
		/// <param name="szAssetName">Size asset name.</param>
		public Work(string szAssetName, WWW download)
		{
			AssetName = szAssetName; Download = download;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="IResourceManager+Work"/> class.
		/// </summary>
		/// <param name="szAssetName">Size asset name.</param>
		public Work(string szAssetName, AssetbundleFileCallback callback)
		{
			AssetName = szAssetName; Callback.Add(callback);
		}
		
		/// <summary>
		/// Dos the interal.
		/// </summary>
		/// <param name="abResource">Ab resource.</param>
		public void	DoInteral(AssetBundleResource abResource)
		{
			foreach(AssetbundleFileCallback call in Callback)
			{
				call(AssetName, abResource);
			}
		}
	}
	
	/// <summary>
	/// The work task.
	/// </summary>
	public Dictionary<string, Work> 
		WorkTask = new Dictionary<string, Work>();
	
	/// <summary>
	/// Install this instance.
	/// </summary>
	public override void Install()
	{
		
	}
	
	/// <summary>
	/// Unstall this instance.
	/// </summary>
	public override void Uninstall()
	{
		
	}
	
	/// <summary>
	/// Startup this instance.
	/// </summary>
	public override void Startup()
	{
		
	}
	
	/// <summary>
	/// Shutdown this instance.
	/// </summary>
	public override void Shutdown()
	{
		
	}
	
	/// <summary>
	/// Registers the asset bundle package.
	/// </summary>
	/// <param name="szPackagePath">Size package path.</param>
	/// <param name="callback">Callback.</param>
	public void 	RegisterAssetBundlePackage(string szPackagePath, 
	                                        AssetbundleFileCallback callback)
	{
		StartCoroutine(
			OnLoadPackageIndexFromMemory(szPackagePath, callback)
			);
	}
	
	/// <summary>
	/// URLs the type.
	/// </summary>
	/// <returns><c>true</c>, if type was URLed, <c>false</c> otherwise.</returns>
	/// <param name="szUrl">Size URL.</param>
	public bool		UrlType(string szUrl)
	{
		if (szUrl[0] == 102 && szUrl[1] == 105 && szUrl[2] == 108 && szUrl[3] == 101)
			return true;
		
		return false;
	}
	
	/// <summary>
	/// Raises the load package index file from memory event.
	/// </summary>
	/// <param name="szPath">Size path.</param>
	/// <param name="callback">Callback.</param>
	IEnumerator		OnLoadPackageIndexFromMemory(string szUrl, 
	                                          AssetbundleFileCallback callback)
	{
		byte[] byIndexFile = File.ReadAllBytes(szUrl);
		if (byIndexFile.Length != 0)
		{
			#if OPEN_DEBUG_LOG
			Debug.Log("register assetbundle resource manifest " + szUrl);
			#endif
		}
		
		AssetBundleCreateRequest req = AssetBundle.CreateFromMemory(byIndexFile);
		yield return req;
		
		Manifest = new ResourceManifest(
			req.assetBundle
			);
		
		#if OPEN_DEBUG_LOG
		Manifest.Dump();
		#endif
		
		callback(szUrl, Manifest);
	}
	
	
	/// <summary>
	/// Gets the file path.
	/// </summary>
	/// <returns>The file path.</returns>
	/// <param name="szAssetName">Size asset name.</param>
	public string 	GetAssetPath(string szAssetName)
	{
		return string.Format("{0}/{1}/{2}",
		                     WUrl.DataURL, typeof(AssetBundle).Name, szAssetName);
	}
	
	/// <summary>
	/// Gets the asset depend.
	/// </summary>
	/// <returns>The asset depend.</returns>
	/// <param name="szAssetName">Size asset name.</param>
	public string[]	GetAssetDepend(string szAssetName)
	{
		return Manifest.GetAllDependencies(szAssetName);
	}
	
	/// <summary>
	/// Exists the specified szAssetName.
	/// </summary>
	/// <param name="szAssetName">Size asset name.</param>
	public bool		Exists(string szAssetName)
	{
		return RefResource.ContainsKey(szAssetName);
	}
	
	/// <summary>
	/// Unloads the asset.
	/// </summary>
	/// <param name="szName">Size name.</param>
	public void 	UnloadAssetBundle(string szAssetName)
	{
		if (RefResource.ContainsKey(szAssetName))
		{
			AssetBundleResource r = RefResource[szAssetName];
			foreach(string depend in r.Depend)
			{
				if (RefResource.ContainsKey(depend))
					RefResource[depend].Drop();
			}
			
			#if OPEN_DEBUG_LOG
			Debug.Log("Unload assetbundle name " + szAssetName);
			#endif
			int nRefCount = RefResource[szAssetName].Drop();
			if (nRefCount <= 0)
				RefResource.Remove(szAssetName);
		}
	}
	
	/// <summary>
	/// Dos the internal.
	/// </summary>
	/// <returns><c>true</c>, if internal was done, <c>false</c> otherwise.</returns>
	/// <param name="szAssetName">Size asset name.</param>
	/// <param name="bManifest">If set to <c>true</c> b manifest.</param>
	public bool		LoadAssetBundleFromStream(string szAssetName, AssetbundleFileCallback callback)
	{
		// To find out if this resource already exists
		if (RefResource.ContainsKey(szAssetName))
		{
			// Add resource reference count
			RefResource[szAssetName].Grab(); 
			
			// execute resource progress
			return callback(
				szAssetName, RefResource[szAssetName]
				);
		}
		else
		{
			if (!WorkTask.ContainsKey(szAssetName))
			{
				WorkTask.Add(szAssetName, new Work(szAssetName, callback));
				
				// add download task
				StartCoroutine(
					OnLoadAssetFromStream(szAssetName)
					);
			}
			else
			{
				WorkTask[szAssetName].Callback.Add(callback);
			}
		}
		
		return true;
	}
	
	/// <summary>
	/// Raises the load asset from stream event.
	/// </summary>
	/// <param name="szAssetName">Size asset name.</param>
	IEnumerator			OnLoadAssetFromStream(string szAssetName)
	{
		Dictionary<string, bool> 
			aryDependTask = new Dictionary<string, bool>();
		
		string[] aryDepend = Manifest.GetAllDependencies(szAssetName);
		
		foreach(string depend in aryDepend)
		{
			if (!RefResource.ContainsKey(depend) && !aryDependTask.ContainsKey(depend))
			{
				byte[] byFile = File.ReadAllBytes(GetAssetPath(depend));
				if (byFile.Length > 0)
				{
					aryDependTask.Add(depend, true);
					
					// create the assetbundle form memory
					AssetBundleCreateRequest req = AssetBundle.CreateFromMemory(byFile);
					yield return req;
					
					// remove load flag
					aryDependTask.Remove(depend);
					
#if OPEN_DEBUG_LOG
					Debug.Log("Load resource depend : " + depend);
#endif
					// save assetbundle ref
					RefResource.Add(
						depend, new AssetBundleResource(req.assetBundle, new string[]{})
						);
				}
			}
			else
			{
				RefResource[depend].Grab();
			}
		}
		
		// load assetbundle file
		string szAssetURL = GetAssetPath(szAssetName);
		
		// create the assetbundle form memory
		AssetBundleCreateRequest ws = AssetBundle.CreateFromMemory(
			File.ReadAllBytes(szAssetURL));
		
		yield return ws;
		
#if OPEN_DEBUG_LOG
		Debug.Log("Load assetbundle url : " + szAssetURL);
#endif

		RefResource.Add(
			szAssetName, new AssetBundleResource(ws.assetBundle, aryDepend)
			);
		
		if (WorkTask.ContainsKey(szAssetName))
		{
			// Execute all resource callback functions
			WorkTask[szAssetName].DoInteral(
				RefResource[szAssetName]
				);
			
			// remove the work task
			WorkTask.Remove(szAssetName);
		}
	}
}

