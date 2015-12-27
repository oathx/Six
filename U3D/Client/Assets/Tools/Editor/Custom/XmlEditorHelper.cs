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
#if UNITY_STANDALONE || UNITY_EDITOR
			return Application.streamingAssetsPath + "/Win";
#elif UNITY_ANDROID
			return Application.streamingAssetsPath + "Android";
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
#if UNITY_STANDALONE || UNITY_EDITOR
			return Application.dataPath + "/Mid/Win";
#elif UNITY_ANDROID
			return Application.dataPath + "/Mid/Android";
#elif UNITY_IPHONE
			return Application.dataPath + "/Mid/iOS";
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

		// Priority search prefab file
		string pattern = string.Format("*.{0}", 
		                               type.ToString().ToLower());

		string[] aryFile = System.IO.Directory.GetFiles(szInputPath, pattern, SearchOption.AllDirectories);
		foreach(string path in aryFile)
		{
			string szPath = path.Substring(Application.dataPath.Length - 6);
			aryReturn.Add(
				szPath.Replace("\\", "/")
				);
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
}
