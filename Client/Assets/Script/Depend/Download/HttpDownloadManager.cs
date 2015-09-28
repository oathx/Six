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
	HS_DECOMPRESS,
	HS_INSTALL,
	HS_TIMEOUT,
	HS_FINISHED,
}

public delegate bool 	HttpWorkEvent(WorkState curState, string szUrl, string szPath,
                                    int nPosition, int nReadSpeed, int nFilength, string szVersion);

/// <summary>
/// Download manager.
/// </summary>
public class HttpDownloadManager : SimpleSingleton<HttpDownloadManager> 
{
	/// <summary>
	/// Download the specified workQueue.
	/// </summary>
	/// <param name="workQueue">Work queue.</param>
	public bool			Download(List<HttpWork> aryWork, HttpWorkEvent evtCallback)
	{
		Queue<HttpWork> workQueue = new Queue<HttpWork>();
		foreach(HttpWork work in aryWork)
		{
			workQueue.Enqueue(work);
		}

		UnityThreading.ActionThread thread = UnityThreadHelper.CreateThread( () => {

			WorkState curState 	= WorkState.HS_FAILURE;
			HttpWork curWork 	= default(HttpWork);

			while(aryWork.Count > 0)
			{
				curWork = workQueue.Dequeue();
				
				if (!string.IsNullOrEmpty(curWork.Url) && !string.IsNullOrEmpty(curWork.FilePath))
				{
					curState = DoHttpThreadWork(curWork, evtCallback);
					if (curState == WorkState.HS_FAILURE || curState == WorkState.HS_INSTALL)
						break;
				}
			}

			if (curState == WorkState.HS_SUCCESS)
			{
				UnityThreadHelper.Dispatcher.Dispatch( ()=> {
					evtCallback(WorkState.HS_FINISHED, curWork.Url, curWork.FilePath, 0, 0, 0, curWork.FilePath);
				});
			}
		});

		return true;
	}

	/// <summary>
	/// Dos the http work.
	/// </summary>
	/// <returns>The http work.</returns>
	/// <param name="work">Work.</param>
	public WorkState	DoHttpThreadWork(HttpWork work, HttpWorkEvent evtCallback)
	{
		int nFileLength = GetFileLength(work.Url);
		if (nFileLength <= 0)
		{
			UnityThreadHelper.Dispatcher.Dispatch( ()=> {
				evtCallback(WorkState.HS_FAILURE, work.Url, work.FilePath, 0, 0, 0, work.Version);
			});

			return WorkState.HS_FAILURE; 
		}

		WebClient web = new WebClient ();
		
		// open http file
		Stream stream = web.OpenRead(work.Url);
		if (stream.CanRead)
		{
			if (File.Exists(work.FilePath))
			{
				FileStream	existStream = File.Open(work.FilePath, FileMode.Open);
				long nTotalBytes		= existStream.Length;
				existStream.Close();

				if (nTotalBytes == nFileLength)
				{
					return DoFileProgress(work, nFileLength, evtCallback);
				}
				else
				{
					File.Delete(work.FilePath);
				}
			}
			else
			{
				// create local file
				FileStream fs = new FileStream (work.FilePath, FileMode.OpenOrCreate);

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
					
					evtCallback(WorkState.HS_DOWNLOAD, work.Url, work.FilePath, 
					         nFilePostion, nSecSpeed, nFileLength, work.Version);
					
				}while(nFilePostion < nFileLength);
				
				// close zip file
				fs.Close();

				UnityThreadHelper.Dispatcher.Dispatch( ()=> {
					evtCallback(nFilePostion != nFileLength ? WorkState.HS_FAILURE : WorkState.HS_COMPLETED, work.Url, work.FilePath, 
					            nFilePostion, nSecSpeed, nFileLength, work.Version);
				});

				if (nFilePostion == nFileLength)
				{
					return DoFileProgress(work, nFileLength, evtCallback);
				}
			}
		}

		return WorkState.HS_FAILURE;
	}

	/// <summary>
	/// Dos the file progress.
	/// </summary>
	/// <returns>The file progress.</returns>
	/// <param name="work">Work.</param>
	/// <param name="nFileLength">N file length.</param>
	public WorkState	DoFileProgress(HttpWork work, int nFileLength, HttpWorkEvent evtCallback)
	{
		switch(work.Type)
		{
		case HttpFileType.HFT_ZIP:
			return Decompression(work.FilePath, GetFilePath(work.FilePath), evtCallback, work.Version);
	
		case HttpFileType.HFT_APK:
			UnityThreadHelper.Dispatcher.Dispatch( ()=> {
				evtCallback(WorkState.HS_INSTALL, work.Url, work.FilePath,
				            nFileLength, nFileLength, nFileLength, work.Version);
			});
			return WorkState.HS_INSTALL;
		}

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

	/// <summary>
	/// Decompression the specified szZipFilePath and szTargetDirectory.
	/// </summary>
	/// <param name="szZipFilePath">Size zip file path.</param>
	/// <param name="szTargetDirectory">Size target directory.</param>
	public WorkState	Decompression(string szZipFilePath, string szTargetDirectory)
	{
		try{
			FastZip zip = new FastZip();
			zip.ExtractZip(szZipFilePath, szTargetDirectory, string.Empty);

			return WorkState.HS_SUCCESS;
		}
		catch(System.Exception e)
		{
			Debug.LogException(e);
		}

		return WorkState.HS_FAILURE;
	}

	/// <summary>
	/// Decompression the specified szSourcePath, szTargetPath, callback and szVersion.
	/// </summary>
	/// <param name="szSourcePath">Size source path.</param>
	/// <param name="szTargetPath">Size target path.</param>
	/// <param name="callback">Callback.</param>
	/// <param name="szVersion">Size version.</param>
	public WorkState 	Decompression(string szSourcePath, string szTargetPath, HttpWorkEvent evtCallback, string szVersion)
	{
		FileStream zipFile = File.OpenRead (szSourcePath);
		if (zipFile.Length <= 0)
		{
			UnityThreadHelper.Dispatcher.Dispatch( ()=> {
				evtCallback(WorkState.HS_FAILURE, szSourcePath, szTargetPath, 
				            0, 0, 0, szVersion);
			});

			return WorkState.HS_FAILURE;
		}
		
		using (ZipInputStream s = new ZipInputStream(zipFile))
		{
			ZipEntry entry;
			while((entry = s.GetNextEntry()) != null)
			{
				string szDirectoryName = string.Format("{0}/{1}/", szTargetPath, Path.GetDirectoryName(entry.Name));
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
								
								evtCallback(WorkState.HS_DECOMPRESS, 
								         szSourcePath, szFileName, nPosition, nSecSpeed, (int)s.Length, szVersion);
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
		
		return WorkState.HS_SUCCESS;
	}
}















