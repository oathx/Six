using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class LogicHelper
{
	/// <summary>
	/// Gets the error text.
	/// </summary>
	/// <returns>The error text.</returns>
	/// <param name="nCode">N code.</param>
	public static string	GetErrorText(int nCode)
	{
		return Tooltip.QueryText(nCode);
	}
	

	/// <summary>
	/// Error the specified nCode.
	/// </summary>
	/// <param name="nCode">N code.</param>
	public static void		Error(int nCode)
	{
		GameEngine.GetSingleton().SendEvent(typeof(IGlobalPlugin).Name, CmdEvent.CreateErrorEvent(nCode));
	}

	/// <summary>
	/// Changes the scene.
	/// </summary>
	/// <returns><c>true</c>, if scene was changed, <c>false</c> otherwise.</returns>
	/// <param name="status">Status.</param>
	/// <param name="nSceneID">N scene I.</param>
	public static bool		ChangeScene(int nSceneID)
	{
		CmdEvent.SceneLoadEventArgs v = new CmdEvent.SceneLoadEventArgs();
		v.SceneID	= nSceneID;
		
		IGlobalPlugin global = GameEngine.GetSingleton().QueryPlugin<IGlobalPlugin>();
		if (global)
		{
			global.PostEvent(
				new IEvent(EngineEventType.EVENT_GLOBAL, CmdEvent.CMD_LOGIC_LOADSCENE, v)
				);
		}

		return true;
	}
}
