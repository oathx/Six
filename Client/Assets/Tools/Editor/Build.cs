using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;

public class Build {
	[MenuItem("Build/Zip")]
	static void 	BuildZipFileToStreamasset()
	{
		BuildZipFile(Application.dataPath + "/Assetbundle", 
		             Application.dataPath + "/Art/Model/Character");
	}

	/// <summary>
	/// Builds the zip file.
	/// </summary>
	/// <param name="aryPath">Ary path.</param>
	static void 	BuildZipFile(string szOutPath, params string[] aryDirectory)
	{
		foreach(string directory in aryDirectory)
		{
			string[] aryFile = System.IO.Directory.GetFiles(directory, "*.prefab", System.IO.SearchOption.AllDirectories);
			foreach(string path in aryFile)
			{
				string szFilePath	= path.Replace("\\", "/");

				// get the assetbundl file path
				string szAssetPath 	= szFilePath.Substring(
					Application.dataPath.Length - 6, szFilePath.Length - Application.dataPath.Length + 6
					);
#if UNITY_EDITOR
				Debug.Log("Build asset path " + szAssetPath);
#endif
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
				}
			}

			string szTargetDirectory = szOutPath;

			// create out directory
			if (!File.Exists(szTargetDirectory))
				Directory.CreateDirectory(szTargetDirectory);

#if UNITY_EDITOR_WIN
			BuildPipeline.BuildAssetBundles(szTargetDirectory, 
			                                BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.StandaloneWindows);
#endif

			CreateZipFile(szTargetDirectory);
		}
	}

	/// <summary>
	/// Creates the zip file.
	/// </summary>
	/// <param name="szOutDirectory">Size out directory.</param>
	static void 	CreateZipFile(string szInputDirectory)
	{
		string szZipFilePath = string.Format("{0}/{1}.pkg", Application.dataPath, typeof(AssetBundle).Name);
		if (File.Exists(szZipFilePath))
			File.Delete(szZipFilePath);

		FileStream zipFileStream = File.Open(szZipFilePath, FileMode.OpenOrCreate);
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
						string szZipEntryName = file.Substring(Application.dataPath.Length + 1,
						                                      file.Length - Application.dataPath.Length - 1);

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
						Debug.Log("Make zip " + szInputDirectory + " >> " + szZipEntryName);
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
