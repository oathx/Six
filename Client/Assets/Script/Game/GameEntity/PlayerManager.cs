using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Player manager.
/// </summary>
public class PlayerManager : IEntityManager
{
	/// <summary>
	/// Gets the main player.
	/// </summary>
	/// <value>The main player.</value>
	public PlayerEntity		MainPlayer
	{ get; private set; }

	/// <summary>
	/// Sets the player entity.
	/// </summary>
	/// <param name="entity">Entity.</param>
	public virtual void 	SetPlayer(PlayerEntity entity)
	{
		MainPlayer = entity;
	}

	/// <summary>
	/// Gets the player.
	/// </summary>
	/// <returns>The player.</returns>
	public PlayerEntity		GetPlayer()
	{
		return MainPlayer;
	}
}
