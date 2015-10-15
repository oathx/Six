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
public class AssetBundleRefResource : IResource<AssetBundle>
{
	/// <summary>
	/// Initializes a new instance of the <see cref="IAssetBundleResource"/> class.
	/// </summary>
	/// <param name="handle">Handle.</param>
	public AssetBundleRefResource(AssetBundle handle)
		: base(handle)
	{

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
}

/// <summary>
/// I resource package.
/// </summary>
public class ResourceManifest : AssetBundleRefResource
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
		: base(handle)
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

public enum ResourceLoadFlag
{
	RLF_UNITY			= 1 << 1,
	RLF_MEMORY			= 1 << 2,
	RLF_THREAD			= 1 << 3,
}

public delegate bool		AssetbundleFileCallback(string szUrl, AssetBundle abFile);

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
	public Dictionary<string, AssetBundleRefResource> RefResource = new Dictionary<string, AssetBundleRefResource>();

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
		if (UrlType(szPackagePath))
		{
			StartCoroutine(OnLoadPackageIndexFile(szPackagePath, callback));
		}
		else
		{
			StartCoroutine(
				OnLoadPackageIndexFileFromMemory(szPackagePath, callback)
				);
		}
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
	IEnumerator		OnLoadPackageIndexFileFromMemory(string szUrl, 
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
		
		callback(szUrl, req.assetBundle);
	}

	/// <summary>
	/// Raises the load package index file event.
	/// </summary>
	/// <param name="szPackagePath">Size package path.</param>
	IEnumerator		OnLoadPackageIndexFile(string szPackagePath, AssetbundleFileCallback callback)
	{
#if OPEN_DEBUG_LOG
		Debug.Log("register assetbundle resource manifest " + szPackagePath);
#endif

		WWW wPackage = WWW.LoadFromCacheOrDownload(szPackagePath, 0);
		yield return wPackage;

		Manifest = new ResourceManifest(
			wPackage.assetBundle
			);
		
#if OPEN_DEBUG_LOG
		Manifest.Dump();
#endif
		
		callback(szPackagePath, wPackage.assetBundle);
	}

	/// <summary>
	/// Loads from file.
	/// </summary>
	/// <param name="szAssetName">Size asset name.</param>
	/// <param name="flag">Flag.</param>
	/// <param name="callback">Callback.</param>
	public virtual void LoadFromFile(string szAssetName, ResourceLoadFlag flag, AssetbundleFileCallback callback)
	{
		string szUrl = flag == ResourceLoadFlag.RLF_UNITY ? GetFilePath(szAssetName) : GetLocalPath(szAssetName);

		switch(flag)
		{
		case ResourceLoadFlag.RLF_UNITY:
			if (!Exists(szUrl))
			{
				StartCoroutine(
					OnUnityDownload(szAssetName, callback)
					);
			}
			else
			{
				RefResource[szUrl].Grab();
				
				callback(szUrl, 
				         RefResource[szUrl].Handle
				         );
				
				RefResource[szUrl].Drop();
			}
			break;
		
		case ResourceLoadFlag.RLF_MEMORY:
			if (!Exists(szUrl))
			{
				StartCoroutine(
					OnMemoryDownload(szAssetName, callback)
					);
			}
			else
			{
				RefResource[szUrl].Grab();
				
				callback(szUrl, 
				         RefResource[szUrl].Handle
				         );
				
				RefResource[szUrl].Drop();
			}
			break;
		}

	}

	/// <summary>
	/// Raises the memory download event.
	/// </summary>
	/// <param name="szAssetName">Size asset name.</param>
	/// <param name="callback">Callback.</param>
	IEnumerator			OnMemoryDownload(string szAssetName, AssetbundleFileCallback callback)
	{
		string[] aryDepend = Manifest.GetAllDependencies(szAssetName);
		foreach(string depend in aryDepend)
		{
			string szDependURL = GetFilePath(depend);

			if (!RefResource.ContainsKey(szDependURL))
			{
				byte[] byFile = File.ReadAllBytes(szDependURL);

				AssetBundleCreateRequest req = AssetBundle.CreateFromMemory(byFile);
				yield return req;
				
#if OPEN_DEBUG_LOG
				Debug.Log("Load resource depend url : " + szDependURL);
#endif
				RefResource.Add(
					szDependURL, new AssetBundleRefResource(req.assetBundle)
					);
			}
		}

		string szAssetURL = GetFilePath(szAssetName);

		// create the assetbundle form memory
		AssetBundleCreateRequest ws = AssetBundle.CreateFromMemory(
			File.ReadAllBytes(szAssetURL));

		yield return ws;
		
#if OPEN_DEBUG_LOG
		Debug.Log("Load resource assetbundle url : " + szAssetURL);
#endif
		RefResource.Add(
			szAssetURL, new AssetBundleRefResource(ws.assetBundle)
			);
		
		// execute call back
		callback(szAssetURL, ws.assetBundle);
	}

	/// <summary>
	/// Raises the unity download event.
	/// </summary>
	/// <param name="szAssetName">Size asset name.</param>
	IEnumerator 		OnUnityDownload(string szAssetName, AssetbundleFileCallback callback)
	{
		string[] aryDepend	= Manifest.GetAllDependencies(szAssetName);
		foreach(string depend in aryDepend)
		{
			string szDependURL = GetFilePath(depend);
			
			if (!RefResource.ContainsKey(szDependURL))
			{
				WWW wDepend = WWW.LoadFromCacheOrDownload(szDependURL,
				                                          Manifest.GetAssetBundleHash(depend));
				yield return wDepend;
				
#if OPEN_DEBUG_LOG
				Debug.Log("Load resource depend url : " + szDependURL);
#endif
				RefResource.Add(
					szDependURL, new AssetBundleRefResource(wDepend.assetBundle)
					);
			}
		}
		
		string szAssetURL = GetFilePath(szAssetName);
		
		// load the assetbundle file
		WWW ws = WWW.LoadFromCacheOrDownload(szAssetURL,
		                                     Manifest.GetAssetBundleHash(szAssetName));
		yield return ws;
		
#if OPEN_DEBUG_LOG
		Debug.Log("Load resource assetbundle url : " + szAssetURL);
#endif
		RefResource.Add(
			szAssetURL, new AssetBundleRefResource(ws.assetBundle)
			);
		
		// execute call back
		callback(szAssetURL, ws.assetBundle);
	}


	/// <summary>
	/// Gets the file path.
	/// </summary>
	/// <returns>The file path.</returns>
	/// <param name="szAssetName">Size asset name.</param>
	public string GetFilePath(string szAssetName)
	{
		return string.Format("{0}/{1}/{2}",
		                             WUrl.Url, typeof(AssetBundle).Name, szAssetName);
	}

	/// <summary>
	/// Gets the local path.
	/// </summary>
	/// <returns>The local path.</returns>
	/// <param name="szAssetName">Size asset name.</param>
	public string GetLocalPath(string szAssetName)
	{
		return string.Format("{0}/{1}/{2}",
		                     WUrl.DataURL, typeof(AssetBundle).Name, szAssetName);
	}

	/// <summary>
	/// Query the specified szAssetName.
	/// </summary>
	/// <param name="szAssetName">Size asset name.</param>
	public AssetBundleRefResource	Query(string szAssetName)
	{
		string szUrl = GetFilePath(szAssetName);
		
		return RefResource[szUrl];
	}
	
	/// <summary>
	/// Exists the specified szAssetName.
	/// </summary>
	/// <param name="szAssetName">Size asset name.</param>
	public bool			Exists(string szAssetName)
	{
		return RefResource.ContainsKey(szAssetName);
	}
}

