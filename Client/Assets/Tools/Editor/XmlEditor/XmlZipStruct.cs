using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Zip word.
/// </summary>
public enum ZipWord
{
	InputFileType,
	FileList
}

/// <summary>
/// Input file.
/// </summary>
public class InputFile : XmlStruct
{
	[ReadonlyField]
	public string			Name
	{ get; set; }

	/// <summary>
	/// Gets or sets the size.
	/// </summary>
	/// <value>The size.</value>
	[ReadonlyField]
	public int 				Size
	{ get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="InputFile"/> class.
	/// </summary>
	public InputFile()
	{

	}

	/// <summary>
	/// Initializes a new instance of the <see cref="InputFile"/> class.
	/// </summary>
	/// <param name="szName">Size name.</param>
	/// <param name="nSize">N size.</param>
	public InputFile(string szName, int nSize)
	{
		Name = szName; Size = nSize;
	}
}

/// <summary>
/// Input directory.
/// </summary>
public class InputSetting : XmlStruct
{
	/// <summary>
	/// Gets or sets the type.
	/// </summary>
	/// <value>The type.</value>
	public SearchFileType	InputFileType
	{ get; set; }

	/// <summary>
	/// Gets or sets the input path.
	/// </summary>
	/// <value>The input path.</value>
	[DirectoryField]
	public string			InputPath
	{ get; set; }

	/// <summary>
	/// The file list.
	/// </summary>
	public List<InputFile>	
		FileList = new List<InputFile>();

	/// <summary>
	/// Initializes a new instance of the <see cref="InputSetting"/> class.
	/// </summary>
	public InputSetting()
	{
		InputFileType = SearchFileType.prefab;
	}
}

/// <summary>
/// Out setting.
/// </summary>
public class OutSetting : XmlStruct
{
	/// <summary>
	/// Gets or sets the version.
	/// </summary>
	/// <value>The version.</value>
	public int				Version
	{ get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="OutSetting"/> out zip.
	/// </summary>
	/// <value><c>true</c> if out zip; otherwise, <c>false</c>.</value>
	public bool				OutZip
	{ get; set; }

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="OutSetting"/> clearup out path.
	/// </summary>
	/// <value><c>true</c> if clearup out path; otherwise, <c>false</c>.</value>
	public bool				ClearupOutPath
	{ get; set; }

	/// <summary>
	/// Gets or sets the type.
	/// </summary>
	/// <value>The type.</value>
	public SearchFileType	OutFileType
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the input path.
	/// </summary>
	/// <value>The input path.</value>
	public string			Name
	{ get; set; }
	
	/// <summary>
	/// Initializes a new instance of the <see cref="InputSetting"/> class.
	/// </summary>
	public OutSetting()
	{
		OutFileType = SearchFileType.unity3d;
	}
}

/// <summary>
/// Zip pack.
/// </summary>
public class ZipPack : XmlStruct
{
	[ReadonlyField]
	public string			Name
	{ get; set; }

	/// <summary>
	/// Gets or sets the out setting.
	/// </summary>
	/// <value>The out setting.</value>
	public OutSetting		outSetting
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the input setting.
	/// </summary>
	/// <value>The input setting.</value>
	public InputSetting		inputSetting
	{ get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="ZipPack"/> class.
	/// </summary>
	public ZipPack()
	{
		inputSetting 	= new InputSetting();
		outSetting		= new OutSetting();
	}
}

/// <summary>
/// Zip xml struct.
/// </summary>
public class XmlZipStruct : XmlStruct
{
	/// <summary>
	/// Gets or sets the out URL.
	/// </summary>
	/// <value>The out URL.</value>
	[ReadonlyField]
	public string		OutDirectory
	{ get; set; }

	/// <summary>
	/// The pack.
	/// </summary>
	public List<ZipPack>	
		Pack = new List<ZipPack>();

	/// <summary>
	/// Initializes a new instance of the <see cref="ZipXmlStruct"/> class.
	/// </summary>
	public XmlZipStruct()
	{
		OutDirectory = AssetBundleHelper.OutUrl.Substring(Application.dataPath.Length - 6);
	}

	/// <summary>
	/// Create this instance.
	/// </summary>
	public void Create()
	{
		Pack.Add(new ZipPack());
	}

	/// <summary>
	/// Save the specified szPath.
	/// </summary>
	/// <param name="szPath">Size path.</param>
	public override void Save (string szPath)
	{
		base.Save (szPath);

		foreach(ZipPack pack in Pack)
		{
			BuildAssetBundles(pack);
		}
	}

	/// <summary>
	/// Build the specified pack.
	/// </summary>
	/// <param name="pack">Pack.</param>
	public virtual void	BuildAssetBundles(ZipPack pack)
	{
		AssetBundleHelper.BuildDirectory dir = new AssetBundleHelper.BuildDirectory(pack.inputSetting.InputPath,
		                                                    pack.inputSetting.InputFileType.ToString());

		string szOutPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6) + pack.inputSetting.InputPath;
		AssetBundleHelper.BuildAssetBundles(pack.outSetting.Name, pack.outSetting.OutZip, pack.outSetting.ClearupOutPath,
		                                    szOutPath, "*." + pack.inputSetting.InputFileType.ToString().ToLower());

	}
}
