using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using System.IO;
using System.Timers;

public class SingleEditor
{
	[MenuItem("Build/Build single package")]
	public static void 	DoSinglePackage()
	{
		BuildSinglePackage("Single", new Build.BuildDirectory[]{
			new Build.BuildDirectory(Application.dataPath + "/Design", "*.bytes"),
		});
	}

	/// <summary>
	/// Builds the single package.
	/// </summary>
	/// <param name="szPackageName">Size package name.</param>
	/// <param name="outZipFile">If set to <c>true</c> out zip file.</param>
	/// <param name="aryDirectory">Ary directory.</param>
	static void 	BuildSinglePackage(string szPackageName, params Build.BuildDirectory[] aryDirectory)
	{
		string szOutPath = Build.GetTempPath(szPackageName);
		
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

		foreach(Build.BuildDirectory directory in aryDirectory)
		{
			string[] aryFile = System.IO.Directory.GetFiles(directory.directory, directory.pattern, System.IO.SearchOption.AllDirectories);
			foreach(string path in aryFile)
			{
				if (!path.Contains(".meta"))
				{
					File.Copy(path, string.Format("{0}/{1}", szOutPath, Build.GetFileName(path)), true);
				}
			}
		}
		
		if (!Directory.Exists(Build.OutUrl))
			Directory.CreateDirectory(Build.OutUrl);
		
		string zipOutPath = string.Format("{0}/{1}.zip", 
		                                  Build.OutUrl, szPackageName);
		// create zip file
		Build.CreateZipFile(szOutPath, zipOutPath);
	}

}
