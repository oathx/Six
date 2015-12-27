using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;

public enum HttpReqState
{
	HRS_CONNECTING,
	HRS_SUCCESS,
	HRS_FAILURE,
}

public delegate bool HttpResponseResult(HttpReqState hrs, string text, object args);

/// <summary>
/// Http session.
/// </summary>
public class HttpSession : MonoBehaviourSingleton<HttpSession>
{
	/// <summary>
	/// Raises the post event.
	/// </summary>
	/// <param name="szMothedName">Size mothed name.</param>
	/// <param name="text">Text.</param>
	/// <param name="execute">Execute.</param>
	/// <param name="bBarrierBox">If set to <c>true</c> b barrier box.</param>
	/// <param name="args">Arguments.</param>
	IEnumerator			OnPost(string url, Dictionary<string, string> fields, HttpResponseResult execute, object args)
	{
#if UNITY_EDITOR
		Debug.Log("Send Http Request : " + url + " fields count " + fields.Count);
#endif
		
		WWWForm wf = new WWWForm();
		foreach(KeyValuePair<string, string> it in fields)
		{
			wf.AddField(it.Key, it.Value);
		}
		
		// notity http request
		execute(HttpReqState.HRS_CONNECTING, url, args);
		
		WWW send = new WWW(url, wf);
		yield return send;
		
		if (send.error != null)
		{
#if UNITY_EDITOR
			Debug.Log("Http return json result : " + send.error);
#endif

			execute(HttpReqState.HRS_FAILURE, send.error, args);
		}
		else
		{
#if UNITY_EDITOR
			Debug.Log("Http return json result : " + send.text);
#endif
			execute(HttpReqState.HRS_SUCCESS, send.text, args);
		}
	}
	
	/// <summary>
	/// Requeset the specified szMothedName, fields, execute and args.
	/// </summary>
	/// <param name="szMothedName">Size mothed name.</param>
	/// <param name="fields">Fields.</param>
	/// <param name="execute">Execute.</param>
	/// <param name="args">Arguments.</param>
	public void 		Requeset(string url, Dictionary<string, string> fields,
	                       HttpResponseResult execute, object args)
	{
		StartCoroutine(OnPost(url, fields, execute, args));
	}
}
