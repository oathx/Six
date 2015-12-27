using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Character table.
/// </summary>
public class CharacterTable : MemoryTable<CharacterTable, int, TcpEvent.CharacterInfo>
{
	/// <summary>
	/// Gets the index of the by.
	/// </summary>
	/// <returns>The by index.</returns>
	/// <param name="idx">Index.</param>
	public  TcpEvent.CharacterInfo	GetByIdx(int idx)
	{
		List<TcpEvent.CharacterInfo> list = new List<TcpEvent.CharacterInfo> (m_dTable.Values);
		if (idx < 0 || idx > list.Count)
			throw new System.NullReferenceException ();
		
		return list [idx];
	}
}
