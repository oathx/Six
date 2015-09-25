using UnityEngine;
using System.Collections;

/// <summary>
/// Http file type.
/// </summary>
public enum HttpFileType
{
	HFT_UNKNOWN,		// thow a unknown execption
	HFT_ZIP,			// if the decomposition is ZIP file, will be automatically extracted
	HFT_APK,			// if the decomposition is APK file, notify main thread progress
	HFT_SIMPLE,			// if the decomposition is simple file, copy the file to cache directory
}

/// <summary>
/// Http work.
/// </summary>
public class HttpWork
{
	/// <summary>
	/// Gets or sets the URL.
	/// </summary>
	/// <value>The URL.</value>
	public string		Url
	{ get; set; }

	/// <summary>
	/// Gets or sets the file path.
	/// </summary>
	/// <value>The file path.</value>
	public string		FilePath
	{ get; set; }

	/// <summary>
	/// Gets or sets the version.
	/// </summary>
	/// <value>The version.</value>
	public string		Version
	{ get; set; }

	/// <summary>
	/// Gets or sets the type.
	/// </summary>
	/// <value>The type.</value>
	public HttpFileType	Type
	{ get; set; }

	/// <summary>
	/// Gets or sets the timeout.
	/// </summary>
	/// <value>The timeout.</value>
	public int 			Timeout
	{ get; set; }

	/// <summary>
	/// Gets or sets the minimum size.
	/// </summary>
	/// <value>The minimum size.</value>
	public int 			MinReadSize
	{ get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="HttpWork"/> class.
	/// </summary>
	/// <param name="szURL">Size UR.</param>
	/// <param name="szPath">Size path.</param>
	/// <param name="szVersion">Size version.</param>
	/// <param name="fileType">File type.</param>
	public HttpWork(string szURL, string szPath, string szVersion, HttpFileType fileType, int nTimeout)
	{
		Url 		= szURL; 
		FilePath 	= szPath; 
		Version 	= szVersion; 
		Type 		= fileType;
		Timeout		= nTimeout;
		MinReadSize	= 0;
	}	
}
