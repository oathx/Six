using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// User interface version.
/// </summary>
public class UICharList : IUIWidget
{
	public const string	UC_GRID	= "UC_GRID";
	public const string	UC_JOIN	= "UC_JOIN";

	/// <summary>
	/// Gets the grid.
	/// </summary>
	/// <value>The grid.</value>
	public Dictionary<int, UICharItem> 
		GridList = new Dictionary<int, UICharItem>();

	/// <summary>
	/// Awakw this instance.
	/// </summary>
	void Awake()
	{
		Install(new string[]{
			UC_GRID,
			UC_JOIN,
		});
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		RegisterClickEvent(UC_JOIN, OnJoinCliceked);
	}

	/// <summary>
	/// Adds the child.
	/// </summary>
	/// <returns>The child.</returns>
	/// <param name="nID">N I.</param>
	/// <param name="szResourceName">Size resource name.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public UICharItem		AddItem(int nID)
	{
		if (GridList.ContainsKey(nID))
			return GridList[nID];

		GameObject prefab = Resources.Load<GameObject>(ResourceDef.UI_CHARITEM);
		if (!prefab)
			throw new System.NullReferenceException();
	
		UICharItem item = AddChild<UICharItem>(UC_GRID, prefab);
		if (!item)
			throw new System.NullReferenceException();

		RegisterClickEvent(
			item.gameObject, OnItemClicked
			);

		item.ID = nID;

		GridList.Add(nID, item);

		return item;
	}

	/// <summary>
	/// Plaies the tween.
	/// </summary>
	public void 	RotateTween(Vector3 add, float fDuration)
	{
		List<UICharItem> aryList = new List<UICharItem>(GridList.Values);
		
		//iTween.Stop();
		for(int i=0; i<aryList.Count; i++)
		{
			iTween.RotateAdd(aryList[i].gameObject, add, (i + 1) * fDuration);
		}
	}

	/// <summary>
	/// Rotates the tween.
	/// </summary>
	public void 	RotateTween()
	{
		RotateTween(new Vector3(360, 0, 0), 0.5f);
	}

	/// <summary>
	/// Select the specified nID.
	/// </summary>
	/// <param name="nID">N I.</param>
	public void 	SetSelected(int nID)
	{
		foreach(KeyValuePair<int, UICharItem> widget in GridList)
		{
			UICharItem item = (UICharItem)widget.Value;
			if (item)
			{
				item.Select = item.ID == nID ? false : true;
			}
		}
	}

	/// <summary>
	/// Gets the selected.
	/// </summary>
	/// <returns>The selected.</returns>
	public int 			GetSelected()
	{
		foreach(KeyValuePair<int, UICharItem> widget in GridList)
		{
			UICharItem item = (UICharItem)widget.Value;
			if (item && !item.Select)
				return item.ID;
		}

		return 0;
	}

	/// <summary>
	/// Gets the select item.
	/// </summary>
	/// <returns>The select item.</returns>
	public UICharItem 	GetSelectItem()
	{
		foreach(KeyValuePair<int, UICharItem> widget in GridList)
		{
			UICharItem item = (UICharItem)widget.Value;
			if (item && !item.Select)
				return item;
		}
		
		return default(UICharItem);
	}

	/// <summary>
	/// Raises the item clicked event.
	/// </summary>
	/// <param name="go">Go.</param>
	/// <param name="eventData">Event data.</param>
	protected void 	OnItemClicked(GameObject go, BaseEventData eventData)
	{
		UICharItem item = go.GetComponent<UICharItem>();
		if (item)
		{
			SetSelected(item.ID);
		}
	}

	/// <summary>
	/// Raises the join cliceked event.
	/// </summary>
	/// <param name="go">Go.</param>
	/// <param name="eventData">Event data.</param>
	protected void 	OnJoinCliceked(GameObject go, BaseEventData eventData)
	{
		CmdEvent.UIJoinEventArgs v = new CmdEvent.UIJoinEventArgs();
		v.PlayerID 	= GetSelected();
		v.Widget	= this;

		GameEngine.GetSingleton().SendEvent(
			new IEvent(EngineEventType.EVENT_UI, CmdEvent.CMD_UI_JOIN, v)
			);
	}
}