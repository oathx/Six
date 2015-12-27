using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class ActionStruct
{
	public int 			ID
	{ get; set; }
	
	public string		Name
	{ get; set; }
	
	public int 			Hard
	{ get; set; }
	
	public int 			Shift
	{ get; set; }
	
	public int 			Interrupt
	{ get; set; }
	
	public float		Transition
	{ get; set; }
	
	public int 			Posture
	{ get; set; }
}

/// <summary>
/// Meta action.
/// </summary>
public sealed class MetaAction
{
	private Dictionary<int, ActionStruct> dStruct = new Dictionary<int, ActionStruct>();
	
	/// <summary>
	/// Gets the type I.
	/// </summary>
	/// <value>The type I.</value>
	public int 	TypeID
	{ get; private set; }
	
	/// <summary>
	/// Parse the specified emt.
	/// </summary>
	/// <param name="emt">Emt.</param>
	public void Parse(XmlElement emt)
	{
		TypeID = XmlParser.GetIntValue(emt, "id");
		
		XmlNodeList aryAction = emt.GetElementsByTagName("item");
		foreach(XmlElement item in aryAction)
		{
			ActionStruct v = new ActionStruct();
			v.ID			= XmlParser.GetIntValue(item, "id");
			v.Name			= XmlParser.GetStringValue(item, "animation");
			v.Hard			= XmlParser.GetIntValue(item, "hard");
			v.Shift 		= XmlParser.GetIntValue(item, "shift");
			v.Interrupt 	= XmlParser.GetIntValue(item, "interrupt");
			v.Transition 	= XmlParser.GetFloatValue(item, "transition");
			v.Posture 		= XmlParser.GetIntValue(item, "startChangePosture");
			
			if (!dStruct.ContainsKey(v.ID))
			{
				dStruct.Add(v.ID, v);
			}
		}
	}
	
	/// <summary>
	/// Gets the action.
	/// </summary>
	/// <returns>The action.</returns>
	/// <param name="nID">N I.</param>
	public ActionStruct	GetAction(int nID)
	{
		if (!dStruct.ContainsKey(nID))
			throw new System.NullReferenceException("Can't find nID:" + nID.ToString());
		
		return dStruct [nID];
	}
}

/// <summary>
/// Character table.
/// </summary>
public class ActionTable : MemoryTable<ActionTable, int, MetaAction>
{
	/// <summary>
	/// Raises the enable event.
	/// </summary>
	void OnEnable()
	{
		if (Empty())
		{
			TextAsset asset = Resources.Load ("Default/Config/Action", typeof(TextAsset)) as TextAsset;
			if (asset)
			{
				XmlParser parser = new XmlParser(asset.text);
				foreach (XmlElement e in parser.root.GetElementsByTagName("type"))
				{
					MetaAction cm = new MetaAction();
					cm.Parse(e);
					
					Add(cm.TypeID, cm);
				}
				
				Resources.UnloadAsset(asset);
			}
		}
	}
	
	/// <summary>
	/// Gets the action struct.
	/// </summary>
	/// <returns>The action struct.</returns>
	/// <param name="nTypeID">N type I.</param>
	/// <param name="nActionID">N action I.</param>
	public ActionStruct	GetActionStruct(int nTypeID, int nActionID)
	{
		if (!m_dTable.ContainsKey (nTypeID))
			throw new System.NullReferenceException (nTypeID.ToString ());
		
		return m_dTable [nTypeID].GetAction (nActionID);
	}
}


