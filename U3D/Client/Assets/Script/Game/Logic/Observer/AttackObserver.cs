using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Joystick observer.
/// </summary>
public class AttackObserver : IEventObserver
{
	protected MonsterManager	m_MonsterManager;
	protected PlayerManager		m_PlayerManagr;
	
	/// <summary>
	/// The successive.
	/// </summary>
	public List<int>	
		Successive = new List<int>();

	/// <summary>
	/// The current magic I.
	/// </summary>
	public int CurrentMagicID = int.MinValue;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		m_MonsterManager 	= GameEngine.GetSingleton().QueryPlugin<MonsterManager>();
		m_PlayerManagr		= GameEngine.GetSingleton().QueryPlugin<PlayerManager>();
	}

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Start()
	{
		SubscribeEvent(CmdEvent.CMD_UI_ATTACK, OnAttackCliecked);
	}

	/// <summary>
	/// Gets the attack U.
	/// </summary>
	/// <value>The attack U.</value>
	public UIAttack			AttackUI 
	{ get; private set; }

	/// <summary>
	/// Active this instance.
	/// </summary>
	public override void Active ()
	{
		if (!AttackUI)
			AttackUI = UISystem.GetSingleton().LoadWidget<UIAttack>(ResourceDef.UI_ATTACK);

		AttackUI.NormalMagicID = 11001;
	}

	/// <summary>
	/// Detive this instance.
	/// </summary>
	public override void Detive ()
	{
		UISystem.GetSingleton().UnloadWidget(ResourceDef.UI_ATTACK);
	}
	
	/// <summary>
	/// Raises the skill cliecked event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool		OnAttackCliecked(IEvent evt)
	{
		CmdEvent.UIAttackEventArgs v = evt.Args as CmdEvent.UIAttackEventArgs;
		if (v.MagicID != 0)
		{

		}

		return true;
	}
}
