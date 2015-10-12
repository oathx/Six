using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// I character entity.
/// </summary>
public class ICharacterEntity : IEntityMovable
{
	protected IEntityShape	m_EntityShape;
	
	/// <summary>
	/// Sets the shape.
	/// </summary>
	/// <param name="shape">Shape.</param>
	public virtual void 	SetShape(IEntityShape newShape)
	{
		// destroy current old entity shape
		if (m_EntityShape != newShape)
		{
			if (m_EntityShape)
				GameObject.Destroy(m_EntityShape.gameObject);
		}
	
		m_EntityShape = newShape;

		// apply new entity shape
		if (m_EntityShape)
		{
			m_EntityShape.SetParent(transform);
		}
	}

	/// <summary>
	/// Gets the shape.
	/// </summary>
	/// <returns>The shape.</returns>
	public IEntityShape		GetShape()
	{
		return m_EntityShape;
	}

	/// <summary>
	/// Play the specified szAnimationName.
	/// </summary>
	/// <param name="szAnimationName">Size animation name.</param>
	public virtual void 	Play(string szAnimationName, float fTransition, bool bReplay)
	{

	}
}