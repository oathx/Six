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

		// subscribe local gui event
		SubscribeEvent(CmdEvent.CMD_UI_JOIN, 		OnJoinClicked);
		SubscribeEvent(CmdEvent.CMD_UI_SELECTJOB, 	OnSelectJob);
		SubscribeEvent(CmdEvent.CMD_UI_CREATEROLE,	OnCreateRole);
		SubscribeEvent(CmdEvent.CMD_UI_CREATERAND,	OnCreateRand);
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
			for(int idx=0; idx<aryCharInfo.Count; idx++)
			{
				UICharItem item = charUI.AddItem(aryCharInfo[idx].PlayerID);
				if (item)
				{
					item.ID		= aryCharInfo[idx].PlayerID;
					item.Level	= aryCharInfo[idx].Rank;
					item.Name	= aryCharInfo[idx].Name;
					item.Select	= idx == 0 ? false : true;
					item.Job	= aryCharInfo[idx].Job;
				}
			}
		
			// play list rotate animation
			charUI.RotateTween();
		}
		else
		{
			UICharacter charUI = UISystem.GetSingleton().LoadWidget<UICharacter>(ResourceDef.UI_CHARACTER);
			if (!charUI)
				throw new System.NullReferenceException();
		}
	}

	/// <summary>
	/// Detive this instance.
	/// </summary>
	public override void Detive()
	{
		string[] aryName = {
			ResourceDef.UI_CHARLIST, ResourceDef.UI_CHARACTER
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

		// requeset enter game world
		CharacterRequest.GetSingleton().RequestEnterWorld(v.PlayerID);

		return true;
	}

	/// <summary>
	/// Raises the select job event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool		OnSelectJob(IEvent evt)
	{
		CmdEvent.UISelectJobEventArgs v = evt.Args  as CmdEvent.UISelectJobEventArgs;
		if (v.Index != 0)
		{
			SqlJob sqlJob = GameSqlLite.GetSingleton().Query<SqlJob>(v.Index);
			if (!sqlJob)
				throw new System.NullReferenceException();

			UICharacter charUI = v.Widget as UICharacter;
			charUI.SetDescribe(sqlJob);
		}

		return true;
	}

	/// <summary>
	/// Raises the create role event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool		OnCreateRole(IEvent evt)
	{
		CmdEvent.UICreateRoleEventArgs v = evt.Args as CmdEvent.UICreateRoleEventArgs;
		
		// requeset create character
		CharacterRequest.GetSingleton ().RequestCreateRole (v.Name, (sbyte)(v.ID));

		return true;
	}

	/// <summary>
	/// Raises the create rand event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool		OnCreateRand(IEvent evt)
	{
		CmdEvent.UIClickEventArgs v = evt.Args as CmdEvent.UIClickEventArgs;
		if (v.Widget)
		{
			string szName = NameTable.GetSingleton ().GetName ();
			v.Widget.SetInputText(UICharacter.UC_NAME, szName);
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
			GlobalUserInfo.City = sqlScene.Type;

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
		TcpEvent.SCNetCreateRole v = evt.Args as TcpEvent.SCNetCreateRole;
		
#if OPEN_DEBUG_LOG
		Debug.Log("Create role succes " + v.PlayerID);
#endif
		
		GlobalUserInfo.PlayerID = v.PlayerID;
		GlobalUserInfo.Job 		= v.Occupation;
		GlobalUserInfo.Name 	= v.PlayerName;
		
		// requeset enter game world
		CharacterRequest.GetSingleton ().RequestEnterWorld (v.PlayerID);

		return true;
	}

}


