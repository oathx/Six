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
	/// The m_d entity.
	/// </summary>
	protected Dictionary<int, IEntity> m_dEntity = new Dictionary<int, IEntity>();

	/// <summary>
	/// Install this instance.
	/// </summary>
	public override void 	Install()
	{
		
	}
	
	/// <summary>
	/// Unstall this instance.
	/// </summary>
	public override void 	Uninstall()
	{

	}
	
	/// <summary>
	/// Startup this instance.
	/// </summary>
	public override void 	Startup()
	{
		
	}
	
	/// <summary>
	/// Shutdown this instance.
	/// </summary>
	public override void 	Shutdown()
	{
	
	}

	/// <summary>
	/// Registers the entity factory.
	/// </summary>
	/// <returns><c>true</c>, if entity factory was registered, <c>false</c> otherwise.</returns>
	/// <param name="szTypeName">Size type name.</param>
	/// <param name="factory">Factory.</param>
	public virtual void		RegisterEntityFactory(string szFactoryName, IEntityFactory factory)
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

	/// <summary>
	/// Creates the entity.
	/// </summary>
	/// <returns>The entity.</returns>
	/// <param name="szFactoryName">Size factory name.</param>
	/// <param name="type">Type.</param>
	/// <param name="nID">N I.</param>
	/// <param name="szName">Size name.</param>
	/// <param name="vPos">V position.</param>
	/// <param name="vScale">V scale.</param>
	/// <param name="vEuler">V euler.</param>
	/// <param name="nStyle">N style.</param>
	/// <param name="args">Arguments.</param>
	public virtual IEntity	CreateEntity(string szFactoryName, EntityType type, int nID, string szName, 
	                                    Vector3 vPos, Vector3 vScale, Vector3 vEuler, int nStyle, object args)
	{
		if (m_dEntity.ContainsKey(nID))
			return m_dEntity[nID];

		// get entity factory
		IEntityFactory factory = GetEntityFactory(szFactoryName);

		// create entity object
		IEntity entity = factory.CreateEntity(type, nID, szName, vPos, vScale, vEuler, args, nStyle, transform);
		if (!entity)
			throw new System.NullReferenceException("Can't create entity " + nID + " name " + szName);

		// add the entity to mananger
		m_dEntity.Add(nID, entity);

		return entity;
	}

	/// <summary>
	/// Destroies the entity.
	/// </summary>
	/// <param name="entity">Entity.</param>
	public virtual void 	DestroyEntity(IEntity entity)
	{

	}
}
