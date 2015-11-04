using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Login.
/// </summary>
public class Character : MonoBehaviour
{
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{	

	}
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		PlayerManager entityManager = GameEngine.GetSingleton().QueryPlugin<PlayerManager>();
		if (entityManager)
		{
			entityManager.CreateEntity(EntityType.ET_MAIN.ToString(), EntityType.ET_MAIN,
			                           0, string.Empty, Vector3.zero, Vector3.one, Vector3.zero, 0, 10000);
			
		}
	}
	
	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	void OnDestroy ()
	{

	}
}

