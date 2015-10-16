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
		SubscribeEvent(CmdEvt.CMD_ERROR, OnSystemError);
	}

	/// <summary>
	/// Raises the system error event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool OnSystemError(IEvent evt)
	{
		CmdEvt.ErrorEventArgs v = evt.Args as CmdEvt.ErrorEventArgs;

		SqlTooltip tooltip = GameSqlLite.GetSingleton().Query<SqlTooltip>(v.Code);
		if (tooltip)
		{
			UISystem.GetSingleton().Box(tooltip.Text);
		}

		return true;
	}
}

