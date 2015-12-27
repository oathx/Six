using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AI;

/// <summary>
/// Human entity factory.
/// </summary>
public class MonsterEntityFactory : HumanEntityFactory
{
	/// <summary>
	/// The asset path.
	/// </summary>
	public string 		AssetPath
	{ get; set; }
	
	/// <summary>
	/// Initializes a new instance of the <see cref="PlayerEntityFactory"/> class.
	/// </summary>
	public MonsterEntityFactory()
	{
		AssetPath 	= WUrl.AssetMonsterPath;
	}
	
	/// <summary>
	/// Creates the entity.
	/// </summary>
	/// <returns>The entity.</returns>
	/// <param name="ab">Ab.</param>
	/// <param name="nID">N I.</param>
	/// <param name="szName">Size name.</param>
	/// <param name="vPos">V position.</param>
	/// <param name="vScale">V scale.</param>
	/// <param name="vEuler">V euler.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	/// <param name="type">Type.</param>
	/// <param name="args">Arguments.</param>
	/// <param name="nStyle">N style.</param>
	/// <param name="parent">Parent.</param>
	public override IEntity	CreateEntity(EntityType type, int nID, string szName,
	                                     Vector3 vPos, Vector3 vScale, Vector3 vEuler, object args, int nStyle, Transform parent)
	{
		SqlMonster sqlMonster = GameSqlLite.GetSingleton().Query<SqlMonster>(System.Convert.ToInt32(args));
		if (!sqlMonster)
			throw new System.NullReferenceException();

		// get the monster shape
		SqlShape sqlShape = GameSqlLite.GetSingleton().Query<SqlShape>(sqlMonster.ShapeID);

		// create monster entity
		MonsterEntity monster = CreateEntityInstance<MonsterEntity>(type, nID, szName, 
		                                                             vPos, vScale, vEuler, AssetPath, sqlShape, nStyle, parent);
		if (!monster)
			throw new System.NullReferenceException();

		monster.ID 				= nID;
		monster.Name 			= string.IsNullOrEmpty(szName) ? sqlShape.Name : szName;
		monster.Style 			= nStyle;
		monster.Type 			= type;
		monster.MaxMoveSpeed	= 4;
		monster.MaxRotateSpeed	= 360;
		monster.ShapeID 		= sqlMonster.ShapeID;

		monster.CreateTitle(nID, nStyle, szName);

		// create ai tress
		CreateAITree(monster, sqlMonster.Blueprint);

		return monster;
	}

	/// <summary>
	/// Gets the shape.
	/// </summary>
	/// <returns>The shape.</returns>
	/// <param name="nID">N I.</param>
	public virtual SqlShape	GetShape(int nID)
	{
		SqlMonster sqlMonster = GameSqlLite.GetSingleton().Query<SqlMonster>(nID);
		if (!sqlMonster)
			throw new System.NullReferenceException();
		
		return  GameSqlLite.GetSingleton().Query<SqlShape>(sqlMonster.ShapeID);
	}

	/// <summary>
	/// Creates the AI tree.
	/// </summary>
	/// <returns>The AI tree.</returns>
	/// <param name="szBlueprintFile">Size blueprint file.</param>
	public AIBehaviourTree	CreateAITree(MonsterEntity monster, string szBlueprintFile)
	{
		if (!string.IsNullOrEmpty(szBlueprintFile))
		{
			TextAsset asset = Resources.Load<TextAsset>(szBlueprintFile);
			if (!asset)
				throw new System.NullReferenceException();
			
			// create the monster ai tree
			return AIBehaviourTreeManager.GetSingleton().CreateAIBehaviourTree(
				monster.ID, new AIEntityContext(monster), asset.text);
		}

		return default(AIBehaviourTree);
	}
}


