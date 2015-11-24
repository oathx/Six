using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Xml;

/// <summary>
/// Readonly field.
/// </summary>
public class ReadonlyField 
	: System.Attribute
{

}

/// <summary>
/// Custom field.
/// </summary>
public class CustomField 
	: System.Attribute
{

}

/// <summary>
/// Directory field.
/// </summary>
public class DirectoryField
	: System.Attribute
{

}

/// <summary>
/// Internal field.
/// </summary>
public class InternalField 
	: System.Attribute
{

}

/// <summary>
/// Type helper.
/// </summary>
public class TypeHelper
{
	/// <summary>
	/// Determines if has  pi type.
	/// </summary>
	/// <returns><c>true</c> if has pi type; otherwise, <c>false</c>.</returns>
	/// <param name="pi">Pi.</param>
	/// <param name="type">Type.</param>
	public static bool	Has(PropertyInfo pi, System.Type type)
	{
		object[] aryAtt = pi.GetCustomAttributes (true);
		foreach(object att in aryAtt)
		{
			if (att.GetType() == type)
				return true;
		}

		return false;
	}

	/// <summary>
	/// Determines if has  fi type.
	/// </summary>
	/// <returns><c>true</c> if has fi type; otherwise, <c>false</c>.</returns>
	/// <param name="fi">Fi.</param>
	/// <param name="type">Type.</param>
	public static bool	Has(FieldInfo fi, System.Type type)
	{
		object[] aryAtt = fi.GetCustomAttributes (true);
		foreach(object att in aryAtt)
		{
			if (att.GetType() == type)
				return true;
		}
		
		return false;
	}
}

/// <summary>
/// Xml struct.
/// </summary>
public class XmlStruct
{
	/// <summary>
	/// Initializes a new instance of the <see cref="XmlStruct"/> class.
	/// </summary>
	public XmlStruct()
	{

	}
}

/// <summary>
/// Xml editor window.
/// </summary>
public class XmlEditorWindow<T> : EditorWindow where T : XmlStruct, new()
{
	/// <summary>
	/// The xml struct instance.
	/// </summary>
	public T			target = new T();

	// invalidate return value define
	public enum ResultType {
		Success, 
		Failure,
	}

	// gui layout type
	public enum LayoutType {
		None,
		Readonly,
		Group,
		Scroll,
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="XmlEditorWindow`1"/> class.
	/// </summary>
	public XmlEditorWindow()
	{

	}

	/// <summary>
	/// Raises the GU event.
	/// </summary>
	public virtual void OnGUI()
	{
		if (target != default(T)) {
			Invalidate (target, LayoutType.None);
		}
	}

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	public virtual void OnEnable()
	{

	}

	/// <summary>
	/// Invalidate the specified target and bReadOnly.
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="bReadOnly">If set to <c>true</c> b read only.</param>
	public virtual ResultType Invalidate(object target, LayoutType layout)
	{
		return ResultType.Success;
	}
}





























