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
	RLF_SYNCHRONOUS		= 1 << 2,
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
	/// Gets the file path.
	/// </summary>
	/// <value>The file path.</value>
	public string		FilePath {
		get{
			return Application.persistentDataPath;
		}
	} 
	
	/// <summary>
	/// Install this instance.
	/// </summary>
	public override void Install()
	{
		try{
			string szManifestPath = string.Format("{0}/{1}/{2}", 
			                                      FilePath, typeof(AssetBundle).Name, typeof(AssetBundle).Name);
			
#if OPEN_DEBUG_LOG
			Debug.Log("Load resource manifest " + szManifestPath);
#endif
			using (FileStream stream = File.OpenRead(szManifestPath))
			{
				if (stream.CanRead)
				{
					byte[] binary = new byte[stream.Length];

					int nReadLength = stream.Read(binary, 0, binary.Length);
					if (nReadLength == binary.Length)
					{
						Manifest = new ResourceManifest(
							AssetBundle.CreateFromMemoryImmediate(binary)
							);
					}
				}

				stream.Close();
			}
	
#if OPEN_DEBUG_LOG
			Manifest.Dump();
#endif
		}
		catch(System.Exception e)
		{
			Debug.LogException(e);
		}
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
	/// Gets the file path.
	/// </summary>
	/// <returns>The file path.</returns>
	/// <param name="szAssetName">Size asset name.</param>
	public string GetFilePath(string szAssetName)
	{
		return string.Format("file://{0}/{1}/{2}",
		                             FilePath, typeof(AssetBundle).Name, szAssetName);
	}

	/// <summary>
	/// Loads the resource.
	/// </summary>
	/// <param name="szAssetName">Size asset name.</param>
	public virtual void LoadFromFile(string szAssetName, ResourceLoadFlag flag, AssetbundleFileCallback callback)
	{
		string szUrl = GetFilePath(szAssetName);

		if (!Exists(szUrl))
		{
			switch(flag)
			{
			case ResourceLoadFlag.RLF_UNITY:
				StartCoroutine(OnUnityDownload(szAssetName, callback));
				break;
			}
		}
	}

	/// <summary>
	/// Exists the specified szAssetName.
	/// </summary>
	/// <param name="szAssetName">Size asset name.</param>
	public bool			Exists(string szAssetName)
	{
		return RefResource.ContainsKey(szAssetName);
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

		if (callback != null)
			callback(szAssetURL, ws.assetBundle);
	}
}

