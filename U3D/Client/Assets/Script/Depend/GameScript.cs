using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using NLua;

/// <summary>
/// Script help.
/// </summary>
public static class ScriptHelp
{
	/// <summary>
	/// getType
	/// </summary>
	/// <param name="classname"></param>
	/// <returns></returns>
	public static System.Type GetType(string classname) 
	{
		Assembly assb = Assembly.GetExecutingAssembly();
		return assb.GetType(classname);
	}
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="obj"></param>
	/// <returns></returns>
	public static Transform[] GetAllChild(GameObject obj) 
	{
		return obj.GetComponentsInChildren<Transform> ();
	}
}

/// <summary>
/// Game script.
/// </summary>
public class GameScript : ScriptableSingleton<GameScript>
{
	public Lua	Global
	{ get; private set; }

	/// <summary>
	/// The execute result.
	/// </summary>
	static System.Object[] executeResult = new object[0];

	/// <summary>
	/// Initializes a new instance of the <see cref="GameScript"/> class.
	/// </summary>
	protected GameScript()
	{		
#if UNITY_EDITOR
		Debug.Log("**************************** lua script start *****************************");
#endif
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	public void 			OnDestroy ()
	{
		Global.Close ();

#if UNITY_EDITOR
		Debug.Log("**************************** lua script close *****************************");
#endif
	}

	/// <summary>
	/// Startup this instance.
	/// </summary>
	public void 			Startup()
	{
		Global = new Lua();
		Global.LoadCLRPackage();
	}

	/// <summary>
	/// Shutdown this instance.
	/// </summary>
	public void 			Shutdown()
	{
		Global.Close ();
	}

	/// <summary>
	/// Registers the global value.
	/// </summary>
	/// <param name="szFullName">Size full name.</param>
	/// <param name="val">Value.</param>
	public void				RegisterGlobalValue(string szFullName, object val)
	{
		Global [szFullName] = val;
	}

	/// <summary>
	/// Dos the string.
	/// </summary>
	/// <param name="text">Text.</param>
	public System.Object[] 	DoString(string text)
	{
		return Global.DoString (text);
	}

	/// <summary>
	/// Dos the file.
	/// </summary>
	/// <returns>The file.</returns>
	/// <param name="filePath">File path.</param>
	public System.Object[] 	DoFile(string filePath)
	{
		return Global.DoFile (filePath);
	}
	
	/// <summary>
	/// Dos the script.
	/// </summary>
	/// <returns>The script.</returns>
	/// <param name="szScriptFileName">Size script file name.</param>
	public System.Object[]	DoScript(string szScriptFileName)
	{
		TextAsset asset = Resources.Load (szScriptFileName, typeof(TextAsset)) as TextAsset;
		if (!asset)
			throw new System.NullReferenceException (szScriptFileName);

		return DoString (asset.text);
	}

	/// <summary>
	/// Registers the function.
	/// </summary>
	/// <param name="szName">Size name.</param>
	/// <param name="mothod">Mothod.</param>
	public void 			RegisterFunction(string szName, System.Reflection.MethodBase mothod)
	{
		Global.RegisterFunction (szName, null, mothod);
	}

	/// <summary>
	/// Registers the function.
	/// </summary>
	/// <param name="szName">Size name.</param>
	/// <param name="target">Target.</param>
	/// <param name="mothod">Mothod.</param>
	public void 			RegisterFunction(string szName, object target, System.Reflection.MethodBase mothod)
	{
		Global.RegisterFunction (szName, target, mothod);
	}

	/// <summary>
	/// Dump the specified fullpath.
	/// </summary>
	/// <param name="fullpath">Fullpath.</param>
	public void				Dump(string fullpath)
	{
		LuaTable table = Global.GetTable (fullpath);
		foreach(KeyValuePair<object, object> it in Global.GetTableDict(table))
		{
			Debug.Log("key: " + it.Key + " value : " + it.Key);
		}
	}

	/// <summary>
	/// Dump the specified table.
	/// </summary>
	/// <param name="table">Table.</param>
	public void				Dump(LuaTable table)
	{
		foreach(KeyValuePair<object, object> it in Global.GetTableDict(table))
		{
			Debug.Log("key: " + it.Key + " value : " + it.Key);
		}
	}

	/// <summary>
	/// Call the specified lf.
	/// </summary>
	/// <param name="lf">Lf.</param>
	public System.Object[]	Call(LuaFunction lf)
	{
		try{
			if (lf != null)
				executeResult = lf.Call ();
		}
		catch(NLua.Exceptions.LuaException e)
		{
			Debug.LogError(GetFormatExceptionString(e));
		}
		
		return executeResult;
	}

	/// <summary>
	/// Call the specified lf and args.
	/// </summary>
	/// <param name="lf">Lf.</param>
	/// <param name="args">Arguments.</param>
	public System.Object[]	Call(LuaFunction lf, params System.Object[] args)
	{
		try{
			if (lf != null)
				executeResult = lf.Call (args);
		}
		catch(NLua.Exceptions.LuaException e)
		{
			Debug.LogError(GetFormatExceptionString(e));
		}
		
		return executeResult;
	}
	
	/// <summary>
	/// Call the specified function and args.
	/// </summary>
	/// <param name="function">Function.</param>
	/// <param name="args">Arguments.</param>
	public System.Object[] 	Call(string function, params System.Object[] args)
	{
		try{
			LuaFunction lf = Global.GetFunction (function);
			if (lf != null)
				executeResult = lf.Call (args);
		}
		catch(NLua.Exceptions.LuaException e)
		{
			Debug.LogError(GetFormatExceptionString(e));
		}

		return executeResult;
	}

	/// <summary>
	/// Call the specified function.
	/// </summary>
	/// <param name="function">Function.</param>
	public System.Object[] Call(string function)
	{
		try{
			LuaFunction lf = Global.GetFunction (function);
			if (lf != null)
				executeResult = lf.Call ();
		}
		catch(NLua.Exceptions.LuaException e)
		{
			Debug.LogError(GetFormatExceptionString(e));
		}

		return executeResult;
	}

	/// <summary>
	/// Call the specified table and function.
	/// </summary>
	/// <param name="table">Table.</param>
	/// <param name="function">Function.</param>
	public System.Object[] Call(LuaTable table, string function)
	{
		try{
			LuaFunction lf = NLua.Method.LuaClassHelper.GetTableFunction (table, function);
			if (lf != null)
				executeResult = lf.Call ();
		}
		catch(NLua.Exceptions.LuaException e)
		{
			Debug.LogError(GetFormatExceptionString(e));
		}
		
		return executeResult;
	}

	/// <summary>
	/// Call the specified table, function and args.
	/// </summary>
	/// <param name="table">Table.</param>
	/// <param name="function">Function.</param>
	/// <param name="args">Arguments.</param>
	public System.Object[] 	Call(LuaTable table, string function, params System.Object[] args)
	{
		try{
			LuaFunction lf = NLua.Method.LuaClassHelper.GetTableFunction (table, function);
			if (lf != null)
				executeResult = lf.Call (args);
		}
		catch(NLua.Exceptions.LuaException e)
		{
			Debug.LogError(GetFormatExceptionString(e));
		}
		
		return executeResult;
	}

	/// <summary>
	/// Gets the table.
	/// </summary>
	/// <returns>The table.</returns>
	/// <param name="szTableName">Size table name.</param>
	public LuaTable			GetTable(string szTableName)
	{
		return Global.GetTable (szTableName);
	}

	/// <summary>
	/// Call the specified tableName and function.
	/// </summary>
	/// <param name="tableName">Table name.</param>
	/// <param name="function">Function.</param>
	public System.Object[]	Call(string tableName, string function)
	{
		return Call(Global.GetTable(tableName), function);
	}

	/// <summary>
	/// Gets the format exception string.
	/// </summary>
	/// <returns>The format exception string.</returns>
	/// <param name="e">E.</param>
	public string 	GetFormatExceptionString(NLua.Exceptions.LuaException e) 
	{
		string source = (string.IsNullOrEmpty(e.Source)) ? "<no source>" : e.Source.Substring(0, e.Source.Length - 2);
		return string.Format("{0}\nLua {1} (at {2})", e.Message, e.StackTrace, source);
	}
}
