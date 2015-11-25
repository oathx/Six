using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using LitJson;
using System.IO;
using System.Xml;

public enum SceneKeyWord
{
	SqlID,
	Group,
}

/// <summary>
/// Scene monster create setting.
/// </summary>
public class SceneMonsterCreateSetting : XmlStruct
{
	/// <summary>
	/// Gets or sets the path.
	/// </summary>
	/// <value>The path.</value>
	[DirectoryField]
	public string				Path
	{ get; set; }

	/// <summary>
	/// Gets or sets the type of the search.
	/// </summary>
	/// <value>The type of the search.</value>
	public SearchFileType		SearchType
	{ get; set; }

	/// <summary>
	/// Gets or sets the create monster I.
	/// </summary>
	/// <value>The create monster I.</value>
	[CustomField]
	public int 					SqlID
	{ get; set; }

	/// <summary>
	/// The select.
	/// </summary>
	[InternalField]
	public Dictionary<int, string>
		Select = new Dictionary<int, string>();

	/// <summary>
	/// Gets or sets the group.
	/// </summary>
	/// <value>The group.</value>
	public int 					Group
	{ get; set; }

	/// <summary>
	/// Gets or sets the monster info.
	/// </summary>
	/// <value>The monster info.</value>
	[ReadonlyField]
	public SqlMonster			Detail
	{ get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="SceneMonsterCreateSetting"/> class.
	/// </summary>
	public SceneMonsterCreateSetting()
	{
		Path 		= string.Empty;
		SearchType 	= SearchFileType.prefab;
	
		// load all monster data
		InstallSqlDatebase();
	}

	/// <summary>
	/// Installs the sql datebase.
	/// </summary>
	public void InstallSqlDatebase()
	{
		GameSqlLite.GetSingleton().OpenDB(WUrl.SqlitePathWin32);
		
		GameSqlLite.GetSingleton().RegisterSqlPackageFactory(
			typeof(SqlMonster).Name, new DefaultSqlPackageFactory<SqlMonster>()
			);
		
		List<SqlMonster> arySqlMonster = GameSqlLite.GetSingleton().QueryTable<SqlMonster>();
		foreach(SqlMonster sqlMonster in arySqlMonster)
		{
			Select.Add(sqlMonster.ID, sqlMonster.Name);
		}

		if (SqlID <= 0)
			SqlID = arySqlMonster[0].ID;

		GameSqlLite.GetSingleton().CloseDB();
	}
}

/// <summary>
/// Scene monster.
/// </summary>
public class SceneMonster : XmlStruct
{
	/// <summary>
	/// Gets or sets the I.
	/// </summary>
	/// <value>The I.</value>
	public int 			ID
	{ get; set; }

	/// <summary>
	/// Gets or sets the group.
	/// </summary>
	/// <value>The group.</value>
	public int 			Group
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the sql monster I.
	/// </summary>
	/// <value>The sql monster I.</value>
	[ReadonlyField]
	public int 			SqlID
	{ get; set; }

	/// <summary>
	/// Gets or sets the height of the battle.
	/// </summary>
	/// <value>The height of the battle.</value>
	public int 			BattleHeight
	{ get; set; }

	/// <summary>
	/// Gets or sets the AI bunch.
	/// </summary>
	/// <value>The AI bunch.</value>
	public int 			AIBunch
	{ get; set; }

	/// <summary>
	/// Gets or sets the AI event.
	/// </summary>
	/// <value>The AI event.</value>
	public string 		AIEvent
	{ get; set; }

	/// <summary>
	/// Gets or sets the shape.
	/// </summary>
	/// <value>The shape.</value>
	public GameObject	Shape
	{ get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="SceneMonster"/> class.
	/// </summary>
	public SceneMonster()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SceneMonster"/> class.
	/// </summary>
	/// <param name="nID">N I.</param>
	/// <param name="nSqlID">N sql I.</param>
	/// <param name="shape">Shape.</param>
	public SceneMonster(int nID, int nSqlID, GameObject shape, int nGroup)
	{
		ID = nID; SqlID = nSqlID; Shape = shape; Group = nGroup;
	}
}

/// <summary>
/// Scene xml struct.
/// </summary>
public class SceneXmlStruct : XmlStruct
{
	[ReadonlyField]
	public string						Path
	{ get; set; }

	/// <summary>
	/// Gets or sets the name.
	/// </summary>
	/// <value>The name.</value>
	[ReadonlyField]
	public string						Name
	{ get; set; }

	/// <summary>
	/// Gets or sets the setting.
	/// </summary>
	/// <value>The setting.</value>
	public SceneMonsterCreateSetting	Setting
	{ get; set; }
	
	/// <summary>
	/// The monster.
	/// </summary>
	public List<SceneMonster>			Monster = new List<SceneMonster>();

	/// <summary>
	/// Initializes a new instance of the <see cref="SceneScriptableObject"/> class.
	/// </summary>
	public SceneXmlStruct()
	{
		Setting = new SceneMonsterCreateSetting();
		Name	= string.Empty;
	}
	
	/// <summary>
	/// Update the specified nSqlID.
	/// </summary>
	/// <param name="nSqlID">N sql I.</param>
	public void 	Update(int nSqlID)
	{
		GameSqlLite.GetSingleton().OpenDB(WUrl.SqlitePathWin32);
		GameSqlLite.GetSingleton().RegisterSqlPackageFactory(
			typeof(SqlMonster).Name, new DefaultSqlPackageFactory<SqlMonster>()
			);

		// update monster detail info
		Setting.Detail = GameSqlLite.GetSingleton().Query<SqlMonster>(nSqlID);
		if (!Setting.Detail)
			throw new System.NullReferenceException();

		GameSqlLite.GetSingleton().CloseDB();
	}

	/// <summary>
	/// Gets the asset path.
	/// </summary>
	/// <returns>The asset path.</returns>
	public void		Create()
	{
		GameSqlLite.GetSingleton().OpenDB(WUrl.SqlitePathWin32);

		// register the monster shape factory
		GameSqlLite.GetSingleton().RegisterSqlPackageFactory(
			typeof(SqlShape).Name, new DefaultSqlPackageFactory<SqlShape>()
			);

		// query the monster shape
		SqlShape shape = GameSqlLite.GetSingleton().Query<SqlShape>(Setting.Detail.ShapeID);
		if (!shape)
			throw new System.NullReferenceException();

		string szAssetPath = string.Format("{0}/{1}", 
		                                   Application.dataPath, !string.IsNullOrEmpty(Setting.Path) ? Setting.Path.Substring(7) : Setting.Path);
		if (!string.IsNullOrEmpty(szAssetPath))
		{
			string[] aryFile = System.IO.Directory.GetFiles(szAssetPath, 
			                                                string.Format("*{0}*.{1}", shape.Skeleton, Setting.SearchType.ToString().ToLower()), 
			                                                SearchOption.AllDirectories
			                                                );
			foreach(string file in aryFile)
			{
				szAssetPath = file.Substring(Application.dataPath.Length - 6);
				if (!string.IsNullOrEmpty(szAssetPath))
				{
					GameObject resource = AssetDatabase.LoadAssetAtPath<GameObject>(szAssetPath);
					if (!resource)
						throw new System.NullReferenceException(szAssetPath);

					GameObject target = GameObject.Instantiate(resource) as GameObject;
					if (target)
					{
						target.name = resource.name;

						SphereCollider sphere = target.AddComponent<SphereCollider>();
						if (sphere)
							sphere.radius = 10;

						Selection.activeGameObject = target;

						Monster.Add(
							new SceneMonster(Monster.Count, Setting.SqlID, target, Setting.Group)
							);
					}

					break;
				}
			}
		}

		GameSqlLite.GetSingleton().CloseDB();
	}

	/// <summary>
	/// Load the specified szPath.
	/// </summary>
	/// <param name="szPath">Size path.</param>
	public override void Load(string szPath)
	{
		Clearup();

		base.Load(szPath);
	}

	/// <summary>
	/// Saves as.
	/// </summary>
	/// <param name="szPath">Size path.</param>
	public override void	Save(string szPath)
	{
		base.Save(szPath);

		XmlDocument doc = new XmlDocument();
		
		// create xml define
		XmlDeclaration declaration = doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
		doc.AppendChild (declaration);
		
		XmlElement data = doc.CreateElement (
			KeyWord.Data.ToString ().ToLower()
			);
		doc.AppendChild (data);

		foreach(SceneMonster monster in Monster)
		{
			if (monster.Shape)
			{
				XmlElement npc = doc.CreateElement(
					KeyWord.Npc.ToString().ToLower()
					);
				data.AppendChild(npc);

				SphereCollider sphere = monster.Shape.GetComponent<SphereCollider>();
				if (!sphere)
					throw new System.NullReferenceException();

				npc.SetAttribute("npcid", 			monster.ID.ToString());
				npc.SetAttribute("npcbaseid", 		monster.SqlID.ToString());
				npc.SetAttribute("groupid", 		monster.Group.ToString());
				npc.SetAttribute("bornpos", 		VS3("{0}:{1}:{2}", monster.Shape.transform.position));
				npc.SetAttribute("battle_center", 	VS3("{0}:{1}:{2}", sphere.center));
				npc.SetAttribute("battle_height", 	monster.BattleHeight.ToString());
				npc.SetAttribute("battle_radius", 	sphere.radius.ToString());
				npc.SetAttribute("aibunch", 		monster.AIBunch.ToString());
				npc.SetAttribute("aievent", 		monster.AIEvent);
				npc.SetAttribute("walkpos", 		string.Empty);
			}
		}

		szPath = szPath.Replace(KeyWord.Xml.ToString(), KeyWord.Pass.ToString());
		doc.Save(szPath);
	}

	/// <summary>
	/// Clearup this instance.
	/// </summary>
	public override void Clearup()
	{
		foreach(SceneMonster m in Monster)
		{
			if (m.Shape)
			{
				GameObject.DestroyImmediate(m.Shape);
			}
		}

		Monster.Clear();
	}
}
