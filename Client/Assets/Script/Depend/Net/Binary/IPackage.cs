using System;
using System.Text;
using System.IO;
using KSNET;
using UnityEngine;

/// <summary>
/// Client Message Pakckage.
/// </summary>
public class IPackage
{
	private TNBuffer	 	m_buffer;
	private BinaryReader 	m_reader;
	private int 			m_nLength;

	/// <summary>
	/// Initializes a new instance of the <see cref="IPackage"/> class.
	/// </summary>
	/// <param name="buffer">Buffer.</param>
	public IPackage(TNBuffer buffer)
	{
		m_buffer 	= buffer;
	}

	/// <summary>
	/// Gets the length.
	/// </summary>
	/// <returns>The length.</returns>
	public int		GetLength()
	{
		return m_buffer.buffer.Length;
	}

	/// <summary>
	/// Starts the read.
	/// </summary>
	public void 	StartRead()
	{
		m_reader = m_buffer.BeginReading();
	}

	/// <summary>
	/// Gets the type.
	/// </summary>
	/// <returns>The type.</returns>
	public int 		GetCmdID()
	{
		return m_reader.ReadInt32();
	}

	/// <summary>
	/// Gets the int8.
	/// </summary>
	/// <returns>The int8.</returns>
	public sbyte 	GetInt8()
	{
		return m_reader.ReadSByte();
	}

	/// <summary>
	/// Gets the int16.
	/// </summary>
	/// <returns>The int16.</returns>
	public short 	GetInt16()
	{
		return m_reader.ReadInt16();
	}

	/// <summary>
	/// Gets the int32.
	/// </summary>
	/// <returns>The int32.</returns>
	public int 		GetInt32()
	{
		return m_reader.ReadInt32();
	}
	
	/// <summary>
	/// Gets the float.
	/// </summary>
	/// <returns>The float.</returns>
	public float 	GetFloat()
	{
		return m_reader.ReadSingle();
	}

	/// <summary>
	/// Gets the int64.
	/// </summary>
	/// <returns>The int64.</returns>
	public long 	GetInt64()
	{
		return m_reader.ReadInt64();
	}

	/// <summary>
	/// Gets the double.
	/// </summary>
	/// <returns>The double.</returns>
	public double	GetDouble()
	{
		return m_reader.ReadDouble();
	}

	/// <summary>
	/// Gets the string.
	/// </summary>
	/// <returns>The string.</returns>
	public string 	GetString()
	{
		short nSize = m_reader.ReadInt16();
		if (nSize <= 0)
		{
			return string.Empty;
		}

		byte[] stringByes = m_reader.ReadBytes(nSize);
		return System.Text.Encoding.UTF8.GetString(stringByes);
	}

	/// <summary>
	/// Gets the vector3.
	/// </summary>
	/// <returns>The vector3.</returns>
	public Vector3 	GetVector3()
	{
		return new Vector3 (GetFloat (), GetFloat (), GetFloat ());
	}

	/// <summary>
	/// Gets the bool.
	/// </summary>
	/// <returns><c>true</c>, if bool was gotten, <c>false</c> otherwise.</returns>
	public bool		GetBool()
	{
		return GetInt8 () == 1;
	}

	/// <summary>
	/// Ends the read.
	/// </summary>
	public void 	EndRead()
	{
		if (m_reader != null)
		{
			m_reader = null;
		}
		
		if (m_buffer != null)
		{
			m_buffer.Recycle();
			m_buffer = null;
		}
	}
}



