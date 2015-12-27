using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// I entity movable.
/// </summary>
public class AIIdle : AIBaseState
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AIBattle"/> class.
	/// </summary>
	/// <param name="nStateID">N state I.</param>
	/// <param name="entity">Entity.</param>
	public AIIdle(int nStateID, IEntity entity)
		: base(nStateID, entity)
	{

	}

	/// <summary>
	/// Raises the start event.
	/// </summary>
	public override bool OnStart ()
	{
		SqlShape sqlShape = GameSqlLite.GetSingleton().Query<SqlShape>(Self.ShapeID);
		if (sqlShape)
		{
			if (Self.GetApplyRootMotion())
				Self.ApplyRootMotion(false);

			PlayAction(sqlShape.Idle);
		}

		return base.OnStart ();
	}

	/// <summary>
	/// Raises the exit event.
	/// </summary>
	public override bool OnExit ()
	{
		if (Self.GetApplyRootMotion())
			Self.ApplyRootMotion(true);

		return base.OnExit ();
	}
}
