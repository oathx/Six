using UnityEngine;
using System.Xml;
using System.IO;

/// <summary>
/// Example:  
/// string filePath =  Application.dataPath + "/Client/Config/";
/// XmlParser xmlParser = new XmlParser(filePath + "ErrorCode.xml");
/// if (xmlParser.Parse() == false)
/// {
/// 	return;
/// }
/// 
/// XmlElement root = xmlParser.root;
/// foreach (XmlElement errorEle in XmlParser.GetChildren(root, "ErrorCode"))
///{
/// int errorcode = int.Parse(XmlParser.GetAttributeVal(errorEle, "id"));
///	string errorMsg = XmlParser.GetAttributeVal(errorEle, "errorMsg")
///}
/// 
/// </summary>
public class XmlParser
{
	/// <summary>
	/// Xml document.
	/// </summary>
	public XmlDocument xmlDoc { get; private set; }
	
	/// <summary>
	/// Xml root element.
	/// </summary>
	public XmlElement root { get; private set; }
	
	/// <summary>
	/// Initializes a new instance of the <see cref="XmlParser"/> class.
	/// </summary>
	/// <param name="filePath">File path.</param>
	public XmlParser(string xml)
	{
		Parse(xml);
	}
	
	/// <summary>
	/// Parse xml file format to Element.
	/// </summary>
	public bool Parse(string xml)
	{
		try
		{
			xmlDoc = new XmlDocument();
			xmlDoc.LoadXml(xml);
			root = xmlDoc.DocumentElement;
			return true;
		}
		catch (XmlException e)
		{
			Debug.LogError(e.ToString());
			Dispose();
			return false;
		}
	}
	
	/// <summary>
	/// Releases all resource used by the <see cref="XmlParser"/> object.
	/// </summary>
	/// <remarks>Call <see cref="Dispose"/> when you are finished using the <see cref="XmlParser"/>. The <see cref="Dispose"/>
	/// method leaves the <see cref="XmlParser"/> in an unusable state. After calling <see cref="Dispose"/>, you must
	/// release all references to the <see cref="XmlParser"/> so the garbage collector can reclaim the memory that the
	/// <see cref="XmlParser"/> was occupying.</remarks>
	public void Dispose()
	{
		root = null;
	}
	
	/// <summary>
	/// Gets the root children.
	/// </summary>
	/// <returns>The children.</returns>
	/// <param name="tagName">Tag name.</param>
	public XmlNodeList GetChildren(string tagName)
	{
		if (root != null)
		{
			root.GetElementsByTagName(tagName);
		}
		return null;
	}
	
	
	/// <summary>
	/// Gets the text by specified XmlElement
	/// </summary>
	/// <returns>The text.</returns>
	/// <param name="element">Element.</param>
	public static string GetText(XmlElement element)
	{
		if (null != element)
		{
			return element.InnerText;
		}
		return "";
	}
	
	/// <summary>
	/// Gets the attribute value by speified Xml Element and attribute name.
	/// </summary>
	/// <returns>The attribute value.</returns>
	/// <param name="element">Element.</param>
	/// <param name="attrName">Attr name.</param>
	public static string GetStringValue(XmlElement element, string attrName)
	{
		if (null != element && element.HasAttribute(attrName))
		{
			return element.GetAttribute(attrName);
		}
		return null;
	}
	
	
	/// <summary>
	/// Gets the int value.
	/// </summary>
	/// <returns>The int value.</returns>
	/// <param name="element">Element.</param>
	/// <param name="attrName">Attr name.</param>
	public static int GetIntValue(XmlElement element, string attrName)
	{
		string val = GetStringValue(element, attrName);
		if (!string.IsNullOrEmpty(val))
		{
			return int.Parse(val);
		}
		return 0;
	}
	
	
	/// <summary>
	/// Gets the int value.
	/// </summary>
	/// <returns>The int value.</returns>
	/// <param name="element">Element.</param>
	/// <param name="attrName">Attr name.</param>
	public static float GetFloatValue(XmlElement element, string attrName)
	{
		string val = GetStringValue(element, attrName);
		if (!string.IsNullOrEmpty(val))
		{
			return float.Parse(val);
		}
		return 0.0f;
	}
	
	
	/// <summary>
	/// Gets the int value.
	/// </summary>
	/// <returns>The int value.</returns>
	/// <param name="element">Element.</param>
	/// <param name="attrName">Attr name.</param>
	public static short GetShortValue(XmlElement element, string attrName)
	{
		string val = GetStringValue(element, attrName);
		if (!string.IsNullOrEmpty(val))
		{
			return short.Parse(val);
		}
		return 0;
	}
	
	/// <summary>
	/// Gets the boolean value.
	/// </summary>
	/// <returns><c>true</c>, if boolean value was gotten, <c>false</c> otherwise.</returns>
	/// <param name="element">Element.</param>
	/// <param name="attrName">Attr name.</param>
	public static bool GetBooleanValue(XmlElement element, string attrName)
	{
		string val = GetStringValue(element, attrName);
		if (!string.IsNullOrEmpty(val))
		{
			return bool.Parse(val);
		}
		return false;
	}
	
	
	/// <summary>
	/// Gets the int value.
	/// </summary>
	/// <returns>The int value.</returns>
	/// <param name="element">Element.</param>
	/// <param name="attrName">Attr name.</param>
	public static double GetDoubleValue(XmlElement element, string attrName)
	{
		string val = GetStringValue(element, attrName);
		if (!string.IsNullOrEmpty(val))
		{
			return double.Parse(val);
		}
		return 0;
	}
	
	
	/// <summary>
	/// Gets the int value.
	/// </summary>
	/// <returns>The int value.</returns>
	/// <param name="element">Element.</param>
	/// <param name="attrName">Attr name.</param>
	public static int GetByteValue(XmlElement element, string attrName)
	{
		string val = GetStringValue(element, attrName);
		if (!string.IsNullOrEmpty(val))
		{
			return sbyte.Parse(val);
		}
		return 0;
	}
	
	/// <summary>
	/// Determines if has attribute the specified element attrName.
	/// </summary>
	/// <returns><c>true</c> if has attribute the specified element attrName; otherwise, <c>false</c>.</returns>
	/// <param name="element">Element.</param>
	/// <param name="attrName">Attr name.</param>
	public static bool HasAttribute(XmlElement element, string attrName)
	{
		if (null != element)
		{
			return element.HasAttribute(attrName);
		}
		return false;
	}
	
	/// <summary>
	/// Gets the specified Element's children.
	/// </summary>
	/// <returns>The children.</returns>
	/// <param name="element">Element.</param>
	/// <param name="tagName">Tag name.</param>
	public static XmlNodeList GetChildren(XmlElement element, string tagName)
	{
		if (element != null)
		{
			return element.GetElementsByTagName(tagName);
		}
		return null;
	}
	
	
	
}
