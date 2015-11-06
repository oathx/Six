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
		int nAIFlag = AITypeID.AI_IDLE;
		
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
	                         Vector3 vPos, Vector3 vScale, Vector3 vEuler, object args, int nStyle, Transform parent) where T : ICharacterEntity
	{
		GameObject capsule = new GameObject(nID.ToString());
		if (!capsule)
			throw new System.NullReferenceException();
		
		capsule.transform.parent 		= parent;
		capsule.transform.position		= vPos;
		capsule.transform.localScale	= vScale;
		capsule.transform.eulerAngles	= vEuler;
		
		T entity = capsule.AddComponent<T>();
		if (!entity)
			throw new System.NullReferenceException();
		
		entity.ID 				= nID;
		entity.Name 			= szName;
		entity.Style 			= nStyle;
		entity.Type 			= type;
		entity.MaxMoveSpeed		= 4;
		entity.MaxRotateSpeed	= 360;

		SqlShape sqlShape = GameSqlLite.GetSingleton().Query<SqlShape>((int)args);
		if (sqlShape)
		{
			entity.MoveableController.center 	= sqlShape.Center;
			entity.MoveableController.radius	= sqlShape.Radius;
			entity.MoveableController.height	= sqlShape.Height;
			
			// get the enetity shape factory
			IEntityShapeFactory factory = EntityShapeFactoryManager.GetSingleton().GetShapeFactory(
				typeof(DefaultShapeFactory).Name
				);
			
			// create entity shape
			factory.CreateShape(sqlShape.ID, delegate(IEntityShape entityShape) {

				// set the entity shape
				entity.SetShape(entityShape);

				// create the entity machine, add entity state
				int nAIFlag = AITypeID.AI_IDLE | AITypeID.AI_MOVE;

				IAIMachine machine = AIConstruct.CreateMachine(nAIFlag, entity);
				if (machine)
				{
					entity.SetMachine(machine);

					// change to default state
					machine.ChangeState(
						machine.HasState(AITypeID.AI_BORN) ? AITypeID.AI_BORN : AITypeID.AI_IDLE
						);
				}
			});
			
			if (!string.IsNullOrEmpty(sqlShape.Name))
				entity.Name = sqlShape.Name;
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
		return CreateEntityInstance<ICharacterEntity>(type, nID, szName, vPos, vScale, vEuler, args, nStyle, parent);
	}
}
