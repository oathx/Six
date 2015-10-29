using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Status observer.
/// </summary>
public class GlobalObserver : IEventObserver
{
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		SubscribeEvent(CmdEvent.CMD_ERROR, 			OnSystemError);
	}

	/// <summary>
	/// Raises the system error event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool OnSystemError(IEvent evt)
	{
		CmdEvent.ErrorEventArgs v = evt.Args as CmdEvent.ErrorEventArgs;

		UISystem.GetSingleton().Box(
			Tooltip.QueryText(v.Code)
			);

		return true;
	}	
}

