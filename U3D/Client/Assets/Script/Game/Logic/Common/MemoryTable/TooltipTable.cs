using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

/// <summary>
/// Tooltip struct.
/// </summary>
public class TooltipStruct
{
	public int 		ID
	{ get; set; }

	public string	Text
	{ get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="TooltipStruct"/> class.
	/// </summary>
	public TooltipStruct()
	{

	}

	/// <summary>
	/// Initializes a new instance of the <see cref="TooltipStruct"/> class.
	/// </summary>
	/// <param name="nID">N I.</param>
	/// <param name="szText">Size text.</param>
	public TooltipStruct(int nID, string szText)
	{
		ID = nID; Text = szText;
	}
}

/// <summary>
/// Character table.
/// </summary>
public class TooltipTable : SimpleSingleton<TooltipTable>
{
	public Dictionary<int, string>
		aryTooltip = new Dictionary<int, string>();

	/// <summary>
	/// Initializes a new instance of the <see cref="TooltipTable"/> class.
	/// </summary>
	public TooltipTable()
	{
		TextAsset asset = Resources.Load ("Default/SqlTooltip", typeof(TextAsset)) as TextAsset;
		if (!asset)
			throw new System.NullReferenceException();
		
		XmlParser parser = new XmlParser(asset.text);
		
		// get parse node
		XmlNodeList aryRecord = parser.root.GetElementsByTagName("RECORDS");
		foreach(XmlElement record in aryRecord)
		{
			aryTooltip.Add(XmlParser.GetIntValue(record, "ID"), 
			               XmlParser.GetStringValue(record, "Text"));
		}

		Resources.UnloadAsset(asset);
	}

	/// <summary>
	/// Gets the text.
	/// </summary>
	/// <returns>The text.</returns>
	/// <param name="nCode">N code.</param>
	public string	GetText(int nCode)
	{
		return aryTooltip[nCode];
	}
}

