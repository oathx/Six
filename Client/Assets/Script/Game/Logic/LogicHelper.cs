using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LogicHelper
{
	/// <summary>
	/// Gets the error text.
	/// </summary>
	/// <returns>The error text.</returns>
	/// <param name="nCode">N code.</param>
	public static string	GetErrorText(int nCode)
	{
		SqlTooltip tooltip = GameSqlLite.GetSingleton().Query<SqlTooltip>(nCode);
		if (tooltip)
			return tooltip.Text;

		return string.Empty;
	}

	/// <summary>
	/// Error the specified nCode.
	/// </summary>
	/// <param name="nCode">N code.</param>
	public static void		Error(int nCode)
	{
		GameEngine.GetSingleton().SendEvent(typeof(IGlobalPlugin).Name, CmdEvt.CreateErrorEvent(nCode));
	}
}
