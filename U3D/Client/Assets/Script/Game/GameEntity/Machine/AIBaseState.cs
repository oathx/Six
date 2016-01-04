using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// I entity movable.
/// </summary>
public class AIBaseState : IAIState
{
	/// <summary>
	/// Gets the self.
	/// </summary>
	/// <value>The self.</value>
	public BaseUnitEntity	Self { get; private set; }
	
	public class ActionBuffer
	{
		/// <summary>
		/// Gets or sets the name of the animation.
		/// </summary>
		/// <value>The name of the animation.</value>
		public string	AnimationName
		{ get; set; }

		/// <summary>
		/// Gets or sets the duration.
		/// </summary>
		/// <value>The duration.</value>
		public float	Duration
		{ get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="AIBaseState+ActionBuffer"/> class.
		/// </summary>
		/// <param name="szAnimationName">Size animation name.</param>
		/// <param name="fDuration">F duration.</param>
		public ActionBuffer(string szAnimationName, float fDuration)
		{
			AnimationName = szAnimationName; Duration = fDuration;
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="AIBaseState"/> class.
	/// </summary>
	/// <param name="nStateID">N state I.</param>
	/// <param name="entity">Entity.</param>
	public AIBaseState(int nStateID, IEntity entity)
		: base(nStateID)
	{
		Self = entity as BaseUnitEntity;
	}

	/// <summary>
	/// Raises the condition event.
	/// </summary>
	/// <param name="target">Target.</param>
	public override bool 	OnCondition (IAIState target)
	{
		return true;
	}

	/// <summary>
	/// Raises the start event.
	/// </summary>
	public override bool 	OnStart ()
	{
		return true;
	}

	/// <summary>
	/// Raises the update event.
	/// </summary>
	/// <param name="fElapsed">F elapsed.</param>
	public override bool 	OnUpdate (float fElapsed)
	{
		return false;
	}

	/// <summary>
	/// Raises the exit event.
	/// </summary>
	public override bool 	OnExit ()
	{
		return true;
	}

	/// <summary>
	/// Raises the event event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	public override bool 	OnEvent (IEvent evt)
	{
		return true;
	}

	/// <summary>
	/// Plaies the action.
	/// </summary>
	/// <returns><c>true</c>, if action was played, <c>false</c> otherwise.</returns>
	/// <param name="sqlAction">Sql action.</param>
	public virtual bool		PlayAction(SqlAction sqlAction)
	{
		if (!string.IsNullOrEmpty(sqlAction.Motion))
		{
			Self.Play(sqlAction.Motion, sqlAction.MotionTransition, false);
		}

		return true;
	}

	/// <summary>
	/// Plaies the action.
	/// </summary>
	/// <returns><c>true</c>, if action was played, <c>false</c> otherwise.</returns>
	/// <param name="nActionID">N action I.</param>
	public virtual bool		PlayAction(int nActionID)
	{
		SqlAction sqlAction = GameSqlLite.GetSingleton().Query<SqlAction>(nActionID);
		if (!sqlAction)
			throw new System.NullReferenceException(nActionID.ToString());

		return PlayAction(sqlAction);
	}
}
