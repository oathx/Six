using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// I entity movable.
/// </summary>
public class AIMove : AIBaseState
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AIBattle"/> class.
	/// </summary>
	/// <param name="nStateID">N state I.</param>
	/// <param name="entity">Entity.</param>
	public AIMove(int nStateID, IEntity entity)
		: base(nStateID, entity)
	{
		
	}

	/// <summary>
	/// Raises the start event.
	/// </summary>
	public override bool OnStart ()
	{
		base.OnStart ();

		SqlShape sqlShape = GameSqlLite.GetSingleton().Query<SqlShape>(Self.ShapeID);
		if (sqlShape)
		{
			if (Self.GetApplyRootMotion())
				Self.ApplyRootMotion(false);
			
			PlayAction(sqlShape.Move);
		}

		return true;
	}

	/// <summary>
	/// Raises the exit event.
	/// </summary>
	public override bool OnExit ()
	{
		base.OnExit ();
		
		if (!Self.GetApplyRootMotion())
			Self.ApplyRootMotion(true);
		
		// stop move
		Self.Move (Vector3.zero, 0);
		
		return true;
	}
}
