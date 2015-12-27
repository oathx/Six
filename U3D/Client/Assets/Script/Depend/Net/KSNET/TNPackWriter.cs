using System.Collections;
using System.IO;
using UnityEngine;

namespace KSNET {

	/// <summary>
	/// 发送数据包的处理
	/// </summary>
	public class TNPackWriter {
		
		/// <summary>
		/// 对客户端传过来的数据类型进行封包和解包， object支持unity特定的类型: Vector2, Vector3
		/// </summary>
		/// <param name="buffer">Buffer.</param>
		/// <param name="objs">Objects.</param>
		public void encode(TNBuffer buffer, params object[] objs)
		{
			if (objs == null || objs.Length == 0) return;
		
			BinaryWriter bw = buffer.BeginWriting(true);
			for (int b = 0, bmax = objs.Length; b < bmax; ++b)
			{
				object obj = objs[b];
				if (obj != null && !WriteObject(bw, obj))
				{
					Debug.LogError("Unable to write type " + obj.GetType());
				}
			}
		}
		
		/// <summary>
		/// 根据特定的对象类型进行封包
		/// </summary>
		/// <returns><c>true</c>, if object was writed, <c>false</c> otherwise.</returns>
		/// <param name="bw">Bw.</param>
		/// <param name="obj">Object.</param>
		static public bool WriteObject (BinaryWriter bw, object obj)
		{
			System.Type type = obj.GetType();

			if (type == typeof(sbyte))
			{
				bw.Write((sbyte)obj);
			} else if (type == typeof(bool))
			{
				bw.Write((bool)obj);
			}
			else if (type == typeof(byte))
			{
				bw.Write((byte)obj);
			}
			else if (type == typeof(short))
			{
				bw.Write((short)obj);
			}
			else if (type == typeof(ushort))
			{
				bw.Write((ushort)obj);
			}
			else if (type == typeof(int))
			{
				bw.Write((int)obj);
			}
			else if (type == typeof(uint))
			{
				bw.Write((uint)obj);
			}
			else if (type == typeof(float))
			{
				bw.Write((float)obj);
			}
			else if (type == typeof(long))
			{
				bw.Write((long)obj);
			}
			else if (type == typeof(string))
			{
				string writeStr = (string)obj;
				if (string.IsNullOrEmpty(writeStr))
				{
					writeStr = " ";
				}

				byte[] stringBytes = System.Text.Encoding.UTF8.GetBytes(writeStr);
				int size = stringBytes.GetLength(0);

				//包大小限制
				if (size > 32764)
					return false;

				//写入字符串长度
				bw.Write((short)size);
				
				//写入字符串
				bw.Write(stringBytes);
			}
			else if (type == typeof(Vector2))
			{
				Write(bw, (Vector2)obj);
			}
			else if (type == typeof(Vector3))
			{
				Write(bw, (Vector3)obj);
			}
			else if (type == typeof(Vector4))
			{
				Write(bw, (Vector4)obj);
			}
			else if (type == typeof(Quaternion))
			{
				Write(bw, (Quaternion)obj);
			}
			else if (type == typeof(Color32))
			{
				Write(bw, (Color32)obj);
			}
			else if (type == typeof(Color))
			{
				Write(bw, (Color)obj);
			}
			else if (type == typeof(bool[]))
			{
				bool[] arr = (bool[])obj;
				bw.Write(arr.Length);
				for (int i = 0, imax = arr.Length; i < imax; ++i) bw.Write(arr[i]);
			}
			else if (type == typeof(byte[]))
			{
				byte[] arr = (byte[])obj;
				bw.Write(arr.Length);
				bw.Write(arr);
			}
			else if (type == typeof(ushort[]))
			{
				ushort[] arr = (ushort[])obj;
				bw.Write(arr.Length);
				for (int i = 0, imax = arr.Length; i < imax; ++i) bw.Write(arr[i]);
			}
			else if (type == typeof(int[]))
			{
				int[] arr = (int[])obj;
				bw.Write(arr.Length);
				for (int i = 0, imax = arr.Length; i < imax; ++i) bw.Write(arr[i]);
			}
			else if (type == typeof(uint[]))
			{
				uint[] arr = (uint[])obj;
				bw.Write(arr.Length);
				for (int i = 0, imax = arr.Length; i < imax; ++i) bw.Write(arr[i]);
			}
			else if (type == typeof(float[]))
			{
				float[] arr = (float[])obj;
				bw.Write(arr.Length);
				for (int i = 0, imax = arr.Length; i < imax; ++i) bw.Write(arr[i]);
			}
			else if (type == typeof(string[]))
			{
				string[] arr = (string[])obj;
				bw.Write(arr.Length);
				for (int i = 0, imax = arr.Length; i < imax; ++i) bw.Write(arr[i]);
			}
			else if (type == typeof(ArrayList))
			{
				Write(bw, (ArrayList)obj);
			}
			else
			{
				bw.Write('0');
				return false;
			}
			return true;
		}
		
		
		/// <summary>
		/// Write a value to the stream.
		/// </summary>
		
		static public void Write (BinaryWriter writer, Vector2 v)
		{
			writer.Write(v.x);
			writer.Write(v.y);
		}
		
		/// <summary>
		/// Write a value to the stream.
		/// </summary>
		
		static public void Write (BinaryWriter writer, Vector3 v)
		{
			writer.Write(v.x);
			writer.Write(v.y);
			writer.Write(v.z);
		}
		
		/// <summary>
		/// Write a value to the stream.
		/// </summary>
		
		static public void Write (BinaryWriter writer, Vector4 v)
		{
			writer.Write(v.x);
			writer.Write(v.y);
			writer.Write(v.z);
			writer.Write(v.w);
		}
		
		/// <summary>
		/// Write a value to the stream.
		/// </summary>
		
		static public void Write (BinaryWriter writer, Quaternion q)
		{
			writer.Write(q.x);
			writer.Write(q.y);
			writer.Write(q.z);
			writer.Write(q.w);
		}
		
		/// <summary>
		/// Write a value to the stream.
		/// </summary>
		
		static public void Write (BinaryWriter writer, Color32 c)
		{
			writer.Write(c.r);
			writer.Write(c.g);
			writer.Write(c.b);
			writer.Write(c.a);
		}
		
		/// <summary>
		/// Write a value to the stream.
		/// </summary>
		
		static public void Write (BinaryWriter writer, Color c)
		{
			writer.Write(c.r);
			writer.Write(c.g);
			writer.Write(c.b);
			writer.Write(c.a);
		}


		static public void Write(BinaryWriter bw, ArrayList list)
		{
			bw.Write((short)list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				object[] objs = (object[])list[i];
				for (int j = 0; j < objs.Length; j++)
				{
					WriteObject(bw, objs[j]);
				}
			}
		}
	}
}
