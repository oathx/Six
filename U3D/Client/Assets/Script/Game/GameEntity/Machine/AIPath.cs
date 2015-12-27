using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// I entity movable.
/// </summary>
public class AIPath : AIBaseState
{
	/// <summary>
	/// Initializes a new instance of the <see cref="AIBattle"/> class.
	/// </summary>
	/// <param name="nStateID">N state I.</param>
	/// <param name="entity">Entity.</param>
	public AIPath(int nStateID, IEntity entity)
		: base(nStateID, entity)
	{
		
	}
}

