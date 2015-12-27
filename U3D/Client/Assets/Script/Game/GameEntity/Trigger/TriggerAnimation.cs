using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Trigger event arguments.
/// </summary>
public class TriggerEventArgs : IEventArgs
{
	/// <summary>
	/// Gets or sets the event I.
	/// </summary>
	/// <value>The event I.</value>
	public int 				EventID
	{ get; set; }
}

/// <summary>
/// Trigger entity.
/// </summary>
public class TriggerAnimation : TriggerObject
{
	public GameObject		Target;
	public AnimationClip	Clip;
	public int 				EventID;
	
	/// <summary>
	/// Raises the trigger enter event.
	/// </summary>
	protected override void OnTriggerEnter(Collider collider)
	{
		if (Target)
		{
			Animator animator = Target.GetComponent<Animator>();
			if (animator)
			{
				animator.Play(Clip.name);
			}
		}

		base.OnTriggerEnter(collider);
	}
	
	/// <summary>
	/// Raises the trigger exit event.
	/// </summary>
	/// <param name="collider">Collider.</param>
	protected override void OnTriggerExit(Collider collider)
	{
		
	}
}


