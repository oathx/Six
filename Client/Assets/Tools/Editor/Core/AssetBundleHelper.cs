using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Timers;
using LitJson;

/// <summary>
/// Xml editor helper.
/// </summary>
public class AssetBundleHelper
{
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

	// build asset directory
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

	/// <summary>
	/// Builds the asset bundles.
	/// </summary>
	/// <param name="szVersion">Size version.</param>
	/// <param name="szPackageName">Size package name.</param>
	/// <param name="outZipFile">If set to <c>true</c> out zip file.</param>
	/// <param name="cleaup">If set to <c>true</c> cleaup.</param>
	/// <param name="szDirectory">Size directory.</param>
	/// <param name="szPattern">Size pattern.</param>
	public static void 	BuildAssetBundles(string szPackageName, 
	                                      bool outZipFile, bool cleaup, string szDirectory, string szPattern)
	{
		string szTargetDirectory = GetTempPath(szPackageName);
		
		// create out directory
		if (!File.Exists(szTargetDirectory))
		{
			Directory.CreateDirectory(szTargetDirectory);
		}
		else
		{
			if (cleaup)
			{
				string[] aryDeleteFile = System.IO.Directory.GetFiles(szTargetDirectory, "*.*", 
				                                                System.IO.SearchOption.AllDirectories);
				for(int i=0; i<aryDeleteFile.Length; i++)
				{
					File.Delete(aryDeleteFile[i]);
				}
			}
		}
		
		List<string> 
			aryAssetName = new List<string>();

		string[] aryFile = System.IO.Directory.GetFiles(szDirectory, szPattern, System.IO.SearchOption.AllDirectories);
		foreach(string path in aryFile)
		{
			string szFilePath	= path.Replace("\\", "/");
			
			// get the assetbundl file path
			string szAssetPath 	= szFilePath.Substring(
				Application.dataPath.Length - 6, szFilePath.Length - Application.dataPath.Length + 6
				);
			
			
			aryAssetName.Add(szAssetPath);
		}

		AssetBundleBuild build 		= new AssetBundleBuild();
		build.assetNames 			= aryAssetName.ToArray();
		build.assetBundleVariant	= Build.ExtName.unity3d.ToString();
		build.assetBundleName		= szPackageName;
		
		string szOutPath	= szTargetDirectory.Replace("\\", "/");
		
		// get the assetbundl file path
		string szTempPath 	= szOutPath.Substring(
			Application.dataPath.Length - 6, szOutPath.Length - Application.dataPath.Length + 6
			);
		
#if UNITY_EDITOR
		BuildPipeline.BuildAssetBundles(szTempPath, new AssetBundleBuild[]{build}, 
			BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows);
		
#elif UNITY_ANDROID
		BuildPipeline.BuildAssetBundles(szTempPath, new AssetBundleBuild[]{build},
			BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.Android);
#elif UNITY_IOS
		BuildPipeline.BuildAssetBundles(szTempPath, new AssetBundleBuild[]{build},
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

	/// <summary>
	/// Gets the temp path.
	/// </summary>
	/// <returns>The temp path.</returns>
	/// <param name="szPackageName">Size package name.</param>
	public static string	GetTempPath(string szPackageName)
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

	/// <summary>
	/// Filiter the specified szName.
	/// </summary>
	/// <param name="szName">Size name.</param>
	public static bool	Filter(string path)
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
	/// Creates the zip file.
	/// </summary>
	/// <param name="szOutDirectory">Size out directory.</param>
	public static void 	CreateZipFile(string szInputDirectory, string szOutPath)
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
				for(int i=0; i<aryFilePath.Length; i++)
				{
					if (!Filter(aryFilePath[i]))
					{
						string path 			= aryFilePath[i].Replace('\\', '/');
						string zipEntryName 	= path.Substring(iStart + 1, path.Length - iStart - 1);
						
						ZipEntry entry = new ZipEntry(zipEntryName);
						stream.PutNextEntry(entry);
						
						EditorUtility.DisplayCancelableProgressBar("Zip File Compress ...", zipEntryName,
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
						
						Debug.Log("Add file to pkg " + szOutPath + " >> " + zipEntryName);
					}
				}
				stream.Close();
			}
			
			zipFileStream.Close();
		}
		
		EditorUtility.ClearProgressBar();
	}
}
