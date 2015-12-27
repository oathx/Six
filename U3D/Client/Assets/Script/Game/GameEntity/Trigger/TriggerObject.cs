using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TriggerType
{
	TRIGGER_NULL,
	TRIGGER_ANIMATION,
	TRIGGER_TRANS,
}

/// <summary>
/// Trigger entity.
/// </summary>
public class TriggerObject : MonoBehaviour
{
	public int 				ID		= 0;
	public TriggerType		Type 	= TriggerType.TRIGGER_NULL;
	public bool				Once 	= false;
	public bool				IsStay	= false;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		BoxCollider collider = GetComponent<BoxCollider>();
		if (!collider)
			collider = gameObject.AddComponent<BoxCollider>();

		collider.isTrigger = true;
	}

	/// <summary>
	/// Raises the trigger enter event.
	/// </summary>
	/// <param name="collider">Collider.</param>
	protected virtual void 	OnTriggerEnter(Collider collider)
	{
		if (!IsStay)
		{
			gameObject.SetActive(!Once);

			CmdEvent.SceneTriggerEventArgs v = new CmdEvent.SceneTriggerEventArgs();
			v.ID 		= ID;
			v.Handle	= this;
			v.EventID	= 0;
			v.Enter		= true;
			v.Who		= collider;
			
			GameEngine.GetSingleton().PostEvent(
				new IEvent(EngineEventType.EVENT_USER, CmdEvent.CMD_SCENE_TRIGGER, v)
				);
		}
	}

	/// <summary>
	/// Raises the trigger stay event.
	/// </summary>
	/// <param name="collider">Collider.</param>
	protected virtual void OnTriggerStay(Collider collider)
	{
		IsStay = true;
	}	
	
	/// <summary>
	/// Raises the trigger exit event.
	/// </summary>
	/// <param name="collider">Collider.</param>
	protected virtual void 	OnTriggerExit(Collider collider)
	{
		CmdEvent.SceneTriggerEventArgs v = new CmdEvent.SceneTriggerEventArgs();
		v.ID 		= ID;
		v.Handle	= this;
		v.EventID	= 0;
		v.Enter		= false;
		v.Who	= collider;

		GameEngine.GetSingleton().PostEvent(
			new IEvent(EngineEventType.EVENT_USER, CmdEvent.CMD_SCENE_TRIGGER, v)
			);

		IsStay = false;
	}
}
