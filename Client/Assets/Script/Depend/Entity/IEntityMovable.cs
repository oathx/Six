using UnityEngine;
using System.Collections;

/// <summary>
/// I entity movable.
/// </summary>
[RequireComponent(typeof(IEntityMotor))]
public class IEntityMovable : IEntity
{
	public CharacterController	MoveableController
	{ get; private set; }

	/// <summary>
	/// Awake this instance.
	/// </summary>
	protected override void Awake()
	{
		MoveableController = GetComponent<CharacterController>();
		if (!MoveableController)
			MoveableController = gameObject.AddComponent<CharacterController>();

		base.Awake();
	}

	/// <summary>
	/// Sets the enabled.
	/// </summary>
	/// <param name="enabled">If set to <c>true</c> enabled.</param>
	public void 			SetEnabled(bool enabled)
	{
		MoveableController.enabled = enabled;
	}
}
