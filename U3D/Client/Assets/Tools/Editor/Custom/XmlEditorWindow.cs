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
	Reset,
	Xml,
	Create,
	Asset,
	PropertyName,
	Unity,
	Pass,
	Build,
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
	zip,
	cs,
}


/// <summary>
/// Button event handle.
/// </summary>
public delegate void ButtonClicked(object target, PropertyInfo p);

/// <summary>
/// Readonly field.
/// </summary>
public class ReadonlyField 
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
/// Custom field.
/// </summary>
public class CustomField 
	: System.Attribute
{

}

/// <summary>
/// Button field.
/// </summary>
public class ButtonField
	: System.Attribute
{
	/// <summary>
	/// Gets or sets the name.
	/// </summary>
	/// <value>The name.</value>
	public string	Name
	{ get; set; }
	
	/// <summary>
	/// Initializes a new instance of the <see cref="ButtonField"/> class.
	/// </summary>
	/// <param name="name">Name.</param>
	public ButtonField(string name)
	{
		Name = name; 
	}
}

/// <summary>
/// Type helper.
/// </summary>
public class TypeHelper
{
	/// <summary>
	/// Gets the name of the type by.
	/// </summary>
	/// <returns>The type by name.</returns>
	/// <param name="szName">Size name.</param>
	public static System.Type	QueryTypeName(string szName)
	{
		Assembly assembly = Assembly.GetExecutingAssembly();
		return assembly.GetType(szName);
	}
	
	/// <summary>
	/// Gets the property.
	/// </summary>
	/// <returns>The property.</returns>
	/// <param name="type">Type.</param>
	public static PropertyInfo GetProperty(System.Type type, string szPropertyName)
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
	public static FieldInfo GetField(System.Type type, string szName)
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
	/// Readonly the specified py.
	/// </summary>
	/// <param name="py">Py.</param>
	public static bool		Has(PropertyInfo py, System.Type typeField)
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
	public static bool		Has(FieldInfo fi, System.Type typeField)
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
	/// Gets the attribute.
	/// </summary>
	/// <returns>The attribute.</returns>
	/// <param name="p">P.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T			GetAttribute<T>(PropertyInfo p) where T : System.Attribute
	{
		object[] aryAttribute = p.GetCustomAttributes (false);
		foreach(object a in aryAttribute)
		{
			if (a.GetType() == typeof(T))
				return (T)a;
		}

		return default(T);
	}
}

public class DirectoryStruct
{
	/// <summary>
	/// Gets or sets the type of the file.
	/// </summary>
	/// <value>The type of the file.</value>
	public SearchFileType	FileType
	{ get; set; }

	/// <summary>
	/// Gets or sets the path.
	/// </summary>
	/// <value>The path.</value>
	public string			Path
	{ get; set; }

	/// <summary>
	/// Gets or sets the full path.
	/// </summary>
	/// <value>The full path.</value>
	public string			FullPath
	{ get; set; }

	/// <summary>
	/// The file array.
	/// </summary>
	public List<string>		
		FileArray = new List<string>();

	/// <summary>
	/// Gets or sets the title.
	/// </summary>
	/// <value>The title.</value>
	public string			Title
	{ get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="DirectoryStruct"/> class.
	/// </summary>
	public DirectoryStruct()
	{
		Title = new string(new char[]{
			'.',
			'.',
			'.',
		});
	}

	/// <summary>
	/// Clear this instance.
	/// </summary>
	public void 			Clear()
	{
		FileArray.Clear();
	}
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
}

/// <summary>
/// I custom editor.
/// </summary>
public class XmlEditorWindow<T> : EditorWindow where T : XmlStruct, new()
{
	public T				Instance 	= new T();
	public Vector2			Scorll 		= Vector2.zero;
	public string			Path 		= string.Empty;

	/// <summary>
	/// Initializes a new instance of the <see cref="XmlEditorWindow`1"/> class.
	/// </summary>
	public XmlEditorWindow()
	{
		Init ();
	}

	/// <summary>
	/// Init this instance.
	/// </summary>
	public virtual void 	Init()
	{
		string dir = string.Format ("{0}/{1}", Application.dataPath, typeof(EditorWindow).Name);
		if (!Directory.Exists (dir))
			Directory.CreateDirectory (dir);
		
		Path = string.Format ("{0}/{1}.{2}",
		                      dir, typeof(T).Name, SearchFileType.xml.ToString ());
		if (!File.Exists (Path))
		{
			File.WriteAllText(Path, string.Empty);
		}
	}
	
	/// <summary>
	/// Raises the disable event.
	/// </summary>
	public virtual void 	OnDisable()
	{
		Init ();

		string text = LitJson.JsonMapper.ToJson(Instance);
		if (!string.IsNullOrEmpty(text))
		{
			File.WriteAllText(Path, text);
		}
	}
	
	/// <summary>
	/// Raises the enable event.
	/// </summary>
	public virtual void 	OnEnable ()
	{
		Init ();

		string text = File.ReadAllText(Path);
		if (!string.IsNullOrEmpty(text))
		{
			Instance = LitJson.JsonMapper.ToObject<T>(text);
		}
	}

	/// <summary>
	/// Raises the inspector GU event.
	/// </summary>
	public virtual void 	OnGUI()
	{
		try{
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
				if (GUILayout.Button(KeyWord.Import.ToString()))
				{
					string szFilePath = EditorUtility.OpenFilePanel(KeyWord.Import.ToString(), Application.dataPath, KeyWord.Xml.ToString());
					if (!string.IsNullOrEmpty(szFilePath))
					{
						string text = File.ReadAllText(szFilePath);
						if (!string.IsNullOrEmpty(text))
						{
							Instance = LitJson.JsonMapper.ToObject<T>(text);
						}
					}
				}

				if (GUILayout.Button(KeyWord.Export.ToString()))
				{
					string szPath = EditorUtility.SaveFilePanel(KeyWord.Save.ToString(), 
					                                            Application.dataPath, GetType().Name, KeyWord.Xml.ToString());
					if (!string.IsNullOrEmpty(szPath))
					{
						string text = LitJson.JsonMapper.ToJson(Instance);
						if (!string.IsNullOrEmpty(text))
						{
							File.WriteAllText(szPath, text);
						}
					}
				}

				if (GUILayout.Button(KeyWord.Reset.ToString()))
				{
					if (!string.IsNullOrEmpty(Path))
					{
						Instance = new T();
					}
				}

				OnCustomGUIPrev();
				EditorGUILayout.EndVertical();
				
				Scorll = EditorGUILayout.BeginScrollView (Scorll);
				Invalidate(Instance, Instance.GetType(), false);
				EditorGUILayout.EndScrollView ();
				
				OnCustomGUIBack();
			}
		}
		catch(System.Exception e)
		{

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
				if (TypeHelper.Has(py, typeof(CustomField)))
				{
					OnCustomField(target, py);
				}
				else
				{
					if (!TypeHelper.Has(py, typeof(InternalField)))
					{
						if (py.PropertyType == typeof(bool))
						{
							bool value = (bool)py.GetValue(target, new object[]{});
							
							if (TypeHelper.Has(py, typeof(ReadonlyField)))
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
							
							if (TypeHelper.Has(py, typeof(ReadonlyField)))
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
							
							if (TypeHelper.Has(py, typeof(ReadonlyField)))
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
							
							if (TypeHelper.Has(py, typeof(ReadonlyField)))
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
							
							if (TypeHelper.Has(py, typeof(ReadonlyField)))
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
							
							if (TypeHelper.Has(py, typeof(ReadonlyField)))
							{
								EditorGUILayout.LabelField(py.Name, value.ToString());
							}
							else
							{
								value = EditorGUILayout.TextField(py.Name, value);
								py.SetValue(target, value, new object[]{});
							}
						}
						else if (py.PropertyType == typeof(Vector2))
						{
							Vector2 value = (Vector2)py.GetValue(target, new object[]{});
							
							if (TypeHelper.Has(py, typeof(ReadonlyField)))
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
							
							if (TypeHelper.Has(py, typeof(ReadonlyField)))
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
							
							if (TypeHelper.Has(py, typeof(ReadonlyField)))
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
							
							if (TypeHelper.Has(py, typeof(ReadonlyField)))
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
						
						}
						else if (py.PropertyType == typeof(DirectoryStruct))
						{
							object value = py.GetValue(target, new object[]{});
							if (value != default(object))
							{
								DirectoryStruct dir = (DirectoryStruct)value;

								EditorGUILayout.BeginVertical(EditorStyles.helpBox);

								SearchFileType newType = (SearchFileType)EditorGUILayout.EnumPopup(typeof(SearchFileType).Name, (System.Enum)dir.FileType);
								if (dir.FileType != newType)
								{
									dir.FileType = newType;

									if (!string.IsNullOrEmpty(dir.FullPath))
									{
										dir.FileArray 	= XmlEditorHelper.SearchFile(dir.FileType, dir.FullPath);
									}
								}

								EditorGUILayout.BeginHorizontal();
								EditorGUILayout.LabelField(py.Name, dir.Path);
								if (GUILayout.Button(dir.Title, GUILayout.Width(25), GUILayout.Height(15)))
								{
									string szDirectory = EditorUtility.OpenFolderPanel(KeyWord.Select.ToString(),
									                                                   Application.dataPath, string.Empty);
									if (!string.IsNullOrEmpty(szDirectory))
									{
										dir.Path 		= szDirectory.Substring(Application.dataPath.Length + 1);
										dir.FullPath	= szDirectory;
										dir.FileArray 	= XmlEditorHelper.SearchFile(dir.FileType, szDirectory);
									}
								}
								EditorGUILayout.EndHorizontal();

								if (!string.IsNullOrEmpty(dir.Path))
								{
									EditorGUILayout.BeginVertical(EditorStyles.helpBox);
									foreach(string file in dir.FileArray)
									{
										EditorGUILayout.LabelField(string.Empty, file);
									}
									EditorGUILayout.EndVertical();
								}

								EditorGUILayout.EndVertical();
							}
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
								else if (value is ButtonClicked)
								{
									ButtonClicked callback = (ButtonClicked)value;
									if (callback != default(ButtonClicked))
									{
										string szName = callback.ToString();

										ButtonField bf = TypeHelper.GetAttribute<ButtonField>(py);
										if (bf != default(ButtonField))
											szName = bf.Name;

										if (GUILayout.Button(szName))
										{
											callback(target, py);
										}
									}
								}
								else
								{
									GUILayout.Label(py.Name, EditorStyles.boldLabel);

									EditorGUILayout.BeginVertical(EditorStyles.helpBox);
									Invalidate(value, value.GetType(),
									           TypeHelper.Has(py, typeof(ReadonlyField)));
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
			if (value != default(object) && !TypeHelper.Has(field, typeof(InternalField)))
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
								           TypeHelper.Has(field, typeof(ReadonlyField)));
							}
						}

						EditorGUILayout.EndVertical();
					}
				}
			}
		}
	}
}
