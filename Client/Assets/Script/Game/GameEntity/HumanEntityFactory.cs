using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Human entity factory.
/// </summary>
public class HumanEntityFactory : IEntityFactory
{
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
		GameObject capsule = new GameObject(nID.ToString());
		if (!capsule)
			throw new System.NullReferenceException();

		capsule.transform.parent 		= parent;
		capsule.transform.position		= vPos;
		capsule.transform.localScale	= vScale;
		capsule.transform.eulerAngles	= vEuler;

		ICharacterEntity entity = capsule.AddComponent<ICharacterEntity>();
		if (!entity)
			throw new System.NullReferenceException();

		entity.ID 		= nID;
		entity.Name 	= szName;
		entity.Style 	= nStyle;
		entity.Type 	= type;

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
				entity.SetShape(entityShape);
			});

			if (!string.IsNullOrEmpty(sqlShape.Name))
				entity.Name = sqlShape.Name;
		}

		return entity;
	}
}
