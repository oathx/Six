using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Login observer.
/// </summary>
public class CharacterObserver : RuntimeObserver
{
	/// <summary>
	/// Awake this instance.
	/// </summary>
	protected override void Awake()
	{
		base.Awake();

		m_Logic.RegisterPackageFactory(TcpEvent.CMD_PUSH_ENTER_WORLD_SUCCESS, 
		                               new DefaultNetMessageFactory<TcpEvent.SCNetCharacterSpawnInfo>());
		m_Logic.RegisterPackageFactory(TcpEvent.CMD_PUSH_CREATE_ROLE,
		                               new DefaultNetMessageFactory<TcpEvent.SCNetCreateRole>());


	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	protected override void Start()
	{
		// register net event
		SubscribeEvent (TcpEvent.CMD_PUSH_ENTER_WORLD_SUCCESS, 	
		                OnEnterWorldSuccess);
		SubscribeEvent (TcpEvent.CMD_PUSH_CREATE_ROLE,   		
		                OnCreateRoleSuccess);

//		 subscribe local gui event
		SubscribeEvent(CmdEvent.CMD_UI_JOIN, OnJoinClicked);
	}

	/// <summary>
	/// Active this instance.
	/// </summary>
	public override void Active()
	{
		base.Active();

		List<TcpEvent.CharacterInfo> aryCharInfo = CharacterTable.GetSingleton().ToArray();
		if (aryCharInfo.Count > 0)
		{
			// create character list
			UICharList charUI = UISystem.GetSingleton().LoadWidget<UICharList>(ResourceDef.UI_CHARLIST);
			if (!charUI)
				throw new System.NullReferenceException();

			// add current all character item
			for(int i=0; i<aryCharInfo.Count; i++)
			{
				UICharItem item = charUI.AddItem(aryCharInfo[i].PlayerID);
				if (item)
				{
					item.ID		= aryCharInfo[i].PlayerID;
					item.Level	= aryCharInfo[i].Rank;
					item.Name	= aryCharInfo[i].Name;
					item.Select	= i == 0 ? false : true;
				}
			}
		
			// play list rotate animation
			charUI.RotateTween();
		}
		else
		{

		}
	}
	
	/// <summary>
	/// Detive this instance.
	/// </summary>
	public override void Detive()
	{
		string[] aryName = {
			ResourceDef.UI_CHARLIST,
		};

		foreach(string widget in aryName)
		{
			UISystem.GetSingleton().UnloadWidget(widget);
		}

		base.Detive();
	}
	
	/// <summary>
	/// Raises the join clicked event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool		OnJoinClicked(IEvent evt)
	{
		CmdEvent.UIJoinEventArgs v = evt.Args as CmdEvent.UIJoinEventArgs;
		if (v.PlayerID != 0)
		{
			CharacterRequest.GetSingleton().RequestEnterWorld(v.PlayerID);
		}
		
		return true;
	}

	/// <summary>
	/// Raises the enter world success event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool		OnEnterWorldSuccess(IEvent evt)
	{
		TcpEvent.SCNetCharacterSpawnInfo spawn = evt.Args as TcpEvent.SCNetCharacterSpawnInfo;

		GlobalUserInfo.Position = spawn.Position;
		GlobalUserInfo.Level 	= spawn.Level;
		GlobalUserInfo.MapID	= spawn.MapID;
		GlobalUserInfo.Job 		= spawn.Job;
		GlobalUserInfo.Name 	= spawn.Name;
		GlobalUserInfo.PlayerID = spawn.PlayerID;

		SqlScene sqlScene = GameSqlLite.GetSingleton ().Query<SqlScene> (spawn.MapID);
		if (sqlScene)
		{
			LogicHelper.ChangeScene(spawn.MapID);
		}

		return true;
	}

	/// <summary>
	/// Raises the create role success event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool		OnCreateRoleSuccess(IEvent evt)
	{
		return true;
	}

}


