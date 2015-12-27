using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// User interface version.
/// </summary>
public class UICharacter : IUIWidget
{
	public const string UC_JOB		= "UC_JOB";
	public const string UC_CREATE	= "UC_CREATE";
	public const string UC_REMOTE	= "UC_REMOTE";
	public const string UC_HEAVY	= "UC_HEAVY";
	public const string UC_ASSAULT	= "UC_ASSAULT";
	public const string UC_NAME		= "UC_NAME";
	public const string UC_RAND		= "UC_RAND";
	public const string UC_JOBNAME	= "UC_JOBNAME";
	public const string UC_JOBDESC	= "UC_JOBDESC";

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Install(new string[]{
			UC_JOB,
			UC_CREATE,
			UC_REMOTE,
			UC_HEAVY,
			UC_ASSAULT,
			UC_NAME,
			UC_RAND,
			UC_JOBNAME,
			UC_JOBDESC,
		});
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		string[] aryName = {
			UC_ASSAULT, UC_HEAVY, UC_REMOTE
		};
		foreach(string cur in aryName)
		{
			RegisterClickEvent (cur, OnSelectClicked);
		}
	
		RegisterClickEvent(UC_CREATE, 	OnCreateClicked);
		RegisterClickEvent(UC_RAND, 	OnRandClicked);

		// default select assult
		Select(
			UC_ASSAULT
			);
	}

	/// <summary>
	/// Sets the describe.
	/// </summary>
	/// <param name="job">Job.</param>
	public void 	SetDescribe(SqlJob job)
	{
		SetText(UC_JOBNAME, job.Name);
		SetText(UC_JOBDESC, job.Describe);
	}

	/// <summary>
	/// Select the specified szName.
	/// </summary>
	/// <param name="szName">Size name.</param>
	public void 	Select(string szName)
	{
		string[] aryName = {
			UC_ASSAULT, UC_HEAVY, UC_REMOTE
		};

		for(int idx=0; idx<aryName.Length; idx++)
		{
			Button button = GetChildComponent<Button>(aryName[idx]);
			if (button)
			{
				button.interactable = (aryName[idx] == szName ? false : true);

				if (aryName[idx] == szName)
				{
					CmdEvent.UISelectJobEventArgs evt = new CmdEvent.UISelectJobEventArgs();
					evt.Widget 	= this;
					evt.Index	= idx + 1;
					
					GameEngine.GetSingleton().SendEvent(
						new IEvent(EngineEventType.EVENT_UI, CmdEvent.CMD_UI_SELECTJOB, evt)
						);
				}
			}
		}
	}

	/// <summary>
	/// Select the specified idx.
	/// </summary>
	/// <param name="idx">Index.</param>
	public void 	Select(int idx)
	{
		string[] aryName = {
			UC_ASSAULT, UC_HEAVY, UC_REMOTE
		};

		if (idx >= 0 && idx<aryName.Length)
			Select (aryName [idx]);
	}

	/// <summary>
	/// Gets the select.
	/// </summary>
	/// <returns>The select.</returns>
	public int 		GetSelect()
	{
		string[] aryName = {
			UC_ASSAULT, UC_HEAVY, UC_REMOTE
		};
		for(int idx=0; idx<aryName.Length; idx++)
		{
			Button button = GetChildComponent<Button>(aryName[idx]);
			if (button.interactable)
				return idx;
		}

		return 0;
	}

	/// <summary>
	/// Raises the select clicked event.
	/// </summary>
	/// <param name="go">Go.</param>
	/// <param name="evtData">Evt data.</param>
	protected void	OnSelectClicked(GameObject go, BaseEventData evtData)
	{
		Select(go.name);
	}

	/// <summary>
	/// Raises the create clicked event.
	/// </summary>
	/// <param name="go">Go.</param>
	/// <param name="evtData">Evt data.</param>
	protected void 	OnCreateClicked(GameObject go, BaseEventData evtData)
	{
		string szUserName = GetInputText(UC_NAME);
		if (!string.IsNullOrEmpty(szUserName))
		{
			CmdEvent.UICreateRoleEventArgs evt = new CmdEvent.UICreateRoleEventArgs();
			evt.Widget 	= this;
			evt.Name	= szUserName;
			evt.ID		= GetSelect();

			GameEngine.GetSingleton().SendEvent(
				new IEvent(EngineEventType.EVENT_UI, CmdEvent.CMD_UI_CREATEROLE, evt)
				);
		}
		else
		{

		}
	}

	/// <summary>
	/// Raises the rand clicked event.
	/// </summary>
	/// <param name="go">Go.</param>
	/// <param name="evtData">Evt data.</param>
	protected void 	OnRandClicked(GameObject go, BaseEventData evtData)
	{
		CmdEvent.UIClickEventArgs evt = new CmdEvent.UIClickEventArgs();
		evt.Widget 	= this;
		
		GameEngine.GetSingleton().SendEvent(
			new IEvent(EngineEventType.EVENT_UI, CmdEvent.CMD_UI_CREATERAND, evt)
			);
	}
}
