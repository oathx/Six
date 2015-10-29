using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Timers;

public class Build {

	public enum SkinMeshType
	{
		mesh,
		bone,
	}

	/// <summary>
	/// Gets the root UR.
	/// </summary>
	/// <value>The root UR.</value>
	public static string	OutUrl
	{
		get
		{
#if UNITY_STANDALONE || UNITY_EDITOR
			return Application.streamingAssetsPath + "/Win";
#elif UNITY_ANDROID
			return Application.streamingAssetsPath + "Android";
#elif UNITY_IPHONE
			return Application.streamingAssetsPath + "/iOS";
#endif
		}
	}

	public class BuildDirectory
	{
		public string	directory
		{ get; set; }

		public string	pattern
		{ get; set; }

		public BuildDirectory(string szDirectory, string szPattern)
		{
			directory = szDirectory; pattern = szPattern;
		}
	}

	[MenuItem("Build/Build Character skeleton and equip")]
	static void 	BuildSplitModel()
	{
		BuildCharacter("Character", Application.dataPath + "/Art/Model", "*.prefab");
	}

	[MenuItem("Build/Build unity3d package")]
	static void 	BuildUnity3DPackage()
	{
		
	}
	
	[MenuItem("Build/Build single package")]
	static void 	BuildSinglePackage()
	{
		BuildSinglePackage("Single", new BuildDirectory[]{
			new BuildDirectory(Application.dataPath + "/Design", "*.bytes"),
		});
	}

	[MenuItem("Build/Build all resource to zip")]
	static void 	BuildZipFileToStreamasset()
	{
		BuildCharacter("Character", Application.dataPath + "/Art/Model", "*.prefab");

		BuildAssetBundles(typeof(AssetBundle).Name, true, new BuildDirectory[]{

			new BuildDirectory(Application.dataPath + "/Art/Scene", 	"*.unity"),
			new BuildDirectory(Application.dataPath + "/Art/Character", "*.asset"),
			new BuildDirectory(Application.dataPath + "/Art/Character", "*.prefab"),
		});

		BuildSinglePackage ();
		BuildUnity3DPackage();
	}

	/// <summary>
	/// Builds the single.
	/// </summary>
	/// <param name="szPackageName">Size package name.</param>
	/// <param name="outZipFile">If set to <c>true</c> out zip file.</param>
	/// <param name="aryDirectory">Ary directory.</param>
	static void 	BuildUnity3D(string szPackageName, bool outZipFile, params BuildDirectory[] aryDirectory)
	{
		string szOutPath = GetTempPath(szPackageName);
		
		// create out directory
		if (!File.Exists(szOutPath))
			Directory.CreateDirectory(szOutPath);

		BuildAssetBundleOptions opt = BuildAssetBundleOptions.UncompressedAssetBundle;

		foreach(BuildDirectory directory in aryDirectory)
		{
			string[] aryFile = System.IO.Directory.GetFiles(directory.directory, directory.pattern, System.IO.SearchOption.AllDirectories);
			foreach(string path in aryFile)
			{
				if (!path.Contains(".meta"))
				{
					string szFilePath	= path.Replace("\\", "/");
					
					// get the assetbundl file path
					string szAssetPath 	= szFilePath.Substring(
						Application.dataPath.Length - 6, szFilePath.Length - Application.dataPath.Length + 6
						);

					string szOutFile	= string.Format("{0}/{1}.unity3d", szOutPath, GetName(szAssetPath));
#if UNITY_EDITOR_WIN
					BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath(szAssetPath), new Object[]{}, szOutFile, 
						opt, BuildTarget.StandaloneWindows);
#elif UNITY_ANDROID
					BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath(szAssetPath), new Object[]{}, szOutFile,
						opt, BuildTarget.Android);
#elif UNITY_IOS
					BuildPipeline.BuildAssetBundle(AssetDatabase.LoadMainAssetAtPath(szAssetPath), new Object[]{}, szOutFile, 
						opt, BuildTarget.iOS);
#endif
				}
			}
		}

		if (outZipFile)
		{
			if (!Directory.Exists(OutUrl))
				Directory.CreateDirectory(OutUrl);
			
			string zipOutPath = string.Format("{0}/{1}.zip", 
			                                  OutUrl, szPackageName);
			// create zip file
			CreateZipFile(szOutPath, zipOutPath);
		}
	}

	/// <summary>
	/// Builds the single package.
	/// </summary>
	/// <param name="szPackageName">Size package name.</param>
	/// <param name="outZipFile">If set to <c>true</c> out zip file.</param>
	/// <param name="aryDirectory">Ary directory.</param>
	static void 	BuildSinglePackage(string szPackageName, params BuildDirectory[] aryDirectory)
	{
		string szOutPath = GetTempPath(szPackageName);

		// create out directory
		if (!File.Exists(szOutPath))
			Directory.CreateDirectory(szOutPath);

		foreach(BuildDirectory directory in aryDirectory)
		{
			string[] aryFile = System.IO.Directory.GetFiles(directory.directory, directory.pattern, System.IO.SearchOption.AllDirectories);
			foreach(string path in aryFile)
			{
				if (!path.Contains(".meta"))
				{
					File.Copy(path, string.Format("{0}/{1}", szOutPath, GetFileName(path)), true);
				}
			}
		}

		if (!Directory.Exists(OutUrl))
			Directory.CreateDirectory(OutUrl);
		
		string zipOutPath = string.Format("{0}/{1}.zip", 
		                                  OutUrl, szPackageName);
		// create zip file
		CreateZipFile(szOutPath, zipOutPath);
	}


	/// <summary>
	/// Builds the zip file.
	/// </summary>
	/// <param name="aryPath">Ary path.</param>
	static void 	BuildAssetBundles(string szPackageName, bool outZipFile, params BuildDirectory[] aryDirectory)
	{
		foreach(BuildDirectory directory in aryDirectory)
		{
			string[] aryFile = System.IO.Directory.GetFiles(directory.directory, directory.pattern, System.IO.SearchOption.AllDirectories);
			foreach(string path in aryFile)
			{
				string szFilePath	= path.Replace("\\", "/");

				// get the assetbundl file path
				string szAssetPath 	= szFilePath.Substring(
					Application.dataPath.Length - 6, szFilePath.Length - Application.dataPath.Length + 6
					);

				string szFileName = GetFileName(szAssetPath);
				AssetImporter imp = AssetImporter.GetAtPath(szAssetPath);
				imp.assetBundleName = szFileName.Split('.')[0];

				string[] aryDepend = AssetDatabase.GetDependencies(new string[]{szAssetPath});
				foreach(string depend in aryDepend)
				{
					if (DependType(depend))
					{
						AssetImporter ai 	= AssetImporter.GetAtPath(depend);

						if (string.IsNullOrEmpty(ai.assetBundleName))
							ai.assetBundleName = AssetDatabase.AssetPathToGUID(depend);

						Debug.Log(" >> depend file : " + depend);
					}
				}
			}

			string szTargetDirectory = GetTempPath(szPackageName);

			// create out directory
			if (!File.Exists(szTargetDirectory))
				Directory.CreateDirectory(szTargetDirectory);

#if UNITY_EDITOR_WIN
			BuildPipeline.BuildAssetBundles(szTargetDirectory, 
			                                BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows);

#elif UNITY_ANDROID
			BuildPipeline.BuildAssetBundles(szTargetDirectory, 
			                                BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.Android);
#elif UNITY_IOS
			BuildPipeline.BuildAssetBundles(szTargetDirectory, 
			                                BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.iOS);
#endif

			if (outZipFile)
			{
				if (!Directory.Exists(OutUrl))
					Directory.CreateDirectory(OutUrl);

				string zipOutPath = string.Format("{0}/{1}.zip", 
				                                  OutUrl, szPackageName);
				// create zip file
				CreateZipFile(szTargetDirectory, zipOutPath);
			}
		}
	}

	/// <summary>
	/// Split the specified szInputDirectory and szOutDirectory.
	/// </summary>
	/// <param name="szInputDirectory">Size input directory.</param>
	/// <param name="szOutDirectory">Size out directory.</param>
	static void 	BuildCharacter(string szPackageName, string szInputDirectory, string pattern)
	{
		string szOutPath = string.Format("{0}/Art/{1}", 
		                                 Application.dataPath, szPackageName);
		
		// create out directory
		if (!File.Exists(szOutPath))
			Directory.CreateDirectory(szOutPath);

		string[] aryPath = System.IO.Directory.GetFiles(szInputDirectory, pattern, System.IO.SearchOption.AllDirectories);
		foreach(string path in aryPath)
		{
			string szFilePath	= path.Replace("\\", "/");
			
			// get the assetbundl file path
			string szAssetPath 	= szFilePath.Substring(
				Application.dataPath.Length - 6, szFilePath.Length - Application.dataPath.Length + 6
				);

			Object resource = AssetDatabase.LoadMainAssetAtPath(szAssetPath);
			if (resource)
			{
				string szTargetPath = string.Format("{0}/{1}/", szOutPath, GetName(szFilePath));
				if (!Directory.Exists(szTargetPath))
					Directory.CreateDirectory(szTargetPath);

				ExtractSkeleton(resource, szTargetPath, false);
				ExtractSkinMesh(resource, szTargetPath, false);
			}
		}

		if (!Directory.Exists(OutUrl))
			Directory.CreateDirectory(OutUrl);
		
		string zipOutPath = string.Format("{0}/{1}.zip", 
		                                  OutUrl, szPackageName);
		// create zip file
		CreateZipFile(szOutPath, zipOutPath);
	}

	/// <summary>
	/// Extracts the skeleton.
	/// </summary>
	/// <param name="resource">Resource.</param>
	/// <param name="szOutPath">Size out path.</param>
	static void 	ExtractSkeleton(Object resource, string szOutPath, bool bSingleFile)
	{
		GameObject fbx = GameObject.Instantiate(resource) as GameObject;
		if (fbx)
		{
			// destroy all mesh render object
			Transform[] aryTransform = fbx.GetComponentsInChildren<Transform>();
			foreach(Transform t in aryTransform)
			{
				MeshRenderer render = t.GetComponent<MeshRenderer>();
				if (render)
					GameObject.DestroyImmediate(t.gameObject);
			}

			// destroy all skinned mesh render
			foreach(SkinnedMeshRenderer mesh in fbx.GetComponentsInChildren<SkinnedMeshRenderer>())
			{
				GameObject.DestroyImmediate(mesh.gameObject);
			}

			SkinnedMeshRenderer skin = fbx.AddComponent<SkinnedMeshRenderer>();
			if (skin)
			{
				GameObject prefab = CreateEmptyPrefab(fbx, string.Format("{0}{1}.prefab", szOutPath, resource.name));
				if (!prefab)
					throw new System.NullReferenceException();

				if (bSingleFile)
				{
					string szTargetFullPath = string.Format("{0}{1}.skeleton",
					                                        szOutPath, resource.name);

					BuildAssetBundleOptions opt = BuildAssetBundleOptions.CollectDependencies | 
						BuildAssetBundleOptions.UncompressedAssetBundle;
#if UNITY_IOS
					BuildPipeline.BuildAssetBundle(prefab, new Object[]{}, szTargetFullPath,
						opt, BuildTarget.iPhone);
#elif UNITY_STANDALONE
					BuildPipeline.BuildAssetBundle(prefab, new Object[]{}, szTargetFullPath,
						opt, BuildTarget.StandaloneWindows);	
#elif UNITY_ANDROID
					BuildPipeline.BuildAssetBundle(prefab, new Object[]{}, szTargetFullPath,
						opt, BuildTarget.Android);						
#endif

					AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(prefab));
				}
			}

			GameObject.DestroyImmediate(fbx);
		}
	}

	/// <summary>
	/// Extracts the skin mesh.
	/// </summary>
	/// <param name="resource">Resource.</param>
	/// <param name="szOutPath">Size out path.</param>
	static void 		ExtractSkinMesh(Object resource, string szOutPath, bool bSingleFile)
	{
		GameObject fbx = GameObject.Instantiate(resource) as GameObject;
		if (fbx)
		{
			foreach(SkinnedMeshRenderer mesh in fbx.GetComponentsInChildren<SkinnedMeshRenderer>(true))
			{
				GameObject clones = GameObject.Instantiate(mesh.gameObject) as GameObject;
				if (!clones)
					throw new System.NullReferenceException();
					
				// create skinmesh prefab
				GameObject prefab = CreateEmptyPrefab(clones, string.Format("{0}{1}@{2}.prefab", 
					                                                            szOutPath, SkinMeshType.mesh.ToString(), mesh.name));
				if (!prefab)
					throw new System.NullReferenceException();

				// create skinmesh bone asset
				StringHolder holder = ScriptableObject.CreateInstance<StringHolder>();
				if (holder)
				{
					List<string> boneName = new List<string>();
					foreach(Transform t in mesh.bones)
						boneName.Add(t.name);
					
					holder.content 	= boneName.ToArray();
					
					string szHolderPath = string.Format("{0}{1}@{2}.asset", GetAssetPath(szOutPath), SkinMeshType.bone.ToString(), mesh.name);
					AssetDatabase.CreateAsset(holder, 
					                          szHolderPath);
				}

				GameObject.DestroyImmediate(clones);
			}

			GameObject.DestroyImmediate(fbx);
		}
	}

	/// <summary>
	/// Gets the asset path.
	/// </summary>
	/// <returns>The asset path.</returns>
	/// <param name="szPath">Size path.</param>
	static string		GetAssetPath(string szPath)
	{
		string szFilePath	= szPath.Replace("\\", "/");
		
		// get the assetbundl file path
		return szFilePath.Substring(
			Application.dataPath.Length - 6, szFilePath.Length - Application.dataPath.Length + 6
			);

	}

	/// <summary>
	/// Creates the empty prefab.
	/// </summary>
	/// <returns>The empty prefab.</returns>
	/// <param name="goItem">Go item.</param>
	/// <param name="szPath">Size path.</param>
	static GameObject	CreateEmptyPrefab(GameObject goItem, string szPath)
	{
		string szFilePath	= szPath.Replace("\\", "/");
		
		// get the assetbundl file path
		string szAssetPath 	= szFilePath.Substring(
			Application.dataPath.Length - 6, szFilePath.Length - Application.dataPath.Length + 6
			);

		Object prefab = PrefabUtility.CreateEmptyPrefab(szAssetPath);
		if (!prefab)
			throw new System.NullReferenceException();
		
		prefab = PrefabUtility.ReplacePrefab(goItem, prefab);
		GameObject.DestroyImmediate(goItem);

		return prefab as GameObject;
	}

	/// <summary>
	/// Creates the zip file.
	/// </summary>
	/// <param name="szOutDirectory">Size out directory.</param>
	static void 	CreateZipFile(string szInputDirectory, string szOutPath)
	{
		if (File.Exists(szOutPath))
			File.Delete(szOutPath);

		szInputDirectory 	= szInputDirectory.Replace('\\', '/');
		int iStart 			= szInputDirectory.LastIndexOf('/');
		
		FileStream zipFileStream = File.Open(szOutPath, FileMode.OpenOrCreate);
		if (zipFileStream.CanWrite)
		{
			using(ZipOutputStream stream = new ZipOutputStream(zipFileStream))
			{
				// 0 - store only to 9 - means best compression
				stream.SetLevel(9);

				string[] aryFilePath = Directory.GetFiles(szInputDirectory, "*.*", SearchOption.AllDirectories);
				foreach(string file in aryFilePath)
				{
					if (!Filter(file))
					{
						string path 			= file.Replace('\\', '/');
						string szZipEntryName 	= path.Substring(iStart + 1, path.Length - iStart - 1);

						ZipEntry entry = new ZipEntry(szZipEntryName);
						stream.PutNextEntry(entry);

						using(FileStream fs = File.OpenRead(file))
						{
							byte[] byData = new byte[fs.Length];
							
							int nReadLength = fs.Read(byData, 0, byData.Length);
							if (nReadLength == fs.Length)
							{
								stream.Write(byData, 0, byData.Length);
							}

							fs.Close();
						}

						Debug.Log("Add file to pkg " + szOutPath + " >> " + szZipEntryName);
					}
				}

				stream.Close();
			}

			zipFileStream.Close();
		}
	}

	/// <summary>
	/// Gets the name of the file.
	/// </summary>
	/// <returns>The file name.</returns>
	/// <param name="szPath">Size path.</param>
	static string	GetFileName(string szPath)
	{
		string szSplit = szPath.Replace('\\', '/');
		if (!string.IsNullOrEmpty(szSplit))
		{
			string[] arySplit = szSplit.Split('/');
			if (arySplit.Length > 0)
				return arySplit[arySplit.Length - 1];
		}

		return szSplit;
	}

	/// <summary>
	/// Filiter the specified szName.
	/// </summary>
	/// <param name="szName">Size name.</param>
	static bool		Filter(string path)
	{
		string[] aryFilter = {
			".meta", ".zip"
		};

		foreach(string filter in aryFilter)
		{
			if (path.Contains(filter))
				return true;
		}

		return false;
	}

	/// <summary>
	/// Gets the name.
	/// </summary>
	/// <returns>The name.</returns>
	/// <param name="szPath">Size path.</param>
	static string	GetName(string szPath)
	{
		int nStart 	= szPath.LastIndexOf('/');
		int nEnd	= szPath.LastIndexOf('.');

		return szPath.Substring(nStart + 1, nEnd - nStart - 1);
	}

	/// <summary>
	/// Gets the name of the directory.
	/// </summary>
	/// <returns>The directory name.</returns>
	/// <param name="szPath">Size path.</param>
	static string	GetDirectoryName(string szPath)
	{
		string szSplit = szPath.Replace('\\', '/');
		if (!string.IsNullOrEmpty(szSplit))
		{
			string[] arySplit = szSplit.Split('/');
			if (arySplit.Length > 0)
				return arySplit[arySplit.Length - 2];
		}
		
		return szSplit;
	}

	/// <summary>
	/// Depends the type.
	/// </summary>
	/// <returns><c>true</c>, if type was depended, <c>false</c> otherwise.</returns>
	/// <param name="szUrl">Size URL.</param>
	static bool		DependType(string szUrl)
	{
		string[] aryFileFormat = {
			".cs"
		};

		string szExt = string.Empty;
		int iPos = szUrl.LastIndexOf('.');
		if (iPos >= 0)
			szExt = szUrl.Substring(iPos, szUrl.Length - iPos);
		
		foreach(string ext in aryFileFormat)
		{
			if (ext == szExt)
				return false;
		}

		return true;
	}

	/// <summary>
	/// Gets the temp path.
	/// </summary>
	/// <returns>The temp path.</returns>
	/// <param name="szPackageName">Size package name.</param>
	static string	GetTempPath(string szPackageName)
	{
#if UNITY_EDITOR_WIN
		string szOutPath = string.Format("{0}/Temp/Win/{1}", 
		                                 Application.dataPath, szPackageName);
#elif UNITY_ANDROID
		string szOutPath = string.Format("{0}/Temp/Android/{1}", 
		                                 Application.dataPath, szPackageName);
#elif UNITY_IOS
		string szOutPath = string.Format("{0}/Temp/iOS/{1}", 
		                                 Application.dataPath, szPackageName);
#endif
		return szOutPath;
	}
}
