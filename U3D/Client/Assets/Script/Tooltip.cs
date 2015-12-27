using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

/// <summary>
/// Card manager.
/// </summary>
public class Tooltip
{
	const string XML_ROOT	= "root";
	const string XML_ITEM	= "item";
	const string XML_ID		= "id";
	const string XML_TEXT	= "text";

	/// <summary>
	/// The d text.
	/// </summary>
	static Dictionary<int, 
		string> dText = new Dictionary<int, string>();

	/// <summary>
	/// Startup this instance.
	/// </summary>
	public static void 		Startup()
	{
		TextAsset asset = Resources.Load("Default/Tooltip", typeof(TextAsset)) as TextAsset;
		if (asset)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(asset.text);
			
			// get root node list
			XmlNodeList root = doc.SelectNodes(XML_ROOT);
			foreach(XmlNode node in root)
			{
				XmlNodeList textNodeList = node.SelectNodes(XML_ITEM);
				foreach(XmlNode item in textNodeList)
				{
					int nID = System.Convert.ToInt32(item.Attributes[XML_ID].Value);
					if (!dText.ContainsKey(nID))
					{
						dText.Add(nID, item.Attributes[XML_TEXT].Value);
					}
				}
			}
		}
	}

	/// <summary>
	/// Queries the text.
	/// </summary>
	/// <returns>The text.</returns>
	/// <param name="nID">N I.</param>
	public static string	QueryText(int nID)
	{
		if (dText.ContainsKey(nID))
			return dText[nID];

		return string.Empty;
	}
}