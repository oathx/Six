using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Login observer.
/// </summary>
public class LoadingObserver : IEventObserver
{
	public UILoading	LoadUI
	{ get; private set; }
	
	/// <summary>
	/// Active this instance.
	/// </summary>
	public override void Active()
	{
		if (!LoadUI)
			LoadUI = UISystem.GetSingleton().LoadWidget<UILoading>(ResourceDef.UI_LOADING);
	}

	/// <summary>
	/// Detive this instance.
	/// </summary>
	public override void Detive()
	{
		if (LoadUI)
			UISystem.GetSingleton().UnloadWidget(ResourceDef.UI_LOADING);
	}
}

