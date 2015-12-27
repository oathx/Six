using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Login observer.
/// </summary>
public class ModleObserver : IEventObserver
{
	/// <summary>
	/// Gets the modle U.
	/// </summary>
	/// <value>The modle U.</value>
	public UIModle			ModleUI
	{ get; private set; }

	/// <summary>
	/// Active this instance.
	/// </summary>
	public override void 	Active()
	{
		ModleUI = UISystem.GetSingleton().LoadWidget<UIModle>(ResourceDef.UI_MODLE);
		if (!ModleUI)
			throw new System.NullReferenceException();

		InstallSystem();
	}

	/// <summary>
	/// Detive this instance.
	/// </summary>
	public override void 	Detive()
	{
		UISystem.GetSingleton().UnloadWidget(ResourceDef.UI_MODLE);
	}

	/// <summary>
	/// Installs the system.
	/// </summary>
	public virtual void 	InstallSystem()
	{
		List<SqlSpread> arySpread = GameSqlLite.GetSingleton().QueryTable<SqlSpread>();
		foreach(SqlSpread spread in arySpread)
		{
			if (spread.State != 0 && !string.IsNullOrEmpty(spread.Observer))
			{
				LoadSystem(spread);
			}
		}
	}

	/// <summary>
	/// Loads the system.
	/// </summary>
	/// <param name="sqlSpread">Sql spread.</param>
	public virtual bool 	LoadSystem(SqlSpread sqlSpread)
	{
		if (!string.IsNullOrEmpty(sqlSpread.Icon))
		{
			GameObject prefab = Resources.Load<GameObject>(sqlSpread.Icon);
			if (prefab)
			{
				ModleUI.Load(prefab, sqlSpread.ID, ModleAlignmentStyle.MAS_RIGHTUP);
			}
		}

		IEventObserver observer = Dispatcher.LoadObserver(sqlSpread.Observer);
		if (observer)
			observer.Active();

		return true;
	}
}
