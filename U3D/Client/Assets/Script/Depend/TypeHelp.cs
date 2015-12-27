using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using NLua;

/// <summary>
/// Script help.
/// </summary>
public static class TypeHelp
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
}


