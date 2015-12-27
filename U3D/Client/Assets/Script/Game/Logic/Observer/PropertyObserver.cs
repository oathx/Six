using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Joystick observer.
/// </summary>
public class PropertyObserver : IEventObserver
{
	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		LogicPlugin plugin = GameEngine.GetSingleton ().QueryPlugin<LogicPlugin> ();
		if (plugin)
		{
			plugin.RegisterPackageFactory(TcpEvent.CMD_PUSH_PROPERTY_CHANGE, 
			                              new DefaultNetMessageFactory<TcpEvent.SCNetPropertyChange>());
			plugin.RegisterPackageFactory(TcpEvent.CMD_PUSH_CHAR_ATTRIBUTE,
			                              new DefaultNetMessageFactory<TcpEvent.SCNetProperty>());
		}
	}
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		CharacterRequest.GetSingleton().RequestCharacterPoperty();

		SubscribeEvent (
			TcpEvent.CMD_PUSH_CHAR_ATTRIBUTE,	OnProperty
			);
		// subscribe event
		SubscribeEvent (
			TcpEvent.CMD_PUSH_PROPERTY_CHANGE,	OnPropertyChange
			);
	}

	/// <summary>
	/// Raises the property event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool		OnProperty(IEvent evt)
	{
		TcpEvent.SCNetProperty v = evt.Args as TcpEvent.SCNetProperty;
		return true;
	}
	
	/// <summary>
	/// Raises the property change event.
	/// </summary>
	protected bool 		OnPropertyChange(IEvent evt)
	{
		TcpEvent.SCNetPropertyChange v = evt.Args as TcpEvent.SCNetPropertyChange;

		return true;
	}
}
