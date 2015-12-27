using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;

/// <summary>
/// Simple singleton.
/// </summary>
public class SimpleSingleton<T> where T : class, new() 
{
	/// <summary>
	/// The instance.
	/// </summary>
	protected static readonly T instance = new T();

	/// <summary>
	/// Gets the singleton.
	/// </summary>
	/// <returns>The singleton.</returns>
	public static T GetSingleton()
	{
		return instance;
	}
}

/// <summary>
/// Scene support.
/// </summary>
public class ScriptableSingleton<T> : ScriptableObject where T : ScriptableSingleton<T>
{
	/// <summary>
	/// The instance.
	/// </summary>
	protected static readonly T instance = ScriptableObject.CreateInstance<T>();
	
	/// <summary>
	/// Gets the singleton.
	/// </summary>
	/// <returns>The singleton.</returns>
	public static T GetSingleton()
	{
		return instance;
	}
}

/// <summary>
/// Mono behaviour singleton.
/// </summary>
public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
{
	/// <summary>
	/// The instance.
	/// </summary>
	public static T	instance;

	/// <summary>
	/// Gets the singleton.
	/// </summary>
	/// <returns>The singleton.</returns>
	public static T GetSingleton()
	{
		if (!instance)
		{
			GameObject singleton = new GameObject(typeof(T).Name);
			if (!singleton)
				throw new System.NullReferenceException();

			instance = singleton.AddComponent<T>();

			GameObject.DontDestroyOnLoad(singleton);
		}

		return instance;
	}
}
