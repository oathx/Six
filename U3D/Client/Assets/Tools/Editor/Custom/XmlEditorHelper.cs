using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Timers;
using LitJson;

public class XmlEditorHelper
{
	/// <summary>
	/// Gets the out path.
	/// </summary>
	/// <value>The out path.</value>
	public static string	OutPath
	{
		get
		{
#if UNITY_STANDALONE && UNITY_EDITOR
			return Application.streamingAssetsPath + "/Win";
#elif UNITY_ANDROID
			return Application.streamingAssetsPath + "/Android";
#elif UNITY_IPHONE
			return Application.streamingAssetsPath + "/iOS";
#endif
		}
	}

	/// <summary>
	/// Gets the middle path.
	/// </summary>
	/// <value>The middle path.</value>
	public static string	MidPath
	{
		get{
#if UNITY_STANDALONE && UNITY_EDITOR
			return Application.streamingAssetsPath + "/Resource/Win";
#elif UNITY_ANDROID
			return Application.streamingAssetsPath + "/Resource/Android";
#elif UNITY_IPHONE
			return Application.streamingAssetsPath + "/Resource/iOS";
#endif
		}
	}

	/// <summary>
	/// Searchs the file.
	/// </summary>
	/// <returns>The file.</returns>
	/// <param name="type">Type.</param>
	/// <param name="szInputPath">Size input path.</param>
	public static List<string>
		SearchFile(SearchFileType type, string szInputPath)
	{
		List<string> 
			aryReturn = new List<string>();

		SearchFileType[] aryMask = new SearchFileType[]{
			SearchFileType.png,
			SearchFileType.prefab,
			SearchFileType.unity,
			SearchFileType.fbx,
			SearchFileType.mat,
			SearchFileType.jpg,
			SearchFileType.tag,
			SearchFileType.xml,
			SearchFileType.bytes,
			SearchFileType.pass,
			SearchFileType.unity3d,
			SearchFileType.zip,
			SearchFileType.cs,
			SearchFileType.dds,
			SearchFileType.DDS,
			SearchFileType.asset,
			SearchFileType.controller,
			SearchFileType.preview,
		};

		foreach(SearchFileType mask in aryMask)
		{
			if ((type & mask) != 0)
			{
				// Priority search prefab file
				string pattern = string.Format(
					"{0}.{1}", "*", mask.ToString().ToLower()
					);

				string[] aryFile = System.IO.Directory.GetFiles(szInputPath, pattern, SearchOption.AllDirectories);
				foreach(string path in aryFile)
				{
					string szPath = path.Substring(Application.dataPath.Length - 6);
					aryReturn.Add(
						szPath.Replace("\\", "/")
						);
				}
			}
		}

		return aryReturn;
	}

	/// <summary>
	/// Gets the length of the file.
	/// </summary>
	/// <returns>The file length.</returns>
	/// <param name="szFilePath">Size file path.</param>
	public static int 	GetFileLength(string szFilePath)
	{
		FileStream stream = File.OpenRead(szFilePath);
		if (stream.CanRead)
			return (int)stream.Length;

		return 0;
	}

	/// <summary>
	/// Builds the asset bundles.
	/// </summary>
	/// <param name="szPackageName">Size package name.</param>
	/// <param name="outZipFile">If set to <c>true</c> out zip file.</param>
	/// <param name="cleaup">If set to <c>true</c> cleaup.</param>
	/// <param name="szDirectory">Size directory.</param>
	/// <param name="szPattern">Size pattern.</param>
	public static bool 	BuildAssetBundles(string szInputPath, SearchFileType type)
	{
		if (string.IsNullOrEmpty(szInputPath))
			throw new System.NullReferenceException();

		// create mid path
		if (!Directory.Exists(MidPath))
			Directory.CreateDirectory(MidPath);

		// Search all files 
		List<string> aryFile = SearchFile(type, szInputPath);
		if (aryFile.Count <= 0)
			return false;

		// get directory name 
		string[] arySplit = szInputPath.Split('/');
		if (arySplit.Length <= 0)
			return false;

		AssetBundleBuild ab 	= new AssetBundleBuild();
		ab.assetNames			= aryFile.ToArray();
		ab.assetBundleVariant	= SearchFileType.unity3d.ToString();
		ab.assetBundleName		= arySplit[arySplit.Length - 1];
	
		// get the assetbundl file path
		string szOutPath = MidPath.Substring(
			Application.dataPath.Length - 6, MidPath.Length - Application.dataPath.Length + 6
			);

		BuildPipeline.BuildAssetBundles(szOutPath, new AssetBundleBuild[]{ab}, 
			BuildAssetBundleOptions.UncompressedAssetBundle, EditorUserBuildSettings.activeBuildTarget);

		return true;
	}

	/// <summary>
	/// Filter the specified path.
	/// </summary>
	/// <param name="path">Path.</param>
	public static bool	Filter(string path)
	{
		string[] aryFilter = {
			".meta", ".zip", ".manifest"
		};
		
		foreach(string filter in aryFilter)
		{
			if (path.Contains(filter))
				return true;
		}
		
		return false;
	}
	
	/// <summary>
	/// Creates the zip file.
	/// </summary>
	/// <returns><c>true</c>, if zip file was created, <c>false</c> otherwise.</returns>
	/// <param name="szInputPath">Size input path.</param>
	/// <param name="szOutPath">Size out path.</param>
	public static void	CreateZipFile(string szInputDirectory, string szOutPath, string pat, SearchFileType type)
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
				
				string[] aryFilePath = Directory.GetFiles(szInputDirectory,
				                                          string.Format("{0}.{1}", pat, type.ToString()), SearchOption.AllDirectories);
				for(int i=0; i<aryFilePath.Length; i++)
				{
					if (!Filter(aryFilePath[i]))
					{
						string path 			= aryFilePath[i].Replace('\\', '/');
						string zipEntryName 	= path.Substring(iStart + 1, path.Length - iStart - 1);
						
						ZipEntry entry = new ZipEntry(zipEntryName);
						stream.PutNextEntry(entry);
						
						EditorUtility.DisplayCancelableProgressBar(zipEntryName, zipEntryName,
						                                          (float) i / (float)(aryFilePath.Length - 1));
						
						using(FileStream fs = File.OpenRead(path))
						{
							byte[] byData = new byte[fs.Length];
							
							int nReadLength = fs.Read(byData, 0, byData.Length);
							if (nReadLength == fs.Length)
							{
								stream.Write(byData, 0, byData.Length);
							}
							
							fs.Close();
						}
						

					}
				}
				stream.Close();
			}
			
			zipFileStream.Close();
		}

		EditorUtility.ClearProgressBar();
	}
	
	public enum PType {
		m_HumanDescription, m_RootMotionBoneName, m_LastHumanDescriptionAvatarSource, m_AnimationType,
	}

	/// <summary>
	/// Gets the name of the asset path.
	/// </summary>
	/// <returns>The asset path name.</returns>
	/// <param name="szAssetPath">Size asset path.</param>
	/// <param name="szReplace">Size replace.</param>
	public static string	GetAssetPathName(string szAssetPath, string szReplace)
	{
		string[] arySplit = szAssetPath.Split('.');
		if (arySplit.Length > 0)
			return szAssetPath.Replace(arySplit[arySplit.Length - 1], szReplace);
		
		return szAssetPath;
	}

	/// <summary>
	/// Sets the name of the root motion bone.
	/// </summary>
	/// <param name="imp">Imp.</param>
	/// <param name="szValue">Size value.</param>
	public static void 		ApplyRootMotionBoneName(ModelImporter imp, string szValue)
	{
		SerializedObject impSO 			= new SerializedObject(imp);
		SerializedProperty rootMotion 	= impSO.FindProperty(PType.m_HumanDescription.ToString()).FindPropertyRelative(PType.m_RootMotionBoneName.ToString());
		rootMotion.stringValue 			= szValue;

		SerializedProperty type			= impSO.FindProperty(PType.m_AnimationType.ToString());
		type.enumValueIndex				= 2;

		impSO.ApplyModifiedProperties();
		impSO.Dispose();
	}

	/// <summary>
	/// Imports the model.
	/// </summary>
	/// <returns><c>true</c>, if model was imported, <c>false</c> otherwise.</returns>
	/// <param name="szAssetPath">Size asset path.</param>
	public static bool		ImportModel(string szAssetPath)
	{
		ModelImporter imp = ModelImporter.GetAtPath(szAssetPath) as ModelImporter;
		if (!imp)
			throw new System.NullReferenceException(szAssetPath);

		ApplyRootMotionBoneName(imp, imp.transformPaths[1]);

		// reset the model importer, reimport the model file
		imp.SaveAndReimport();

		return true;
	}

	
	/// <summary>
	/// Imports the clip.
	/// </summary>
	/// <returns>The clip.</returns>
	/// <param name="animator">Animator.</param>
	/// <param name="aryClip">Ary clip.</param>
	public static void		ImportClip(string szAssetPath, Animator animator, List<string> aryClip)
	{
		UnityEditor.Animations.AnimatorController ac = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(
			GetAssetPathName(szAssetPath, SearchFileType.controller.ToString())
			);
		// set animation controller
		animator.runtimeAnimatorController = ac;
		
		// clearup old animation state
		for(int i=0; i<ac.layers[0].stateMachine.states.Length; i++)
		{
			ac.layers[0].stateMachine.RemoveState(ac.layers[0].stateMachine.states[i].state);
		}
		
		ImprtState(animator, aryClip, ac.layers[0].stateMachine, 100, 60);
	}
	
	/// <summary>
	/// Imprts the state.
	/// </summary>
	/// <param name="animator">Animator.</param>
	/// <param name="aryClip">Ary clip.</param>
	/// <param name="machine">Machine.</param>
	public static void 		ImprtState(Animator animator, List<string> aryClipPath,
	                               	UnityEditor.Animations.AnimatorStateMachine machine, int nOffsetX, int nOffsetY)
	{
		machine.entryPosition 		= new Vector3(machine.entryPosition.x + nOffsetX, machine.entryPosition.y, 0);
		machine.anyStatePosition	= new Vector3(machine.entryPosition.x, machine.entryPosition.y - nOffsetY, 0);
		machine.exitPosition		= new Vector3(machine.entryPosition.x, machine.entryPosition.y + nOffsetY, 0);
		
		for(int i=0; i<aryClipPath.Count; i++)
		{
			ModelImporter imp = ModelImporter.GetAtPath(aryClipPath[i]) as ModelImporter;
			if (!imp)
				throw new System.NullReferenceException(aryClipPath[i]);
			
			ApplyRootMotionBoneName(imp, imp.transformPaths[1]);
			
			string szClipName 	= imp.assetPath.Substring(
				imp.assetPath.LastIndexOf('@') + 1);
			szClipName 			= szClipName.Remove(szClipName.LastIndexOf('.'));
			
			ModelImporterClipAnimation clipImp = new ModelImporterClipAnimation ();
			if (imp.clipAnimations.Length > 0)
				clipImp = imp.clipAnimations[0];
			
			// load animation clip
			AnimationClip clip = AssetDatabase.LoadAssetAtPath(imp.assetPath, typeof(AnimationClip)) as AnimationClip;
			if (clip)
			{
				AnimationClipSettings set = AnimationUtility.GetAnimationClipSettings(clip);
				if (set != default(AnimationClipSettings))
				{
					clipImp.firstFrame 	= set.startTime * clip.frameRate;
					clipImp.lastFrame 	= set.stopTime  * clip.frameRate;
				}
			}
			
			clipImp.maskType				= ClipAnimationMaskType.CreateFromThisModel;
			clipImp.keepOriginalOrientation = true;
			clipImp.keepOriginalPositionXZ	= true;
			clipImp.keepOriginalPositionY	= true;
			clipImp.lockRootRotation		= true;
			clipImp.lockRootHeightY			= true;
			clipImp.name					= szClipName;
			
			// set loop animation
			string[] aryFiliter = {
				"idle", "move", "run", "walk", "fly", "turn"
			};
			foreach(string fliter in aryFiliter)
			{
				if (szClipName.ToLower().Contains(fliter))
				{
					clipImp.loopTime = true;
					break;
				}
			}
			
			imp.clipAnimations = new ModelImporterClipAnimation[]{clipImp};
			
			// reset model importer
			imp.SaveAndReimport();
			
			Object[] aryClip = AssetDatabase.LoadAllAssetsAtPath(imp.assetPath);
			for(int c=0; c<aryClip.Length; c++)
			{
				Object o = aryClip[c];
				
				if (o.GetType() == typeof(AnimationClip) && !o.name.Contains(SearchFileType.preview.ToString()))
				{
					Vector3 vStatePosition = new Vector3(machine.entryPosition.x + 200, machine.entryPosition.y + i * 65, 0);
					
					// create animator state
					UnityEditor.Animations.AnimatorState state = machine.AddState(o.name, vStatePosition);
					if (state)
					{
						state.motion = o as Motion;
						
						// set default state
						if (state.name.ToLower().Contains(aryFiliter[0]))
						{
							machine.defaultState = state;
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Creates the animator.
	/// </summary>
	/// <param name="skeleton">Skeleton.</param>
	/// <param name="aryClip">Ary clip.</param>
	public static void 		CreateAnimator(GameObject skeleton, List<string> aryClip, string szModelPath)
	{
		if (ImportModel(szModelPath))
		{
			Animation old = skeleton.GetComponent<Animation>();
			if (old)
				GameObject.DestroyImmediate(old, true);

			// add a animator contorller
			Animator animator = skeleton.GetComponent<Animator>();
			if (!animator)
				animator = skeleton.AddComponent<Animator>();

			string skeletonAssetPath = AssetDatabase.GetAssetPath(skeleton);
			if (!string.IsNullOrEmpty(skeletonAssetPath))
			{
				ImportClip(skeletonAssetPath, animator, aryClip);
			}
		}
	}

	/// <summary>
	/// Creates the empty prefab.
	/// </summary>
	/// <returns>The empty prefab.</returns>
	/// <param name="goItem">Go item.</param>
	/// <param name="szPath">Size path.</param>
	public static GameObject	CreateEmptyPrefab(GameObject goItem, string szPath)
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
	/// Gets the asset path.
	/// </summary>
	/// <returns>The asset path.</returns>
	/// <param name="szPath">Size path.</param>
	public static string		GetAssetPath(string szPath)
	{
		string szFilePath	= szPath.Replace("\\", "/");
		
		// get the assetbundl file path
		return szFilePath.Substring(
			Application.dataPath.Length - 6, szFilePath.Length - Application.dataPath.Length + 6
			);
	}

	/// <summary>
	/// Extracts the skeleton.
	/// </summary>
	/// <returns>The skeleton.</returns>
	/// <param name="resource">Resource.</param>
	/// <param name="szOutPath">Size out path.</param>
	public static GameObject	ExtractSkeleton(Object resource, string szOutPath, bool bSplit)
	{
		GameObject fbx = GameObject.Instantiate(resource) as GameObject;
		if (!fbx)
			throw new System.NullReferenceException();

		if (bSplit)
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
		}

		SkinnedMeshRenderer skin = fbx.GetComponent<SkinnedMeshRenderer>();
		if (!skin)
			skin = fbx.AddComponent<SkinnedMeshRenderer>();

		GameObject prefab = CreateEmptyPrefab(fbx, szOutPath);
		if (!prefab)
			throw new System.NullReferenceException();

		GameObject.DestroyImmediate(fbx);

		return prefab;
	}

	public enum SkinMeshType {
		mesh, bone,
	}

	/// <summary>
	/// Extracts the skin mesh.
	/// </summary>
	/// <param name="resource">Resource.</param>
	/// <param name="szOutPath">Size out path.</param>
	/// <param name="bSingleFile">If set to <c>true</c> b single file.</param>
	public static void 		ExtractSkinMesh(UnityEngine.Object[] aryResource, string szOutPath)
	{
		foreach(Object resource in aryResource)
		{
			GameObject fbx = GameObject.Instantiate(resource) as GameObject;
			if (!fbx)
				throw new System.NullReferenceException();
			
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
					GameObject prefab = CreateEmptyPrefab(clones, string.Format("{0}/{1}@{2}.prefab", 
					                                                            szOutPath, SkinMeshType.mesh.ToString(), render.name));
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
				GameObject prefab = CreateEmptyPrefab(clones, string.Format("{0}/{1}@{2}.prefab", 
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
					
					string szHolderPath = string.Format("{0}/{1}@{2}.asset", GetAssetPath(szOutPath), SkinMeshType.bone.ToString(), mesh.name);
					AssetDatabase.CreateAsset(holder, 
					                          szHolderPath);
				}
				
				GameObject.DestroyImmediate(clones);
			}
			
			GameObject.DestroyImmediate(fbx);
		}
	}

	/// <summary>
	/// Clears the directory.
	/// </summary>
	/// <param name="szPath">Size path.</param>
	public static void 	ClearDirectory(string szPath)
	{
		// clear old file
		string[] aryDelete = System.IO.Directory.GetFiles(
			szPath, "*.*", SearchOption.AllDirectories
			);
		foreach(string delete in aryDelete)
		{
			File.Delete(delete);
		}
	}
}
