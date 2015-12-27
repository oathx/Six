using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityThreading;
using System.IO;
using System.Text;
using System.Net;
using ICSharpCode.SharpZipLib.Zip;

public enum WorkState
{
	HS_SUCCESS,
	HS_FAILURE,
	HS_DOWNLOAD,
	HS_COMPLETED,
	HS_DECOMPRE,
	HS_TIMEOUT,
	HS_FINISHED,
}

public delegate bool 	HttpWorkEvent(WorkState curState, string szUrl, string szPath,
                                    int nPosition, int nReadSpeed, int nFilength, string szVersion);


public enum UrlType
{
	URL_STREAM,
	URL_LOCAL,
	URL_HTTP,
}

/// <summary>
/// Download manager.
/// </summary>
public class HttpDownloadManager : SimpleSingleton<HttpDownloadManager> 
{
	/// <summary>
	/// Gets the type of the URL.
	/// </summary>
	/// <returns>The URL type.</returns>
	/// <param name="szUrl">Size URL.</param>
	public UrlType		GetUrlType(string szUrl)
	{
		if (szUrl[0] == 'f' && szUrl[1] == 'i' && szUrl[2] == 'l' && szUrl[3] == 'e')
			return UrlType.URL_STREAM;
		
		if (szUrl[0] == 'h' && szUrl[1] == 't' && szUrl[2] == 't' && szUrl[3] == 'p')
			return UrlType.URL_HTTP;
		
		return UrlType.URL_LOCAL;
	}

	/// <summary>
	/// Download the specified work and evtCallback.
	/// </summary>
	/// <param name="work">Work.</param>
	/// <param name="evtCallback">Evt callback.</param>
	public WorkState	Download(HttpWork work, HttpWorkEvent evtCallback)
	{
		
		string szFilePath = string.Format("{0}/{1}", 
		                                  work.FilePath, GetFileName(work.Url));

		UrlType type = GetUrlType(work.Url);
		if (type == UrlType.URL_LOCAL)
		{
			// download completed
			evtCallback(WorkState.HS_COMPLETED, work.Url,
			            work.Url, 1, 1, 1, work.Version);

			return WorkState.HS_SUCCESS;
		}

		int nFileLength = GetFileLength(work.Url);
		if (nFileLength <= 0)
			return WorkState.HS_FAILURE;

		// create http web connected
		WebClient web = new WebClient ();
		
		// open http file
		Stream stream = web.OpenRead(work.Url);
		if (!stream.CanRead)
			return WorkState.HS_FAILURE;

		// create local file
		FileStream fs = new FileStream (szFilePath, FileMode.OpenOrCreate);
		
		// current read length
		int nReadLength 	= 0;
		int nMinLength		= work.MinReadSize == 0 ? nFileLength : work.MinReadSize;
		int nFilePostion	= 0;
		int nReadSpeed		= 0;
		int nSecSpeed		= 0;
		int nPrevTick 		= System.Environment.TickCount;
		
		// start time
		byte[] byBuffer = new byte[nMinLength];
		do{
			// read file bytes
			nReadLength = stream.Read(byBuffer, 0, byBuffer.Length);
			if (nReadLength == 0)
				break;
			
			// write buffer data to file
			fs.Write(byBuffer, 0, nReadLength);
			
			nFilePostion 	+= nReadLength;
			nReadSpeed		+= nReadLength;
			
			int nElapsed = System.Environment.TickCount - nPrevTick;
			if (nElapsed >= 1000)
			{
				nPrevTick 	= System.Environment.TickCount;
				nSecSpeed	= nReadSpeed;
				nReadSpeed 	= 0;
			}
			
			evtCallback(WorkState.HS_DOWNLOAD, work.Url, szFilePath, 
			            nFilePostion, nSecSpeed, nFileLength, work.Version);
			
		}while(nFilePostion < nFileLength);
		
		// close zip file
		fs.Close();

		// download completed
		evtCallback(WorkState.HS_COMPLETED, work.Url,
		            szFilePath, nFilePostion, nSecSpeed, nFileLength, work.Version);

		return WorkState.HS_SUCCESS;
	}

	/// <summary>
	/// Decompression the specified szZipFilePath and szTargetDirectory.
	/// </summary>
	/// <param name="szZipFilePath">Size zip file path.</param>
	/// <param name="szTargetDirectory">Size target directory.</param>
	public WorkState	Decompression(string szZipFilePath, string szTargetDirectory, string szVersion, HttpWorkEvent evtCallback)
	{
		FileStream zipFile = File.OpenRead (szZipFilePath);
		if (zipFile.Length <= 0)
			return WorkState.HS_FAILURE;

		using (ZipInputStream s = new ZipInputStream(zipFile))
		{
			ZipEntry entry;
			while((entry = s.GetNextEntry()) != null)
			{
				string szDirectoryName = string.Format("{0}/{1}/", szTargetDirectory, Path.GetDirectoryName(entry.Name));
				if (!Directory.Exists(szDirectoryName))
				{
					Directory.CreateDirectory(szDirectoryName);
				}
				
				string szFileName = Path.GetFileName(entry.Name);
				if (!string.IsNullOrEmpty(szFileName))
				{
					using (FileStream sw = File.Open(szDirectoryName + szFileName, FileMode.OpenOrCreate))
					{
						int nReadSize	= 0;
						byte[] byBuffer = new byte[1024];
						int nPosition	= 0;
						
						int nReadSpeed	= 0;
						int nSecSpeed	= 0;
						int nPrevTick 	= System.Environment.TickCount;
						
						do{
							nReadSize = s.Read(byBuffer, 0, byBuffer.Length);
							if (nReadSize > 0)
							{
								sw.Write(byBuffer, 0, nReadSize);
								
								nPosition 	+= nReadSize;
								nReadSpeed	+= nReadSize;
								
								int nElapsed = System.Environment.TickCount - nPrevTick;
								if (nElapsed >= 1000)
								{
									nPrevTick 	= System.Environment.TickCount;
									nSecSpeed	= nReadSpeed;
									nReadSpeed 	= 0;
								}
								
								evtCallback(WorkState.HS_DECOMPRE, 
								            szZipFilePath, szFileName, nPosition, nSecSpeed, (int)s.Length, szVersion);
							}
							
						}while(nPosition < s.Length);
						
#if OPEN_DEBUG_LOG
						Debug.Log("Decompression file : " + sw.Name + " length " + sw.Length);
#endif
						sw.Close();
					}
				}
			}
			
			zipFile.Close();
		}

		evtCallback(WorkState.HS_COMPLETED, 
		            szZipFilePath, szTargetDirectory, 1, 1, 1, szVersion);

		return WorkState.HS_SUCCESS;
	}

	/// <summary>
	/// Gets the length of the file.
	/// </summary>
	/// <returns>The file length.</returns>
	/// <param name="szUrl">Size URL.</param>
	public int			GetFileLength(string szUrl)
	{
		// create web requeset
		WebRequest req = WebRequest.Create(szUrl);
		
		// get web response
		WebResponse rp = req.GetResponse() as WebResponse;
		
		// close the requeset
		rp.Close();
		
		return (int)(rp.ContentLength);
	}

	/// <summary>
	/// Gets the name of the file.
	/// </summary>
	/// <returns>The file name.</returns>
	/// <param name="szUrl">Size URL.</param>
	public string		GetFileName(string szUrl)
	{
		string[] arySplit = szUrl.Split('/');
		if (arySplit.Length > 0)
			return arySplit [arySplit.Length - 1];
		
		return szUrl;
	}
	
	/// <summary>
	/// Gets the file path.
	/// </summary>
	/// <returns>The file path.</returns>
	/// <param name="szPath">Size path.</param>
	public string		GetFilePath(string szPath)
	{
		int iPos = szPath.LastIndexOf("\\");
		if (iPos < 0)
			iPos = szPath.LastIndexOf("/");
		
		return szPath.Substring(0, iPos);
	}	
}















