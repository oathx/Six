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
	public static string		GetErrorText(int nCode)
	{
		return Tooltip.QueryText(nCode);
	}

	/// <summary>
	/// Gets the name.
	/// </summary>
	/// <returns>The name.</returns>
	/// <param name="szPath">Size path.</param>
	public static string		GetFileName(string szPath)
	{
		int nStart 	= szPath.LastIndexOf('/');
		int nEnd	= szPath.LastIndexOf('.');
		
		return szPath.Substring(nStart + 1, nEnd - nStart - 1);
	}

	/// <summary>
	/// Error the specified nCode.
	/// </summary>
	/// <param name="nCode">N code.</param>
	public static void			Error(int nCode)
	{
		GameEngine.GetSingleton().SendEvent(typeof(IGlobalPlugin).Name, CmdEvent.CreateErrorEvent(nCode));
	}

	/// <summary>
	/// Changes the scene.
	/// </summary>
	/// <returns><c>true</c>, if scene was changed, <c>false</c> otherwise.</returns>
	/// <param name="status">Status.</param>
	/// <param name="nSceneID">N scene I.</param>
	public static bool			ChangeScene(int nSceneID)
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

	/// <summary>
	/// Active the specified szObserverName.
	/// </summary>
	/// <param name="szObserverName">Size observer name.</param>
	public static void			Active(string szObserverName)
	{
		LogicPlugin plugin = GameEngine.GetSingleton().QueryPlugin<LogicPlugin> ();
		if (plugin)
		{
			IEventObserver observer = plugin.QueryObserver(szObserverName);
			if (observer)
				observer.Active();
		}
	}

	/// <summary>
	/// Detive the specified szObserverName.
	/// </summary>
	/// <param name="szObserverName">Size observer name.</param>
	public static void 			Detive(string szObserverName)
	{
		LogicPlugin plugin = GameEngine.GetSingleton().QueryPlugin<LogicPlugin> ();
		if (plugin)
		{
			IEventObserver observer = plugin.QueryObserver(szObserverName);
			if (observer)
				observer.Detive();
		}
	}

	/// <summary>
	/// Gets the main player.
	/// </summary>
	/// <returns>The main player.</returns>
	public static PlayerEntity	GetMainPlayer()
	{
		PlayerManager mgr = GameEngine.GetSingleton().QueryPlugin<PlayerManager>();
		if (!mgr)
			throw new System.NullReferenceException();

		return mgr.GetPlayer();
	}

	/// <summary>
	/// Gets the monster.
	/// </summary>
	/// <returns>The monster.</returns>
	/// <param name="nMonsterID">N monster I.</param>
	public static MonsterEntity	GetMonster(int nMonsterID)
	{
		MonsterManager mgr = GameEngine.GetSingleton().QueryPlugin<MonsterManager>();
		if (!mgr)
			throw new System.NullReferenceException();
		
		return mgr.GetEntity(nMonsterID) as MonsterEntity;
	}
}
