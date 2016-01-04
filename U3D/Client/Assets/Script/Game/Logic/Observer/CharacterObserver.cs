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

		m_Logic.RegisterPackageFactory(TcpEvent.CMD_REPLY_ENTER_WORLD, 
		                               new DefaultNetMessageFactory<TcpEvent.SCNetEnterWorldReply>());
		m_Logic.RegisterPackageFactory(TcpEvent.CMD_REPLY_CREATE_ROLE,
		                               new DefaultNetMessageFactory<TcpEvent.SCNetCreateRoleReply>());
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	protected override void Start()
	{
		// register net event
		SubscribeEvent (TcpEvent.CMD_REPLY_ENTER_WORLD, 	
		                OnNetEnterWorldResult);
		SubscribeEvent (TcpEvent.CMD_REPLY_CREATE_ROLE,   		
		                OnNetCreateRoleResult);

		// subscribe local gui event
		SubscribeEvent(CmdEvent.CMD_UI_JOIN, 		OnUIJoinClicked);
		SubscribeEvent(CmdEvent.CMD_UI_SELECTJOB, 	OnUISelectJob);
		SubscribeEvent(CmdEvent.CMD_UI_CREATEROLE,	OnUICreateRole);
		SubscribeEvent(CmdEvent.CMD_UI_CREATERAND,	OnUICreateRand);
	}

	/// <summary>
	/// Active this instance.
	/// </summary>
	public override void Active()
	{
		base.Active();

		List<TcpEvent.CharacterStruct> aryCharInfo = CharacterTable.GetSingleton().ToArray();
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
	protected bool		OnUIJoinClicked(IEvent evt)
	{
		CmdEvent.UIJoinEventArgs v = evt.Args as CmdEvent.UIJoinEventArgs;

		// requeset enter game world
		LogicRequest.GetSingleton().RequestEnterWorld(v.PlayerID);

		return true;
	}

	/// <summary>
	/// Raises the select job event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool		OnUISelectJob(IEvent evt)
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
	protected bool		OnUICreateRole(IEvent evt)
	{
		CmdEvent.UICreateRoleEventArgs v = evt.Args as CmdEvent.UICreateRoleEventArgs;
		
		// requeset create character
		LogicRequest.GetSingleton ().RequestCreateRole (v.Name, (sbyte)(v.ID));

		return true;
	}

	/// <summary>
	/// Raises the create rand event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool		OnUICreateRand(IEvent evt)
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
	protected bool		OnNetEnterWorldResult(IEvent evt)
	{
		TcpEvent.SCNetEnterWorldReply spawn = evt.Args as TcpEvent.SCNetEnterWorldReply;

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
	protected bool		OnNetCreateRoleResult(IEvent evt)
	{
		TcpEvent.SCNetCreateRoleReply v = evt.Args as TcpEvent.SCNetCreateRoleReply;

		GlobalUserInfo.PlayerID = v.PlayerID;
		GlobalUserInfo.Job 		= v.Occupation;
		GlobalUserInfo.Name 	= v.PlayerName;
		
		// requeset enter game world
		LogicRequest.GetSingleton ().RequestEnterWorld (v.PlayerID);

		return true;
	}

}


