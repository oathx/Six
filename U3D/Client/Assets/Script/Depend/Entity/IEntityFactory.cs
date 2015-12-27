using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// I entity factory.
/// </summary>
public abstract class IEntityFactory
{
	protected IResourceManager	m_ResourceManager;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="IEntityShapeFactory"/> class.
	/// </summary>
	public IEntityFactory()
	{
		m_ResourceManager = GameEngine.GetSingleton().QueryPlugin<IResourceManager>();
		if (!m_ResourceManager)
			throw new System.NullReferenceException();
	}

	/// <summary>
	/// Creates the entity.
	/// </summary>
	/// <returns>The entity.</returns>
	/// <param name="ab">Ab.</param>
	/// <param name="nID">N I.</param>
	/// <param name="szName">Size name.</param>
	/// <param name="vPos">V position.</param>
	/// <param name="vScale">V scale.</param>
	/// <param name="vEuler">V euler.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public abstract IEntity	CreateEntity(EntityType type, int nID, string szName,
	                                     Vector3 vPos, Vector3 vScale, Vector3 vEuler, object args, int nStyle, Transform parent);
}


