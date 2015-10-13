using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

public class Build {
	[MenuItem("Build/Build all resource to zip")]
	static void 	BuildZipFileToStreamasset()
	{
		string[,] aryPackage = new string[,]{
			{"AssetBundle", "*.prefab"},
		};

		for(int i=0; i<aryPackage.GetLength(0); i++)
		{
			BuildFile(aryPackage[i, 0], aryPackage[i, 1], true, Application.dataPath + "/Art");
		}
	}
	
	/// <summary>
	/// Builds the zip file.
	/// </summary>
	/// <param name="aryPath">Ary path.</param>
	static void 	BuildFile(string szPackageName, string pattern, bool outZipFile, params string[] aryDirectory)
	{
#if UNITY_EDITOR_WIN
		string szOutPath = string.Format("{0}/Win/{1}", 
		                                 Application.streamingAssetsPath, szPackageName);
#elif UNITY_ANDROID
		string szOutPath = string.Format("{0}/Android/{1}", 
		                                 Application.streamingAssetsPath, szPackageName);
#elif UNITY_IOS
		string szOutPath = string.Format("{0}/iOS/{1}", 
		                                 Application.streamingAssetsPath, szPackageName);
#endif

		foreach(string directory in aryDirectory)
		{
			string[] aryFile = System.IO.Directory.GetFiles(directory, pattern, System.IO.SearchOption.AllDirectories);
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
					AssetImporter ai = AssetImporter.GetAtPath(depend);
					if (string.IsNullOrEmpty(ai.assetBundleName))
					{
						ai.assetBundleName = AssetDatabase.AssetPathToGUID(depend);
					}

#if UNITY_EDITOR
					Debug.Log(" >> depend : " + depend);
#endif 
				}
			}

			string szTargetDirectory = szOutPath;

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
#if UNITY_EDITOR_WIN
				string szPackagePath = string.Format("{0}/Win", 
				                                 Application.streamingAssetsPath);
#elif UNITY_ANDROID
				string szPackagePath = string.Format("{0}/Android", 
				                                 Application.streamingAssetsPath);
#elif UNITY_IOS
				string szPackagePath = string.Format("{0}/iOS", 
				                                 Application.streamingAssetsPath);
#endif
				if (!Directory.Exists(szPackagePath))
					Directory.CreateDirectory(szPackagePath);

				string zipOutPath = string.Format("{0}/{1}.pkg", szPackagePath, szPackageName);
				// create zip file
				CreateZipFile(szTargetDirectory, zipOutPath);
			}
		}
	}

	/// <summary>
	/// Creates the zip file.
	/// </summary>
	/// <param name="szOutDirectory">Size out directory.</param>
	static void 	CreateZipFile(string szInputDirectory, string szOutPath)
	{
		if (File.Exists(szOutPath))
			File.Delete(szOutPath);

		FileStream zipFileStream = File.Open(szOutPath, FileMode.OpenOrCreate);
		if (zipFileStream.CanWrite)
		{
			using(ZipOutputStream stream = new ZipOutputStream(zipFileStream))
			{
				// 0 - store only to 9 - means best compression
				stream.SetLevel(9);

				string[] aryFilePath = Directory.GetFiles(szInputDirectory);
				foreach(string file in aryFilePath)
				{
					if (!file.Contains(".meta"))
					{
						string szZipEntryName = file.Substring(Application.dataPath.Length + 5,
						                                      file.Length - Application.dataPath.Length - 5);

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

#if UNITY_EDITOR
						Debug.Log("Add file to pkg " + szInputDirectory + " >> " + szZipEntryName);
#endif
					}
				}

				stream.Close();
			}
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
}
