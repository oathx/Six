using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

/// <summary>
/// Mission dialog struct.
/// </summary>
public class PlotStruct : IEventArgs
{
	public int 		StoryID
	{ get; set; }

	public int 		NpcID
	{ get; set; }

	public string	Text
	{ get; set; }

	public void Parse(XmlElement element)
	{
		StoryID = XmlParser.GetIntValue(element, "storyID");
		NpcID 	= XmlParser.GetIntValue(element, "npcId");
		Text 	= XmlParser.GetStringValue(element, "talk");
	}
}

/// <summary>
/// Dialog struct.
/// </summary>
public class DialogStruct : IEventArgs
{
	/// <summary>
	/// Gets or sets the I.
	/// </summary>
	/// <value>The I.</value>
	public int 	ID
	{ get; set; }

	/// <summary>
	/// The plot.
	/// </summary>
	public List<PlotStruct> 
		Plot = new List<PlotStruct>();

	/// <summary>
	/// Parse the specified element.
	/// </summary>
	/// <param name="element">Element.</param>
	public void Parse(XmlElement element)
	{
		ID = XmlParser.GetIntValue(element, "id");

		foreach(XmlElement child in element.ChildNodes)
		{
			PlotStruct p = new PlotStruct();
			p.Parse(child);

			Plot.Add(p);
		}
	}
}

/// <summary>
/// Mission dialog.
/// </summary>
public class MissionDialog : MemoryTable<MissionDialog, int, DialogStruct>
{
	/// <summary>
	/// Raises the enable event.
	/// </summary>
	void OnEnable()
	{
		TextAsset asset = Resources.Load ("Default/Config/MissionDialog", typeof(TextAsset)) as TextAsset;
		if (asset)
		{
			XmlParser parser = new XmlParser(asset.text);
			foreach (XmlElement e in parser.root.GetElementsByTagName("plot"))
			{
				DialogStruct cm = new DialogStruct();
				cm.Parse(e);
				
				Add(cm.ID, cm);
			}

			Resources.UnloadAsset(asset);
		}
	}
}
