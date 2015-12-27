using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

public enum AttackType {
	AT_NORMAL, AT_MAGIC
}

/// <summary>
/// User interface version.
/// </summary>
public class UIAttack : IUIWidget
{
	public const string UA_ATTACK = "UA_ATTACK";
	
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Install(new string[]{
			UA_ATTACK
		});
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		RegisterClickEvent(UA_ATTACK, OnAttackClicked);
	}

	/// <summary>
	/// Gets or sets the normal magic I.
	/// </summary>
	/// <value>The normal magic I.</value>
	public int 		NormalMagicID { get; set; }

	/// <summary>
	/// Raises the attack clicked event.
	/// </summary>
	/// <param name="goSend">Go send.</param>
	/// <param name="evtData">Evt data.</param>
	protected void 	OnAttackClicked(GameObject goSend, BaseEventData evtData)
	{
		CmdEvent.UIAttackEventArgs v = new CmdEvent.UIAttackEventArgs();
		v.MagicID 	= NormalMagicID;
		v.Type		= AttackType.AT_NORMAL;

		GameEngine.GetSingleton().SendEvent(
			new IEvent(EngineEventType.EVENT_UI, CmdEvent.CMD_UI_ATTACK, v)
			);
	}
}
