using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Timers;

public class CharacterEditor
{
	[MenuItem("Build/Split Character prefab")]
	public static void 	DoSplitCharacter()
	{
		BuildCharacter("Character", 
		               Application.dataPath + "/Art/Model/Character", "*.prefab");
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
		{
			Directory.CreateDirectory(szOutPath);
		}
		else
		{
			string[] aryFile = System.IO.Directory.GetFiles(szOutPath, "*.*", 
			                                                System.IO.SearchOption.AllDirectories);
			foreach(string file in aryFile)
			{
				File.Delete(file);
			}
		}

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
				string szTargetPath = string.Format("{0}/{1}/", szOutPath, Build.GetName(szFilePath));
				if (!Directory.Exists(szTargetPath))
					Directory.CreateDirectory(szTargetPath);
				
				ExtractSkeleton(resource, szTargetPath, false);
				ExtractSkinMesh(resource, szTargetPath, false);
			}
		}
		
		if (!Directory.Exists(Build.OutUrl))
			Directory.CreateDirectory(Build.OutUrl);
		
		string zipOutPath = string.Format("{0}/{1}.zip", 
		                                  Build.OutUrl, szPackageName);
		// create zip file
		Build.CreateZipFile(szOutPath, zipOutPath);
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
				GameObject prefab = Build.CreateEmptyPrefab(fbx, string.Format("{0}{1}.prefab", szOutPath, resource.name));
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
			Transform[] aryTransform = fbx.GetComponentsInChildren<Transform>();
			foreach(Transform t in aryTransform)
			{
				MeshRenderer render = t.GetComponent<MeshRenderer>();
				if (render)
				{
					GameObject clones = GameObject.Instantiate(render.gameObject) as GameObject;
					if (!clones)
						throw new System.NullReferenceException();
					
					// create skinmesh prefab
					GameObject prefab = Build.CreateEmptyPrefab(clones, string.Format("{0}{1}@{2}.prefab", 
					                                                                  szOutPath, Build.SkinMeshType.mesh.ToString(), render.name));
					if (!prefab)
						throw new System.NullReferenceException();
				}
			}
			
			foreach(SkinnedMeshRenderer mesh in fbx.GetComponentsInChildren<SkinnedMeshRenderer>(true))
			{
				GameObject clones = GameObject.Instantiate(mesh.gameObject) as GameObject;
				if (!clones)
					throw new System.NullReferenceException();
				
				// create skinmesh prefab
				GameObject prefab = Build.CreateEmptyPrefab(clones, string.Format("{0}{1}@{2}.prefab", 
				                                                                  szOutPath, Build.SkinMeshType.mesh.ToString(), mesh.name));
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
					
					string szHolderPath = string.Format("{0}{1}@{2}.asset", Build.GetAssetPath(szOutPath), Build.SkinMeshType.bone.ToString(), mesh.name);
					AssetDatabase.CreateAsset(holder, 
					                          szHolderPath);
				}
				
				GameObject.DestroyImmediate(clones);
			}
			
			GameObject.DestroyImmediate(fbx);
		}
	}
}