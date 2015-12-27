using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

/// <summary>
/// Dialog text.
/// </summary>
public class DialogText {
	/// <summary>
	/// Gets or sets the index.
	/// </summary>
	/// <value>The index.</value>
	public int 		Index
	{ get; set; }
	
	/// <summary>
	/// Gets or sets the text.
	/// </summary>
	/// <value>The text.</value>
	public string	Text
	{ get; set; }
	
	/// <summary>
	/// Initializes a new instance of the <see cref="UIDialog+DialogText"/> class.
	/// </summary>
	/// <param name="nIndex">N index.</param>
	/// <param name="szName">Size name.</param>
	/// <param name="szText">Size text.</param>
	public DialogText(int nIndex, string szText)
	{
		Index = nIndex; Text = szText;
	}
}

public class DialogAward
{
	public int 		ID
	{ get; set; }

	public string	Icon
	{ get; set; }

	public int 		Count
	{ get; set; }

	public DialogAward(int nID, string szIcon, int nCount)
	{
		ID = nID; Icon = szIcon; Count = nCount;
	}
}

/// <summary>
/// User interface version.
/// </summary>
public class UIDialog : IUIWidget
{
	public const string	UD_TEXT 	= "UD_TEXT";
	public const string UD_CLICK	= "UD_CLICK";
	public const string UD_TITLE	= "UD_TITLE";
	public const string UD_EXP		= "UD_EXP";
	public const string UD_MONEY	= "UD_MONEY";
	public const string UD_AWARD	= "UD_AWARD";
	public const string UD_ITEM		= "UD_ITEM";

	/// <summary>
	/// The dialog text.
	/// </summary>
	public Queue<DialogText>	
		DlgQueue = new Queue<DialogText>();

	/// <summary>
	/// The item award.
	/// </summary>
	public List<UIItem> 
		ItemAward = new List<UIItem>();

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Install(new string[]{
			UD_TEXT,
			UD_CLICK,
			UD_TITLE,
			UD_EXP,
			UD_MONEY,
			UD_AWARD,
			UD_ITEM,
		});
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		RegisterClickEvent(UD_CLICK, OnDialogClicked);
	}

	/// <summary>
	/// Sets the text.
	/// </summary>
	/// <param name="szText">Size text.</param>
	public void 	SetText(string szText)
	{
		SetText(UD_TEXT, szText);
	}

	/// <summary>
	/// Adds the text.
	/// </summary>
	/// <param name="szText">Size text.</param>
	public void 	AddText(int nIndex, string szText)
	{
		DlgQueue.Enqueue(
			new DialogText(nIndex, szText)
			);
	}

	/// <summary>
	/// Clearup this instance.
	/// </summary>
	public void 	Clearup()
	{
		DlgQueue.Clear();
	}

	/// <summary>
	/// Active this instance.
	/// </summary>
	public override bool Show()
	{
		// close award box
		Show(
			UD_AWARD, false
			);

		return base.Show();
	}

	/// <summary>
	/// Detive this instance.
	/// </summary>
	public override void Hide()
	{
		DlgQueue.Clear();

		for(int idx=0; idx<ItemAward.Count; idx++)
		{
			GameObject.Destroy(
				ItemAward[idx].gameObject
				);
		}

		ItemAward.Clear();

		// close award box
		Show(
			UD_AWARD, false
			);

		// call super class hide
		base.Hide();
	}

	/// <summary>
	/// Sets the award.
	/// </summary>
	/// <param name="nExp">N exp.</param>
	/// <param name="nMoney">N money.</param>
	public void 	SetAward(int nExp, int nMoney, List<DialogAward> aryAward)
	{
		SetText(UD_EXP, 	nExp.ToString());
		SetText(UD_MONEY, 	nMoney.ToString());

		Show(
			UD_AWARD, true
			);

		GameObject prefab = Resources.Load<GameObject>(ResourceDef.UI_ITEM);
		if (!prefab)
			throw new System.NullReferenceException();

		foreach(DialogAward award in aryAward)
		{
			UIItem item = AddChild<UIItem>(UD_ITEM, prefab);
			if (item)
			{
				StartCoroutine(
					OnResetAward(item, award)
					);

				ItemAward.Add(item);
			}
		}
	}

	/// <summary>
	/// Raises the reset award event.
	/// </summary>
	/// <param name="award">Award.</param>
	IEnumerator		OnResetAward(UIItem item, DialogAward award)
	{
		yield return new WaitForEndOfFrame();

		item.Icon 	= award.Icon;
		item.Count	= award.Count;
		item.ID		= award.ID;
	}

	/// <summary>
	/// Raises the dialog clicked event.
	/// </summary>
	/// <param name="go">Go.</param>
	/// <param name="eventData">Event data.</param>
	public void 	OnDialogClicked(GameObject go, BaseEventData eventData)
	{
		string 	szText 	= string.Empty;
		int 	nIndex	= 0;
		int 	nCount	= DlgQueue.Count;

		if (DlgQueue.Count > 0)
		{			
			DialogText dlg = DlgQueue.Dequeue();

			szText = dlg.Text;
			nIndex = dlg.Index;

			SetText(szText);
		}

		// send secne dialog scene event 
		CmdEvent.UIDialogTextEventArgs v = new CmdEvent.UIDialogTextEventArgs();
		v.Text 		= szText;
		v.Index		= nIndex;
		v.Count		= nCount;
		v.Widget	= this;
		
		GameEngine.GetSingleton().SendEvent(
			new IEvent(EngineEventType.EVENT_UI, CmdEvent.CMD_UI_DIALOG, v)
			);

		SetVisible(nCount == 0 ? false : true);

	}
}
