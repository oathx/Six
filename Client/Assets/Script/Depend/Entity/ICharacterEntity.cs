using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// I character entity.
/// </summary>
public class ICharacterEntity : IEntityMovable
{
	public IEntityShape		Shape 
	{ get; private set; }

	/// <summary>
	/// Sets the shape.
	/// </summary>
	/// <param name="shape">Shape.</param>
	public virtual void 	SetShape(IEntityShape newShape)
	{
		StartCoroutine(OnResetShape(newShape));
	}

	/// <summary>
	/// Raises the reset shape event.
	/// </summary>
	/// <param name="newShape">New shape.</param>
	IEnumerator				OnResetShape(IEntityShape newShape)
	{
		// destroy current old entity shape
		if (Shape != newShape)
		{
			if (Shape)
				GameObject.Destroy(Shape.gameObject);
		}

		// bind new entity shape
		Shape = newShape;

		// set the shape parent
		Shape.SetParent(
			transform
			);

		yield return new WaitForEndOfFrame();

		// reset
		Shape.SetPosition(Vector3.zero);
	}

	/// <summary>
	/// Gets the shape.
	/// </summary>
	/// <returns>The shape.</returns>
	public IEntityShape		GetShape()
	{
		return Shape;
	}

	/// <summary>
	/// Play the specified szAnimationName.
	/// </summary>
	/// <param name="szAnimationName">Size animation name.</param>
	public virtual void 	Play(string szAnimationName, float fTransition, bool bReplay)
	{

	}
}