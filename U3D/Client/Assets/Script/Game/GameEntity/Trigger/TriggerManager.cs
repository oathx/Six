using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Trigger mananger.
/// </summary>
public class TriggerMananger : SimpleSingleton<TriggerMananger>
{
	protected Dictionary<int, TriggerObject> 
		Trigger = new Dictionary<int, TriggerObject>();

	/// <summary>
	/// Creates the trigger.
	/// </summary>
	/// <returns>The trigger.</returns>
	/// <param name="type">Type.</param>
	/// <param name="nID">N I.</param>
	/// <param name="triggerType">Trigger type.</param>
	/// <param name="vPosition">V position.</param>
	/// <param name="vEuler">V euler.</param>
	/// <param name="vScale">V scale.</param>
	/// <param name="args">Arguments.</param>
	public TriggerObject	CreateTrigger(int nID, TriggerType triggerType, 
	                                   Vector3 vPosition, Vector3 vEuler, Vector3 vScale, bool once)
	{
		if (Trigger.ContainsKey(nID))
			return Trigger[nID];

		GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
		if (!cube)
			throw new System.NullReferenceException();

		cube.transform.position 	= vPosition;
		cube.transform.eulerAngles 	= vEuler;
		cube.transform.localScale	= vScale;
		cube.layer					= EntityLayer.TRIGGER;

		Physics.IgnoreLayerCollision(EntityLayer.TRIGGER, EntityLayer.NPC);
		Physics.IgnoreLayerCollision(EntityLayer.TRIGGER, EntityLayer.MONSTER);

		TriggerObject trigger = (triggerType ==  TriggerType.TRIGGER_NULL ? 
			cube.AddComponent<TriggerObject>() : cube.AddComponent<TriggerAnimation>());
		if (trigger)
		{
			trigger.ID 		= nID;
			trigger.Type	= triggerType;
			trigger.Once	= once;

			Trigger.Add(nID, trigger);
		}

		return trigger;
	}

	/// <summary>
	/// Destroies the trigger.
	/// </summary>
	/// <param name="nID">N I.</param>
	public void 			DestroyTrigger(int nID)
	{
		if (Trigger.ContainsKey(nID))
		{
			GameObject.Destroy(
				Trigger[nID].gameObject
				);

			Trigger.Remove(nID);
		}
	}

	/// <summary>
	/// Finds all trigger.
	/// </summary>
	/// <returns>The all trigger.</returns>
	public TriggerObject[]	GetTriggerArray()
	{
		return GameObject.FindObjectsOfType<TriggerObject>();
	}

	/// <summary>
	/// Finds the trigger.
	/// </summary>
	/// <returns>The trigger.</returns>
	/// <param name="nID">N I.</param>
	public TriggerObject	QueryTrigger(int nID)
	{
		if (!Trigger.ContainsKey(nID))
			throw new System.NullReferenceException();

		return Trigger[nID];
	}
}
