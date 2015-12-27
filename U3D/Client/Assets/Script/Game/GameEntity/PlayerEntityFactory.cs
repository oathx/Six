using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Human entity factory.
/// </summary>
public class PlayerEntityFactory : HumanEntityFactory
{
	/// <summary>
	/// The asset path.
	/// </summary>
	public string AssetPath
	{ get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="PlayerEntityFactory"/> class.
	/// </summary>
	public PlayerEntityFactory()
	{
		AssetPath = WUrl.AssetCharacterPath;
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
		// Query main player job info
		SqlJob sqlJob 		= GameSqlLite.GetSingleton().Query<SqlJob>(System.Convert.ToInt32(args));
		if (!sqlJob)
			throw new System.NullReferenceException();

		SqlShape sqlShape 	= GameSqlLite.GetSingleton().Query<SqlShape>(sqlJob.ShapeID);
		if (!sqlShape)
			throw new System.NullReferenceException();

		PlayerEntity player = CreateEntityInstance<PlayerEntity>(type, nID, szName, vPos, vScale, vEuler, 
		                                          	AssetPath, sqlShape, nStyle, parent);
		if (!player)
			throw new System.NullReferenceException();
			
		player.ID 				= nID;
		player.Name 			= string.IsNullOrEmpty(szName) ? sqlShape.Name : szName;
		player.Style 			= nStyle;
		player.Type 			= type;
		player.MaxMoveSpeed		= 4;
		player.MaxRotateSpeed	= 360;
		player.ShapeID 			= sqlJob.ShapeID;

		// create player title
		player.CreateTitle(nID, nStyle, szName);

		// create player machine
		IAIMachine machine = CreateMachine(player);
		if (machine)
		{
			player.SetMachine(machine);
			
			// change to default state
			machine.ChangeState(
				machine.HasState(AITypeID.AI_BORN) ? AITypeID.AI_BORN : AITypeID.AI_IDLE
				);
		}

		return player;
	}
}

