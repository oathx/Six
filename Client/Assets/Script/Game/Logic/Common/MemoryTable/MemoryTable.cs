using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Memory table.
/// </summary>
public class MemoryTable<T, K, V> : ScriptableObject where T : MemoryTable<T, K, V> where V : new()
{
	/// <summary>
	/// Instance.
	/// </summary>
	private static readonly T instance = ScriptableObject.CreateInstance<T>();
	
	/// <summary>
	/// Gets the singleton.
	/// </summary>
	/// <returns>The singleton.</returns>
	public static T			GetSingleton()
	{
		return instance;
	}
	
	/// <summary>
	/// The group.
	/// </summary>
	protected Dictionary<K, V> m_dTable = new Dictionary<K, V> ();
	
	/// <summary>
	/// Add the specified nID and data.
	/// </summary>
	/// <param name="nID">N I.</param>
	/// <param name="data">Data.</param>
	public virtual void 	Add(K nID, V data)
	{
		if (!m_dTable.ContainsKey(nID))
		{
			m_dTable.Add(nID, data);
		}
	}
	
	/// <summary>
	/// Query the specified nID.
	/// </summary>
	/// <param name="nID">N I.</param>
	public virtual V		Query(K nID)
	{
		if (!m_dTable.ContainsKey (nID))
			throw new System.NullReferenceException (nID.ToString ());
		
		return m_dTable [nID];
	}
	
	/// <summary>
	/// Remove the specified nID.
	/// </summary>
	/// <param name="nID">N I.</param>
	public virtual void 	Remove(K nID)
	{
		if (m_dTable.ContainsKey (nID))
		{
			m_dTable.Remove(nID);
		}
	}
	
	/// <summary>
	/// Clearup this instance.
	/// </summary>
	public virtual void 	Clearup()
	{
		m_dTable.Clear ();
	}
	
	/// <summary>
	/// Tos the array.
	/// </summary>
	/// <returns>The array.</returns>
	public List<V>			ToArray()
	{
		return new List<V> (m_dTable.Values);
	}
	
	/// <summary>
	/// Empty this instance.
	/// </summary>
	public bool				Empty()
	{
		return m_dTable.Count == 0;
	}
}