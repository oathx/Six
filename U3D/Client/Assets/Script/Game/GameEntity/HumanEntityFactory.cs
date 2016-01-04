using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Human entity factory.
/// </summary>
public class HumanEntityFactory : IEntityFactory
{
	/// <summary>
	/// Game engine.
	/// </summary>
	public class AIConstruct
	{
		/// <summary>
		/// Creates the machine.
		/// </summary>
		/// <returns>The machine.</returns>
		/// <param name="nAIFlag">N AI flag.</param>
		public static IAIMachine 	CreateMachine(int nAIFlag, IEntity entity)
		{
			IAIMachine machine = new IAIMachine ();
			
			if ((nAIFlag & AITypeID.AI_IDLE) != 0)
			{
				machine.RegisterState(
					new AIIdle(AITypeID.AI_IDLE, entity)
					);
			}

			if ((nAIFlag & AITypeID.AI_MOVE) != 0)
			{
				machine.RegisterState(
					new AIMove(AITypeID.AI_MOVE, entity)
					);
			}

			if ((nAIFlag & AITypeID.AI_BATTLE) != 0)
			{
				machine.RegisterState(
					new AIBattle(AITypeID.AI_BATTLE, entity)
					);
			}

			if ((nAIFlag & AITypeID.AI_PATH) != 0)
			{
				machine.RegisterState(
					new AIPath(AITypeID.AI_PATH, entity)
					);
			}

			if ((nAIFlag & AITypeID.AI_BORN) != 0)
			{
				machine.RegisterState(
					new AIBorn(AITypeID.AI_BORN, entity)
					);
			}

			if ((nAIFlag & AITypeID.AI_CHARGE) != 0)
			{
				machine.RegisterState(
					new AICharge(AITypeID.AI_CHARGE, entity)
					);
			}
			
			return machine;
		}
	}

	/// <summary>
	/// Creates the machine.
	/// </summary>
	/// <returns>The machine.</returns>
	/// <summary>
	/// Creates the machine.
	/// </summary>
	/// <param name="entity">Entity.</param>
	public IAIMachine 		CreateMachine(IEntity entity)
	{
		int nAIFlag = AITypeID.AI_IDLE | AITypeID.AI_MOVE | AITypeID.AI_BATTLE | AITypeID.AI_PATH | AITypeID.AI_BORN | AITypeID.AI_CHARGE;
		
		// create ai machine
		return AIConstruct.CreateMachine (nAIFlag, entity);
	}

	/// <summary>
	/// Creates the entity instance.
	/// </summary>
	/// <returns>The entity instance.</returns>
	/// <param name="type">Type.</param>
	/// <param name="nID">N I.</param>
	/// <param name="szName">Size name.</param>
	/// <param name="vPos">V position.</param>
	/// <param name="vScale">V scale.</param>
	/// <param name="vEuler">V euler.</param>
	/// <param name="args">Arguments.</param>
	/// <param name="nStyle">N style.</param>
	/// <param name="parent">Parent.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public virtual T	CreateEntityInstance<T>(EntityType type, int nID, string szName,
	                         Vector3 vPos, Vector3 vScale, Vector3 vEuler, string szAssetPath, SqlShape sqlShape, int nStyle, Transform parent) where T : ICharacterEntity
	{
		// get entity shape asset resource
		GameObject resource = m_ResourceManager.GetAsset<GameObject>(szAssetPath, sqlShape.Skeleton);
		if (!resource)
			throw new System.NullReferenceException();

		// create skeleton gameobject
		GameObject shape = GameObject.Instantiate(resource) as GameObject;
	
		shape.transform.name				= nID.ToString();
		shape.transform.position 			= vPos;
		shape.transform.localScale			= vScale;
		shape.transform.localEulerAngles 	= vEuler;
		shape.transform.parent				= parent;
		
		T entity = shape.AddComponent<T>();
		if (!entity)
			throw new System.NullReferenceException();

		entity.MoveableController.center 	= sqlShape.Center;
		entity.MoveableController.radius	= sqlShape.Radius;
		entity.MoveableController.height	= sqlShape.Height;

		Dictionary<MountType, string[]> dmt = new Dictionary<MountType, string[]> ();
		dmt.Add(MountType.DUMMY_WING, 		new string[]{"Bone_Chibang"});
		dmt.Add(MountType.DUMMY_PICK, 		new string[]{"Bone_Caiji"});
		dmt.Add(MountType.DUMMY_ARM, 		new string[]{"Bone_Staff", "Bone_Bow", "Bone_Sword"});
		dmt.Add(MountType.DUMMY_GIDDINESS, 	new string[]{"Bone_Yun"});
		dmt.Add(MountType.DUMMY_HORSE, 		new string[]{"Bone_Zuoqi"});
		
		// add the shape all moune
		entity.InstallMount(dmt);
		
		foreach(int equip in sqlShape.Equip)
		{
			SqlItem sqlItem = GameSqlLite.GetSingleton().Query<SqlItem>(equip);
			if (!string.IsNullOrEmpty(sqlItem.Url))
			{
				if (sqlItem.Part != (int)PartType.PT_ARM)
				{	
					// load the equip mesh
					GameObject mesh = m_ResourceManager.GetAsset<GameObject>(WUrl.AssetCharacterPath, sqlItem.Url);
					if (!mesh)
						throw new System.NullReferenceException(sqlItem.Url);
					
					StringHolder holder = m_ResourceManager.GetAsset<StringHolder>(
						WUrl.AssetCharacterPath, sqlItem.ExtendUrl
						);
					if (holder)
						entity.ChangeEquip((PartType)sqlItem.Part, mesh, holder);
				}
				else
				{
					Transform mount = entity.GetMount(MountType.DUMMY_ARM);
					if (mount)
					{
						// load the equip mesh
						GameObject mesh = m_ResourceManager.GetAsset<GameObject>(WUrl.AssetArmPath, sqlItem.Url);
						if (!mesh)
							throw new System.NullReferenceException(sqlItem.Url);
						
						GameObject arm = GameObject.Instantiate(mesh) as GameObject;
						if (!arm)
							throw new System.NullReferenceException();
						
						arm.name					= mesh.name;
						arm.transform.parent 		= mount;
						arm.layer					= entity.gameObject.layer;
						
						arm.transform.localPosition = Vector3.zero;
						arm.transform.localRotation = Quaternion.identity;
						arm.transform.localScale 	= Vector3.one;
					}
				}
			}
		}

		switch(type)
		{
		case EntityType.ET_MAIN:
			entity.SetLayer(EntityLayer.MAIN);
			break;

		case EntityType.ET_NPC:
			entity.SetLayer(EntityLayer.NPC);
			break;

		case EntityType.ET_MONSTER:
			entity.SetLayer(EntityLayer.MONSTER);
			break;
		}

		return entity;
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
		SqlShape sqlShape = GameSqlLite.GetSingleton().Query<SqlShape>(System.Convert.ToInt32(args));
		if (!sqlShape)
			throw new System.NullReferenceException();

		return CreateEntityInstance<BaseUnitEntity>(type, nID, szName, vPos, vScale, vEuler, WUrl.AssetCharacterPath, sqlShape, nStyle, parent);
	}
}
