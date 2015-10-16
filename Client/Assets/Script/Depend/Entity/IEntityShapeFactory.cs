using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void	CreateShapeFactoryCallback(IEntityShape entityShape);

/// <summary>
/// I entity shape factory.
/// </summary>
public abstract class IEntityShapeFactory
{
	protected IResourceManager		m_ResourceManager;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="IEntityShapeFactory"/> class.
	/// </summary>
	public IEntityShapeFactory()
	{
		m_ResourceManager = GameEngine.GetSingleton().QueryPlugin<IResourceManager>();
		if (!m_ResourceManager)
			throw new System.NullReferenceException();
	}
	
	/// <summary>
	/// Creates the shape.
	/// </summary>
	/// <returns>The shape.</returns>
	public abstract void	CreateShape(int nShapeID, CreateShapeFactoryCallback callback);
}


/// <summary>
/// Default shape factory.
/// </summary>
public class DefaultShapeFactory : IEntityShapeFactory
{
	/// <summary>
	/// Creates the shape.
	/// </summary>
	/// <returns>The shape.</returns>
	/// <param name="nShapeID">N shape I.</param>
	public override void	CreateShape(int nShapeID, CreateShapeFactoryCallback callback)
	{
		SqlShape sqlShape = GameSqlLite.GetSingleton().Query<SqlShape>(nShapeID);
		if (!sqlShape)
			throw new System.NullReferenceException();
		
		m_ResourceManager.LoadFromFile(sqlShape.Skeleton,
		                               delegate(string szUrl, AssetBundle abFile) {
			
			GameObject resource = abFile.LoadAsset(sqlShape.Skeleton, typeof(GameObject)) as GameObject;
			if (!resource)
				throw new System.NullReferenceException();
			
			GameObject shape = GameObject.Instantiate(resource) as GameObject;
			if (shape)
			{
				shape.transform.position 			= Vector3.zero;
				shape.transform.localScale			= Vector3.one;
				shape.transform.localEulerAngles 	= Vector3.zero;
				
				IEntityShape entityShape = shape.AddComponent<IEntityShape>();
				if (!entityShape)
					throw new System.NullReferenceException();
				
				callback(entityShape);
			}
			
			return true;
		});
	}
}

/// <summary>
/// Entity shape factory manager.
/// </summary>
public class EntityShapeFactoryManager : SimpleSingleton<EntityShapeFactoryManager>
{
	protected Dictionary<string, IEntityShapeFactory> 
		m_dFactory = new Dictionary<string, IEntityShapeFactory>();
	
	/// <summary>
	/// Registers the factory.
	/// </summary>
	/// <param name="factory">Factory.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public void					RegisterFactory<T>(T factory) where T : IEntityShapeFactory
	{
		if (!m_dFactory.ContainsKey(typeof(T).Name))
		{
			m_dFactory.Add(typeof(T).Name, factory);
		}
	}
	
	/// <summary>
	/// Gets the shape factory.
	/// </summary>
	/// <returns>The shape factory.</returns>
	/// <param name="szFactoryName">Size factory name.</param>
	public IEntityShapeFactory	GetShapeFactory(string szFactoryName)
	{
		return m_dFactory[szFactoryName];
	}
	
	/// <summary>
	/// Unregisters the factory.
	/// </summary>
	/// <param name="szFactoryName">Size factory name.</param>
	public void 				UnregisterFactory(string szFactoryName)
	{
		if (m_dFactory.ContainsKey(szFactoryName))
		{
			m_dFactory.Remove(szFactoryName);
		}
	}
}
