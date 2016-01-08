using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// I magic factory.
/// </summary>
public abstract class MagicShapeFactory
{
	/// <summary>
	/// Creates the magic.
	/// </summary>
	/// <returns>The magic.</returns>
	/// <param name="nMagicID">N magic I.</param>
	public abstract MagicShape	CreateMagic(SqlMagic magic);
}

/// <summary>
/// Default magic factory.
/// </summary>
public class DefaultMagicFactory<T> : MagicShapeFactory where T : MagicShape, new()
{
	/// <summary>
	/// Creates the magic.
	/// </summary>
	/// <returns>The magic.</returns>
	/// <param name="nMagicID">N magic I.</param>
	public override MagicShape	CreateMagic(SqlMagic magic)
	{
		MagicShape shape = new T();
		shape.OnInit (magic);

		return shape;
	}
}
