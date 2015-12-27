using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

/// <summary>
/// Monster struct.
/// </summary>
public class MonsterStruct : IEventArgs
{
	/// <summary>
	/// Gets or sets the npc I.
	/// </summary>
	/// <value>The npc I.</value>
	public int 			ID
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the base I.
	/// </summary>
	/// <value>The base I.</value>
	public int 			NpcID
	{ get; set; }

	/// <summary>
	/// Gets or sets the type.
	/// </summary>
	/// <value>The type.</value>
	public EntityType	Type
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the position.
	/// </summary>
	/// <value>The position.</value>
	public Vector3		Position
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the battle center.
	/// </summary>
	/// <value>The battle center.</value>
	public Vector3		BattleCenter
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the battle radius.
	/// </summary>
	/// <value>The battle radius.</value>
	public float		BattleRadius
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the wave.
	/// </summary>
	/// <value>The wave.</value>
	public int 			Wave
	{ get; set; }
	
	/// <summary>
	/// Parse the specified e.
	/// </summary>
	/// <param name="e">E.</param>
	public void 		Parse(XmlElement ele)
	{
		ID 			= XmlParser.GetIntValue(ele, "npcid");
		NpcID 		= XmlParser.GetIntValue(ele, "npcbaseid");
		Wave 		= XmlParser.GetIntValue(ele, "groupid");
		Type		= (EntityType)XmlParser.GetIntValue(ele, "type");
		
		string szPos = XmlParser.GetStringValue(ele, "bornpos");
		if (!string.IsNullOrEmpty(szPos))
		{
			string[] aryPosition = szPos.Split(':');
			if (aryPosition.Length >= 3)
				Position = new Vector3(float.Parse(aryPosition[0]), float.Parse(aryPosition[1]), float.Parse(aryPosition[2]));
		}
	}
}

/// <summary>
/// Map monster.
/// </summary>
public class MapMonster
{
	/// <summary>
	/// The m_d monster.
	/// </summary>
	protected Dictionary<int, MonsterStruct> 
		dMonster = new Dictionary<int, MonsterStruct>();
	
	/// <summary>
	/// Parse this instance.
	/// </summary>
	public void 			Parse(string text)
	{
		XmlParser parser = new XmlParser(text);
		
		foreach (XmlElement e in parser.root.GetElementsByTagName("npc"))
		{
			MonsterStruct cm = new MonsterStruct();
			cm.Parse(e);
			
			if (!dMonster.ContainsKey(cm.ID))
				dMonster.Add(cm.ID, cm);
		}
	}
	
	/// <summary>
	/// Gets the monster.
	/// </summary>
	/// <returns>The monster.</returns>
	/// <param name="nNpcID">N npc I.</param>
	public MonsterStruct		GetMonster(int nNpcID)
	{
		return dMonster [nNpcID];
	}
	
	/// <summary>
	/// Gets the monster group.
	/// </summary>
	/// <returns>The monster group.</returns>
	/// <param name="nID">N I.</param>
	public List<MonsterStruct>	GetMonsterGroup(int nID)
	{
		List<MonsterStruct> aryResult = new List<MonsterStruct> ();
		foreach(KeyValuePair<int, MonsterStruct> it in dMonster)
		{
			if (it.Value.Wave == nID)
				aryResult.Add(it.Value);
		}
		
		return aryResult;
	}
}

/// <summary>
/// Character table.
/// </summary>
public class MonsterTable : MemoryTable<MonsterTable, int, MapMonster>
{	
	/// <summary>
	/// Gets the group monster.
	/// </summary>
	/// <returns>The group monster.</returns>
	/// <param name="nMapID">N map I.</param>
	/// <param name="nGroupID">N group I.</param>
	public List<MonsterStruct>	GetGroupMonster(SqlScene sqlScene, int nGroupID)
	{
		if (!m_dTable.ContainsKey (sqlScene.ID))
		{
			IResourceManager resMgr = GameEngine.GetSingleton().QueryPlugin<IResourceManager>();
			if (resMgr)
			{
				TextAsset asset = resMgr.GetAsset<TextAsset>(WUrl.AssetPassPath, sqlScene.Pass);
				if (!asset)
					throw new System.NullReferenceException();
				
				MapMonster monster = new MapMonster();
				monster.Parse(asset.text);
				
				m_dTable.Add(
					sqlScene.ID, monster);
			}
		}
		
		return m_dTable [sqlScene.ID].GetMonsterGroup (nGroupID); 
	}

	/// <summary>
	/// Gets the group monster.
	/// </summary>
	/// <returns>The group monster.</returns>
	/// <param name="nSceneID">N scene I.</param>
	/// <param name="nGroupID">N group I.</param>
	public List<MonsterStruct>	GetGroupMonster(int nSceneID, int nGroupID)
	{
		SqlScene sqlScene = GameSqlLite.GetSingleton().Query<SqlScene>(nSceneID);
		if (!sqlScene)
			throw new System.NullReferenceException();

		return GetGroupMonster(sqlScene, nGroupID);
	}
}

