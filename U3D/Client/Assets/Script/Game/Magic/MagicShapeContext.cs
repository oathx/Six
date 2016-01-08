using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// I magic context.
/// </summary>
public class MagicShapeContext : INullObject
{
	/// <summary>
	/// Gets the owner.
	/// </summary>
	/// <value>The owner.</value>
	public IEntity			Owner
	{ get; private set; }

	/// <summary>
	/// Gets the enemy.
	/// </summary>
	/// <value>The enemy.</value>
	public List<IEntity>	Enemy
	{ get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="IMagicContext`1"/> class.
	/// </summary>
	/// <param name="owner">Owner.</param>
	public MagicShapeContext(IEntity owner)
	{
		Owner = owner;
	}	
}
