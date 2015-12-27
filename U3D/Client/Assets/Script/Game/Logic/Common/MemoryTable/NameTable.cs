using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class NameStruct
{
	public string[] 	Surname
	{ get; set; }
	
	public string[]		Name
	{ get; set; }
}

/// <summary>
/// Character table.
/// </summary>
public class NameTable : MemoryTable<NameTable, int, NameStruct>
{
	private NameStruct	RandName = new NameStruct();
	/// <summary>
	/// Raises the enable event.
	/// </summary>
	void OnEnable()
	{
		if (Empty())
		{
			TextAsset asset = Resources.Load ("Default/Config/RandomName", typeof(TextAsset)) as TextAsset;
			if (!asset)
				throw new System.NullReferenceException();
			
			XmlParser parser = new XmlParser(asset.text);
			
			// get parse node
			XmlNodeList a = parser.root.GetElementsByTagName("FirstName");
			XmlNodeList b = parser.root.GetElementsByTagName("LastName");
			
			if (a.Count > 0 && b.Count > 0)
			{
				string s = XmlParser.GetText(a[0] as XmlElement);
				string n = XmlParser.GetText(b[0] as XmlElement);
				
				RandName.Surname 	= s.Split(',');
				RandName.Name		= n.Split(',');
			}
			
			Resources.UnloadAsset(asset);
		}
	}
	
	/// <summary>
	/// Gets the name.
	/// </summary>
	/// <returns>The name.</returns>
	public string 	GetName()
	{
		int nLast 	= Random.Range (0, RandName.Surname.Length - 1);
		int nFirst 	= Random.Range (0, RandName.Name.Length - 1);
		
		return RandName.Surname[nLast] + RandName.Name[nFirst];
	}
}

