using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// I entity manager.
/// </summary>
public class IEntityManager : IGamePlugin
{
	/// <summary>
	/// The m_d factory.
	/// </summary>
	protected Dictionary<string, 
		IEntityFactory> m_dFactory = new Dictionary<string, IEntityFactory> ();

	/// <summary>
	/// Install this instance.
	/// </summary>
	public override void 		Install()
	{
		
	}
	
	/// <summary>
	/// Unstall this instance.
	/// </summary>
	public override void 		Uninstall()
	{

	}
	
	/// <summary>
	/// Startup this instance.
	/// </summary>
	public override void 		Startup()
	{
		
	}
	
	/// <summary>
	/// Shutdown this instance.
	/// </summary>
	public override void 		Shutdown()
	{
	
	}

	/// <summary>
	/// Registers the entity factory.
	/// </summary>
	/// <returns><c>true</c>, if entity factory was registered, <c>false</c> otherwise.</returns>
	/// <param name="szTypeName">Size type name.</param>
	/// <param name="factory">Factory.</param>
	public virtual void			RegisterEntityFactory(string szFactoryName, IEntityFactory factory)
	{
		if (!m_dFactory.ContainsKey(szFactoryName))
		{
			m_dFactory.Add(szFactoryName, factory);
		}
	}

	/// <summary>
	/// Gets the entity factory.
	/// </summary>
	/// <returns>The entity factory.</returns>
	/// <param name="szFactoryName">Size factory name.</param>
	public IEntityFactory	GetEntityFactory(string szFactoryName)
	{
		if (!m_dFactory.ContainsKey (szFactoryName))
			throw new System.NullReferenceException ("Can't find entity factory " + szFactoryName);
		
		return m_dFactory [szFactoryName];
	}
	
	/// <summary>
	/// Unregisters the factory.
	/// </summary>
	/// <param name="szFactoryName">Size factory name.</param>
	public void 			UnregisterFactory(string szFactoryName)
	{
		if (m_dFactory.ContainsKey(szFactoryName))
		{
			m_dFactory.Remove(szFactoryName);
		}
	}
}
