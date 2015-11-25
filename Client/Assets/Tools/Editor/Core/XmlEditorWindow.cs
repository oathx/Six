using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Xml;
using System;

public enum KeyWord
{
	Name,
	Position,
	EulerAngle,
	Scale,
	Center,
	Radius,
	Size,
	Trigger,
	ToggleGroup,
	Box,
	Root,
	Data,
	Npc,
	Member,
	Item,
	Select,
	EditorOnly,
	Save,
	Export,
	Import,
	Xml,
	Create,
	Asset,
	PropertyName,
	Unity,
	Pass,
}

public enum SearchFileType
{
	png,
	prefab,
	unity,
	fbx,
	mat,
	jpg,
	tag,
	xml,
	bytes,
	pass,
	unity3d,
}

public class ReadonlyField 
	: System.Attribute
{

}

public class DirectoryField 
	: System.Attribute
{

}

public class InternalField 
	: System.Attribute
{

}

public class CustomField 
	: System.Attribute
{

}

/// <summary>
/// Base xml struct.
/// </summary>
public class XmlStruct
{
	/// <param name="xml">Xml.</param>
	public static implicit operator bool(XmlStruct xml)
	{return xml != default(XmlStruct);}

	/// <summary>
	/// Vs the s2.
	/// </summary>
	/// <returns>The s2.</returns>
	/// <param name="v">V.</param>
	public static string VS2(Vector2 v)
	{
		return string.Format ("{0},{1}", v.x, v.y);
	}
	
	/// <summary>
	/// Ss the v2.
	/// </summary>
	/// <returns>The v2.</returns>
	/// <param name="v">V.</param>
	public static Vector2 SV2(string v)
	{
		if (!string.IsNullOrEmpty(v))
		{
			string[] arySplit = v.Split(',');
			if (arySplit.Length >= 2)
				return new Vector2 (float.Parse (arySplit [0]), float.Parse (arySplit [1]));
		}
		
		return Vector2.zero;
	}
	
	/// <summary>
	/// Vs the s3.
	/// </summary>
	/// <returns>The s3.</returns>
	/// <param name="v">V.</param>
	public static string VS3(Vector3 v)
	{
		return string.Format ("{0},{1},{2}", v.x, v.y, v.z);
	}

	/// <summary>
	/// Vs the s3.
	/// </summary>
	/// <returns>The s3.</returns>
	/// <param name="format">Format.</param>
	/// <param name="v">V.</param>
	public static string VS3(string format, Vector3 v)
	{
		return string.Format (format, v.x, v.y, v.z);
	}
	
	/// <summary>
	/// Ss the v3.
	/// </summary>
	/// <returns>The v3.</returns>
	/// <param name="v">V.</param>
	public static Vector3 SV3(string v)
	{
		if (!string.IsNullOrEmpty(v))
		{
			string[] arySplit = v.Split(',');
			if (arySplit.Length >= 3)
				return new Vector3 (float.Parse (arySplit [0]), float.Parse (arySplit [1]), float.Parse (arySplit [2]));
		}
		
		return Vector3.zero;
	}
	
	/// <summary>
	/// Vs the s4.
	/// </summary>
	/// <returns>The s4.</returns>
	/// <param name="v">V.</param>
	public static string VS4(Vector4 v)
	{
		return string.Format ("{0},{1},{2}, {3}", v.x, v.y, v.z, v.w);
	}
	
	/// <summary>
	/// Ss the v4.
	/// </summary>
	/// <returns>The v4.</returns>
	/// <param name="v">V.</param>
	public static Vector4 SV4(string v)
	{
		if (!string.IsNullOrEmpty(v))
		{
			string[] arySplit = v.Split(',');
			if (arySplit.Length >= 4)
				return new Vector4 (float.Parse (arySplit [0]), float.Parse (arySplit [1]), float.Parse (arySplit [2]), float.Parse (arySplit [3]));
		}
		
		return Vector4.zero;
	}
	
	/// <summary>
	/// VC the specified v.
	/// </summary>
	/// <param name="v">V.</param>
	public static string CCS(Color v)
	{
		return string.Format ("{0},{1},{2}, {3}", v.r, v.g, v.b, v.a);
	}
	
	/// <summary>
	/// SC the specified v.
	/// </summary>
	/// <param name="v">V.</param>
	public static Color SCC(string v)
	{
		if (!string.IsNullOrEmpty(v))
		{
			string[] arySplit = v.Split(',');
			if (arySplit.Length >= 4)
				return new Color (float.Parse (arySplit [0]), float.Parse (arySplit [1]), float.Parse (arySplit [2]), float.Parse (arySplit [3]));
		}
		
		return Color.white;
	}

	/// <summary>
	/// Readonly the specified py.
	/// </summary>
	/// <param name="py">Py.</param>
	public virtual bool		HasAttribute(PropertyInfo py, System.Type typeField)
	{
		object[] aryAttribute = py.GetCustomAttributes (true);
		foreach(object a in aryAttribute)
		{
			if (a.GetType() == typeField)
				return true;
		}
		
		return false;
	}

	/// <summary>
	/// Determines whether this instance has attribute the specified fi typeField.
	/// </summary>
	/// <returns><c>true</c> if this instance has attribute the specified fi typeField; otherwise, <c>false</c>.</returns>
	/// <param name="fi">Fi.</param>
	/// <param name="typeField">Type field.</param>
	public virtual bool		HasAttribute(FieldInfo fi, System.Type typeField)
	{
		object[] aryAttribute = fi.GetCustomAttributes (true);
		foreach(object a in aryAttribute)
		{
			if (a.GetType() == typeField)
				return true;
		}
		
		return false;
	}

	/// <summary>
	/// Clearup this instance.
	/// </summary>
	public virtual void 	Clearup()
	{

	}

	/// <summary>
	/// Save the specified szPath.
	/// </summary>
	/// <param name="szPath">Size path.</param>
	public virtual void 	Save(string szPath)
	{
		XmlDocument doc = new XmlDocument();
		
		// create xml define
		XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
		doc.AppendChild (declaration);
		
		XmlElement root = doc.CreateElement (
			KeyWord.Root.ToString ()
			);
		doc.AppendChild (root);

		OnWrite(doc, root, string.Empty, this);

		doc.Save (szPath);

	}

	/// <summary>
	/// Load the specified szPath.
	/// </summary>
	/// <param name="szPath">Size path.</param>
	public virtual void 	Load(string szPath)
	{
		XmlDocument doc = new XmlDocument();
		doc.Load (szPath);

		XmlNodeList aryRoot = doc.GetElementsByTagName(KeyWord.Root.ToString());
		foreach(XmlElement root in aryRoot)
		{
			foreach(XmlElement child in root.ChildNodes)
			{
				OnRead(doc, child, this);
			}
		}
	}

	/// <summary>
	/// Gets the name of the type by.
	/// </summary>
	/// <returns>The type by name.</returns>
	/// <param name="szName">Size name.</param>
	public virtual System.Type	QueryTypeName(string szName)
	{
		Assembly assembly = Assembly.GetExecutingAssembly();
		return assembly.GetType(szName);
	}

	/// <summary>
	/// Gets the property.
	/// </summary>
	/// <returns>The property.</returns>
	/// <param name="type">Type.</param>
	public virtual PropertyInfo GetProperty(System.Type type, string szPropertyName)
	{
		PropertyInfo[] aryProperty = type.GetProperties();
		foreach(PropertyInfo p in aryProperty)
		{
			if (p.Name == szPropertyName)
				return p;
		}

		return default(PropertyInfo);
	}

	/// <summary>
	/// Gets the field.
	/// </summary>
	/// <returns>The field.</returns>
	/// <param name="type">Type.</param>
	/// <param name="szName">Size name.</param>
	public virtual FieldInfo GetField(System.Type type, string szName)
	{
		FieldInfo[] aryField = type.GetFields();
		foreach(FieldInfo f in aryField)
		{
			if (f.Name == szName)
				return f;
		}
		
		return default(FieldInfo);
	}

	/// <summary>
	/// Raises the custom write event.
	/// </summary>
	/// <param name="element">Element.</param>
	/// <param name="target">Target.</param>
	public virtual bool 	OnWrite(XmlDocument doc, XmlElement element, string szName, object target)
	{
		if (target == default(object))
			return false;

		System.Type targetType = target.GetType();
		
		// create property element
		XmlElement typeElement = doc.CreateElement(
			string.IsNullOrEmpty(szName) ? targetType.Name : szName
			);

		element.AppendChild(typeElement);
	
		// write all property
		PropertyInfo[] aryProperty 	= targetType.GetProperties(
			BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly
			);
		foreach(PropertyInfo pi in aryProperty)
		{
			if (!HasAttribute(pi, typeof(InternalField)))
			{
				object result = pi.GetValue(target, new object[]{});
				if (pi.PropertyType.IsClass)
				{
					if (result is string)
					{
						typeElement.SetAttribute(
							pi.Name, result == default(object) ? string.Empty : result.ToString()
							);
					}
					else if (result is GameObject)
					{
						OnWriteGameObject(doc, typeElement, pi.Name, (GameObject)result);
					}
					else
					{
						if (target != default(object))
						{
							OnWrite(doc, typeElement, pi.Name, result);
						}
					}
				}
				else if (pi.PropertyType.IsEnum)
				{
					typeElement.SetAttribute(
						pi.Name, ((int)result).ToString()
						);
				}
				else if (pi.PropertyType == typeof(Vector2))
				{
					typeElement.SetAttribute(
						pi.Name, VS2((Vector2)result)
						);
				}
				else if (pi.PropertyType == typeof(Vector3))
				{
					typeElement.SetAttribute(
						pi.Name, VS3((Vector3)result)
						);
				}
				else if (pi.PropertyType == typeof(Vector4))
				{
					typeElement.SetAttribute(
						pi.Name, VS4((Vector3)result)
						);
				}
				else if (pi.PropertyType == typeof(Color))
				{
					typeElement.SetAttribute(
						pi.Name, CCS((Color)result)
						);
				}
				else
				{
					typeElement.SetAttribute(
						pi.Name, result.ToString()
						);
				}
			}
		}

		return OnWriteField(doc, typeElement, target);
	}

	/// <summary>
	/// Raises the write field event.
	/// </summary>
	/// <param name="doc">Document.</param>
	/// <param name="element">Element.</param>
	/// <param name="target">Target.</param>
	public virtual bool OnWriteField(XmlDocument doc, XmlElement element, object target)
	{
		if (target == default(object))
			return false;

		FieldInfo[] aryField 	= target.GetType().GetFields(
			BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly
			);
		foreach(FieldInfo field in aryField)
		{
			if (!HasAttribute(field, typeof(InternalField)))
			{
				object result = field.GetValue(target);
				if (result != default(object))
				{
					if (result is IList)
					{
						XmlElement xmlField = doc.CreateElement(
							field.Name
							);
						element.AppendChild(xmlField);
						
						foreach(object o in (IList)result)
						{
							OnWrite(doc, xmlField, string.Empty, o);
						}
					}
				}
			}
		}

		return true;
	}

	/// <summary>
	/// Raises the write game object event.
	/// </summary>
	/// <param name="doc">Document.</param>
	/// <param name="element">Element.</param>
	/// <param name="target">Target.</param>
	public virtual bool OnWriteGameObject(XmlDocument doc, XmlElement element, string szPropertyName, GameObject target)
	{
		if (!target) 
			return false;

		XmlElement g = doc.CreateElement(
			szPropertyName
			);
		element.AppendChild(g);

		// readly all write property
		Dictionary<string, string>
			dict = new Dictionary<string, string>();

		dict.Add(KeyWord.Name.ToString(), 		target.name);
		dict.Add(KeyWord.Position.ToString(), 	VS3(target.transform.position));
		dict.Add(KeyWord.EulerAngle.ToString(), VS3(target.transform.eulerAngles));
		dict.Add(KeyWord.Scale.ToString(), 		VS3(target.transform.localScale));

		foreach(KeyValuePair<string, string> it in dict)
		{
			g.SetAttribute(it.Key, it.Value);
		}

		SphereCollider sphere = target.GetComponent<SphereCollider>();
		if (sphere)
		{
			XmlElement c = doc.CreateElement(
				typeof(SphereCollider).Name
				);
			g.AppendChild(c);

			c.SetAttribute(KeyWord.Center.ToString(), VS3(sphere.center));
			c.SetAttribute(KeyWord.Radius.ToString(), sphere.radius.ToString());
		}

		BoxCollider box = target.GetComponent<BoxCollider>();
		if (box)
		{
			XmlElement b = doc.CreateElement(
				typeof(BoxCollider).Name
				);
			g.AppendChild(b);

			b.SetAttribute(KeyWord.Center.ToString(), VS3(box.center));
			b.SetAttribute(KeyWord.Size.ToString(), VS3(box.size));
		}

		return true;
	}

	
	/// <summary>
	/// Raises the read property event.
	/// </summary>
	/// <param name="doc">Document.</param>
	/// <param name="element">Element.</param>
	/// <param name="target">Target.</param>
	public virtual bool		OnRead(XmlDocument doc, XmlElement element, object target)
	{
		if (target == default(object))
			return false;

		System.Type targetType = target.GetType();
		
		foreach(XmlAttribute att in element.Attributes)
		{
			PropertyInfo pi = targetType.GetProperty(att.Name);
			if (pi == default(PropertyInfo))
				continue;

			if (!HasAttribute(pi, typeof(InternalField)))
			{
				object result = pi.GetValue(target, new object[]{});
				if (pi.PropertyType == typeof(bool))
				{
					pi.SetValue(
						target, bool.Parse(att.Value), new object[]{}
					);
				}
				else if (pi.PropertyType == typeof(int))
				{
					pi.SetValue(
						target, int.Parse(att.Value), new object[]{}
					);
				}
				else if (pi.PropertyType == typeof(float))
				{
					pi.SetValue(
						target, float.Parse(att.Value), new object[]{}
					);
				}
				else if (pi.PropertyType == typeof(double))
				{
					pi.SetValue(
						target, double.Parse(att.Value), new object[]{}
					);
				}
				else if (pi.PropertyType == typeof(long))
				{
					pi.SetValue(
						target, long.Parse(att.Value), new object[]{}
					);
				}
				else if (pi.PropertyType == typeof(byte))
				{
					pi.SetValue(
						target, byte.Parse(att.Value), new object[]{}
					);
				}
				else if (pi.PropertyType == typeof(string))
				{
					pi.SetValue(
						target, att.Value, new object[]{}
					);
				}
				else if (pi.PropertyType == typeof(Vector2))
				{
					pi.SetValue(
						target, SV2(att.Value), new object[]{}
					);
				}
				else if (pi.PropertyType == typeof(Vector3))
				{
					pi.SetValue(
						target, SV3(att.Value), new object[]{}
					);
				}
				else if (pi.PropertyType == typeof(Vector4))
				{
					pi.SetValue(
						target, SV4(att.Value), new object[]{}
					);
				}
				else if (pi.PropertyType == typeof(Color))
				{
					pi.SetValue(
						target, SCC(att.Value), new object[]{}
					);
				}
				else if (pi.PropertyType == typeof(GameObject))
				{
					OnReadGameObject(doc, element, pi, target);
				}
				else if (pi.PropertyType.IsEnum)
				{
					int nResult = int.Parse(att.Value);
					pi.SetValue(target, nResult, new object[]{});
				}
			}
		}

		foreach(XmlElement child in element.ChildNodes)
		{
			PropertyInfo p = GetProperty(targetType, child.Name);
			if (p != default(PropertyInfo))
			{
				object result = p.GetValue(target, new object[]{});
				if (result == default(object))
				{
					if (p.PropertyType == typeof(GameObject))
					{
						OnReadGameObject(doc, child, p, target);
						continue;
					}
					else
					{
						result = System.Activator.CreateInstance(p.PropertyType);
						p.SetValue(
							target, result, new object[]{}
						);
					}
				}

				OnRead(doc, child, result);
			}
			else
			{
				OnReadField(doc, child, target);
			}

		}

		return true;
	}

	/// <summary>
	/// Raises the custom read event.
	/// </summary>
	/// <param name="element">Element.</param>
	/// <param name="target">Target.</param>
	public virtual bool 	OnReadField(XmlDocument doc, XmlElement element, object target)
	{
		FieldInfo f = GetField(target.GetType(), element.Name);
		if (f == default(FieldInfo))
			return false;

		object result = f.GetValue(target);
		if (result is IList)
		{
			IList aryList = (IList)result;
			aryList.Clear();
			
			foreach(XmlElement child in element.ChildNodes)
			{
				System.Type instanceType = QueryTypeName(child.Name);
				if (instanceType == default(System.Type))
					throw new System.NullReferenceException(child.Name);

				object instance = System.Activator.CreateInstance(instanceType);
				if (instance != default(object))
				{
					aryList.Add(instance);
					
					// read the instance property
					OnRead(
						doc, child, instance
						);
				}
			}
		}

		return true;
	}

	/// <summary>
	/// Raises the read game object event.
	/// </summary>
	/// <param name="doc">Document.</param>
	/// <param name="element">Element.</param>
	/// <param name="target">Target.</param>
	public virtual void 	OnReadGameObject(XmlDocument doc, XmlElement element, PropertyInfo p, object target)
	{
		string	szName		= element.GetAttribute(KeyWord.Name.ToString());

		// read base property
		Vector3 vPosition 	= SV3(element.GetAttribute(KeyWord.Position.ToString()));
		Vector3 vAngle 		= SV3(element.GetAttribute(KeyWord.EulerAngle.ToString()));
		Vector3 vScale 		= SV3(element.GetAttribute(KeyWord.Scale.ToString()));

		GameObject instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
		if (!instance)
			throw new System.NullReferenceException(szName);

		string szAssetPath = GetAssetPath(szName);
		if (!string.IsNullOrEmpty(szAssetPath))
		{
			GameObject resource	= AssetDatabase.LoadAssetAtPath<GameObject>(szAssetPath);
			if (resource)
			{
				if (instance)
					GameObject.DestroyImmediate(instance);

				instance = GameObject.Instantiate(resource) as GameObject;
			}
		}
	
		instance.name					= szName;
		instance.transform.position 	= vPosition;
		instance.transform.eulerAngles	= vAngle;
		instance.transform.localScale	= vScale;

		foreach(XmlElement collider in element.ChildNodes)
		{
			if (collider.Name == typeof(SphereCollider).Name)
			{
				Vector3 vCenter = SV3(collider.GetAttribute(KeyWord.Center.ToString()));
				float	fRadius = float.Parse(
					collider.GetAttribute(KeyWord.Radius.ToString())
					);
	
				SphereCollider sphere = instance.AddComponent<SphereCollider>();
				if (sphere)
				{
					sphere.center = vCenter;
					sphere.radius = fRadius;
				}
			}
			else if (collider.Name == typeof(BoxCollider).Name)
			{
				Vector3 vCenter = SV3(collider.GetAttribute(KeyWord.Center.ToString()));
				Vector3	vSize 	= SV3(collider.GetAttribute(KeyWord.Size.ToString()));
				
				BoxCollider box = instance.AddComponent<BoxCollider>();
				if (box)
				{
					box.center 	= vCenter;
					box.size	= vSize;
				}
			}
		}

		p.SetValue(
			target, instance, new object[]{}
		);
	}

	/// <summary>
	/// Gets the asset path.
	/// </summary>
	/// <returns>The asset path.</returns>
	/// <param name="szName">Size name.</param>
	public virtual string	GetAssetPath(string szName)
	{
		// Priority search prefab file
		string pattern = string.Format("*{0}*.{1}", 
		                               szName, SearchFileType.prefab.ToString().ToLower());
		
		string[] aryFile = System.IO.Directory.GetFiles(Application.dataPath, pattern, SearchOption.AllDirectories);
		if (aryFile.Length <= 0)
		{
			// serach fbx file
			pattern = string.Format("*{0}*.{1}", 
			                        szName, SearchFileType.fbx.ToString().ToLower());
			
			aryFile = System.IO.Directory.GetFiles(
				Application.dataPath, pattern, SearchOption.AllDirectories
				);
		}

		foreach(string file in aryFile)
		{
			return file.Substring(Application.dataPath.Length - 6);
		}

		return string.Empty;
	}
}

/// <summary>
/// I custom editor.
/// </summary>
public class XmlEditorWindow<T> : EditorWindow where T : XmlStruct, new()
{
	public T				Instance = new T();
	public Vector2			Scorll = Vector2.zero;

	/// <summary>
	/// Raises the inspector GU event.
	/// </summary>
	public virtual void 	OnGUI()
	{
		if (!Instance)
		{
			Instance = new T();
		}
		else
		{
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);
			GUILayout.Label(
				typeof(T).Name, EditorStyles.boldLabel
				);
			OnCustomGUIPrev();
			EditorGUILayout.EndVertical();

			Scorll = EditorGUILayout.BeginScrollView (Scorll);
			Invalidate(Instance, Instance.GetType(), false);
			EditorGUILayout.EndScrollView ();

			OnCustomGUIBack();
		}
	}
	
	/// <summary>
	/// Raises the custom field event.
	/// </summary>
	/// <param name="target">Target.</param>
	public virtual void 	OnCustomField(object target, PropertyInfo pi)
	{

	}

	/// <summary>
	/// Raises the inspector update event.
	/// </summary>
	public virtual void 	OnInspectorUpdate()
	{
		Repaint();
	}

	/// <summary>
	/// Raises the hierarchy change event.
	/// </summary>
	public virtual void 	OnHierarchyChange()
	{

	}

	/// <summary>
	/// Raises the custom GUI previous event.
	/// </summary>
	public virtual void 	OnCustomGUIPrev()
	{
		
	}

	/// <summary>
	/// Raises the custom GUI back event.
	/// </summary>
	public virtual void 	OnCustomGUIBack()
	{

	}

	/// <summary>
	/// Readonly the specified py.
	/// </summary>
	/// <param name="py">Py.</param>
	public virtual bool		HasAttribute(PropertyInfo py, System.Type typeField)
	{
		object[] aryAttribute = py.GetCustomAttributes (true);
		foreach(object a in aryAttribute)
		{
			if (a.GetType() == typeField)
				return true;
		}
		
		return false;
	}

	/// <summary>
	/// Determines whether this instance has attribute the specified fi typeField.
	/// </summary>
	/// <returns><c>true</c> if this instance has attribute the specified fi typeField; otherwise, <c>false</c>.</returns>
	/// <param name="fi">Fi.</param>
	/// <param name="typeField">Type field.</param>
	public virtual bool		HasAttribute(FieldInfo fi, System.Type typeField)
	{
		object[] aryAttribute = fi.GetCustomAttributes (true);
		foreach(object a in aryAttribute)
		{
			if (a.GetType() == typeField)
				return true;
		}
		
		return false;
	}

	/// <summary>
	/// Gets the property.
	/// </summary>
	/// <returns>The property.</returns>
	/// <param name="type">Type.</param>
	/// <param name="szPropertyName">Size property name.</param>
	public PropertyInfo 	GetProperty(System.Type type, string szPropertyName)
	{
		PropertyInfo[] aryProperty = type.GetProperties();
		foreach(PropertyInfo p in aryProperty)
		{
			if (p.Name == szPropertyName)
				return p;
		}
		
		return default(PropertyInfo);
	}

	/// <summary>
	/// Invalidate the specified target and type.
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="type">Type.</param>
	public virtual void		Invalidate(object target, System.Type type, bool bReadOnly)
	{
		PropertyInfo[] aryProperty = type.GetProperties (BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
		foreach(PropertyInfo py in aryProperty)
		{
			if (bReadOnly)
			{
				object value = py.GetValue(target, new object[]{});
				if (value != default(object))
				{
					EditorGUILayout.LabelField(py.Name, value.ToString());
				}
			}
			else
			{
				if (HasAttribute(py, typeof(CustomField)))
				{
					OnCustomField(target, py);
				}
				else
				{
					if (!HasAttribute(py, typeof(InternalField)))
					{
						if (py.PropertyType == typeof(bool))
						{
							bool value = (bool)py.GetValue(target, new object[]{});
							
							if (HasAttribute(py, typeof(ReadonlyField)))
							{
								EditorGUILayout.LabelField(py.Name, value.ToString());
							}
							else
							{
								value = EditorGUILayout.Toggle(py.Name, value);
								py.SetValue(target, value, new object[]{});
							}
						}
						else if (py.PropertyType == typeof(int))
						{
							int value = (int)py.GetValue(target, new object[]{});
							
							if (HasAttribute(py, typeof(ReadonlyField)))
							{
								EditorGUILayout.LabelField(py.Name, value.ToString());
							}
							else
							{
								value = EditorGUILayout.IntField(py.Name, value);
								py.SetValue(target, value, new object[]{});
							}
						}
						else if (py.PropertyType == typeof(float))
						{
							float value = (float)py.GetValue(target, new object[]{});
							
							if (HasAttribute(py, typeof(ReadonlyField)))
							{
								EditorGUILayout.LabelField(py.Name, value.ToString());
							}
							else
							{
								value = EditorGUILayout.FloatField(py.Name, value);
								py.SetValue(target, value, new object[]{});
							}
						}
						else if (py.PropertyType == typeof(long))
						{
							long value = (long)py.GetValue(target, new object[]{});
							
							if (HasAttribute(py, typeof(ReadonlyField)))
							{
								EditorGUILayout.LabelField(py.Name, value.ToString());
							}
							else
							{
								value = EditorGUILayout.LongField(py.Name, value);
								py.SetValue(target, value, new object[]{});
							}
						}
						else if (py.PropertyType == typeof(double))
						{
							double value = (double)py.GetValue(target, new object[]{});
							
							if (HasAttribute(py, typeof(ReadonlyField)))
							{
								EditorGUILayout.LabelField(py.Name, value.ToString());
							}
							else
							{
								value = EditorGUILayout.DoubleField(py.Name, value);
								py.SetValue(target, value, new object[]{});
							}
						}
						else if (py.PropertyType == typeof(string))
						{
							string value = (string)py.GetValue(target, new object[]{});
							if (string.IsNullOrEmpty(value))
								value = string.Empty;
							
							if (HasAttribute(py, typeof(ReadonlyField)))
							{
								EditorGUILayout.LabelField(py.Name, value.ToString());
							}
							else
							{
								if (HasAttribute(py, typeof(DirectoryField)))
								{
									EditorGUILayout.BeginHorizontal();
									
									value = EditorGUILayout.TextField(py.Name, value);
									
									if (GUILayout.Button("...", GUILayout.Width(25), GUILayout.Height(15)))
									{
										string szDirectory = EditorUtility.OpenFolderPanel(KeyWord.Select.ToString(),
										                                                   Application.dataPath, string.Empty);
										if (!string.IsNullOrEmpty(szDirectory))
										{
											OnSelectDirectoryChange(szDirectory, py, target);
											
											value = szDirectory.Substring(
												Application.dataPath.Length - 6
												);
										}
									}
									
									py.SetValue(target, value, new object[]{});
									EditorGUILayout.EndHorizontal();
								}
								else
								{
									value = EditorGUILayout.TextField(py.Name, value);
									py.SetValue(target, value, new object[]{});
								}
								
							}
						}
						else if (py.PropertyType == typeof(Vector2))
						{
							Vector2 value = (Vector2)py.GetValue(target, new object[]{});
							
							if (HasAttribute(py, typeof(ReadonlyField)))
							{
								EditorGUILayout.LabelField(py.Name, value.ToString());
							}
							else
							{
								value = EditorGUILayout.Vector2Field(py.Name, value);
								py.SetValue(target, value, new object[]{});
							}
						}
						else if (py.PropertyType == typeof(Vector3))
						{
							Vector3 value = (Vector3)py.GetValue(target, new object[]{});
							
							if (HasAttribute(py, typeof(ReadonlyField)))
							{
								EditorGUILayout.LabelField(py.Name, value.ToString());
							}
							else
							{
								value = EditorGUILayout.Vector3Field(py.Name, value);
								py.SetValue(target, value, new object[]{});
							}
						}
						else if (py.PropertyType == typeof(Vector4))
						{
							Vector4 value = (Vector4)py.GetValue(target, new object[]{});
							
							if (HasAttribute(py, typeof(ReadonlyField)))
							{
								EditorGUILayout.LabelField(py.Name, value.ToString());
							}
							else
							{
								value = EditorGUILayout.Vector4Field(py.Name, value);
								py.SetValue(target, value, new object[]{});
							}
						}
						else if (py.PropertyType == typeof(Color))
						{
							Color value = (Color)py.GetValue(target, new object[]{});
							
							if (HasAttribute(py, typeof(ReadonlyField)))
							{
								EditorGUILayout.LabelField(py.Name, value.ToString());
							}
							else
							{
								value = EditorGUILayout.ColorField(py.Name, value);
								py.SetValue(target, value, new object[]{});
							}
						}
						else if (py.PropertyType == typeof(GameObject))
						{
							GUILayout.Label(py.Name, EditorStyles.boldLabel);
							
							EditorGUILayout.BeginVertical(EditorStyles.helpBox);
							
							GameObject value = (GameObject)py.GetValue(target, new object[]{});
							value = (GameObject)EditorGUILayout.ObjectField(value, typeof(GameObject), true);
							py.SetValue(target, value, new object[]{});
							
							if (value)
							{
								if (HasAttribute(py, typeof(ReadonlyField)))
								{
									EditorGUILayout.LabelField(KeyWord.Position.ToString(),
									                           value.transform.position.ToString());
									EditorGUILayout.LabelField(KeyWord.EulerAngle.ToString(),
									                           value.transform.eulerAngles.ToString());
									EditorGUILayout.LabelField(KeyWord.Scale.ToString(), 
									                           value.transform.localScale.ToString());
									
									SphereCollider sphere = value.GetComponent<SphereCollider>();
									if (sphere)
									{
										GUILayout.Label(typeof(SphereCollider).Name, EditorStyles.boldLabel);
										EditorGUILayout.LabelField(
											KeyWord.Center.ToString(), sphere.center.ToString()
											);
										EditorGUILayout.LabelField(
											KeyWord.Radius.ToString(), sphere.radius.ToString()
											);
									}
									
									BoxCollider box = value.GetComponent<BoxCollider>();
									if (box)
									{
										GUILayout.Label(typeof(BoxCollider).Name, EditorStyles.boldLabel);
										EditorGUILayout.LabelField(KeyWord.Trigger.ToString(), box.isTrigger.ToString());
										EditorGUILayout.LabelField(
											KeyWord.Center.ToString(), box.center.ToString()
											);
										EditorGUILayout.LabelField(
											KeyWord.Size.ToString(), box.size.ToString()
											);
									}
								}
								else
								{
									value.transform.position 	= EditorGUILayout.Vector3Field(
										KeyWord.Position.ToString(), value.transform.position
										);
									value.transform.eulerAngles = EditorGUILayout.Vector3Field(
										KeyWord.EulerAngle.ToString(), value.transform.eulerAngles
										);
									value.transform.localScale 	= EditorGUILayout.Vector3Field(
										KeyWord.Scale.ToString(), value.transform.localScale
										);
									
									SphereCollider sphere = value.GetComponent<SphereCollider>();
									if (sphere)
									{
										GUILayout.Label(typeof(SphereCollider).Name, EditorStyles.boldLabel);
										sphere.center = EditorGUILayout.Vector3Field(
											KeyWord.Center.ToString(), sphere.center
											);
										sphere.radius = EditorGUILayout.FloatField(
											KeyWord.Radius.ToString(), sphere.radius
											);
									}
									
									BoxCollider box = value.GetComponent<BoxCollider>();
									if (box)
									{
										GUILayout.Label(typeof(BoxCollider).Name, EditorStyles.boldLabel);
										box.isTrigger = EditorGUILayout.Toggle(KeyWord.Trigger.ToString(), box.isTrigger);
										box.center 	= EditorGUILayout.Vector3Field(
											KeyWord.Center.ToString(), box.center
											);
										box.size 	= EditorGUILayout.Vector3Field(
											KeyWord.Size.ToString(), box.size
											);
									}
								}
								
								value.tag = KeyWord.EditorOnly.ToString();
							}
							
							EditorGUILayout.EndVertical();
						}
						else 
						{
							object value = py.GetValue(target, new object[]{});
							if (value != default(object))
							{
								if (value.GetType().IsEnum)
								{
									value = EditorGUILayout.EnumPopup(py.Name, (System.Enum)value);
									py.SetValue(target, value, new object[]{});
								}
								else
								{
									GUILayout.Label(py.Name, EditorStyles.boldLabel);

									EditorGUILayout.BeginVertical(EditorStyles.helpBox);
									Invalidate(value, value.GetType(),
									           HasAttribute(py, typeof(ReadonlyField)));
									EditorGUILayout.EndVertical();
								}
							}
						}
					}
				}
			}
		}

		FieldInfo[] aryField = type.GetFields();
		foreach(FieldInfo field in aryField)
		{
			object value = field.GetValue(target);
			if (value != default(object) && !HasAttribute(field, typeof(InternalField)))
			{
				if (value is IList)
				{
					GUILayout.Label(field.Name, EditorStyles.boldLabel);

					IList result = (IList)value;
					if (result.Count > 0)
					{
						EditorGUILayout.BeginVertical(EditorStyles.helpBox);
						foreach(object element in (IList)value)
						{
							if (element != default(object))
							{
								GUILayout.Space(10);
								GUILayout.Label(element.GetType().Name, EditorStyles.boldLabel);
		
								Invalidate(element, element.GetType(),
								           HasAttribute(field, typeof(ReadonlyField)));
							}
						}

						EditorGUILayout.EndVertical();
					}
				}
			}
		}
	}

	/// <summary>
	/// Raises the disable event.
	/// </summary>
	public virtual void OnDisable()
	{
		
	}

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	public virtual void OnEnable ()
	{
		
	}

	/// <summary>
	/// Raises the select directory change event.
	/// </summary>
	/// <param name="szDirectory">Size directory.</param>
	/// <param name="target">Target.</param>
	public virtual void OnSelectDirectoryChange(string szDirectory, PropertyInfo p, object target)
	{

	}
}
