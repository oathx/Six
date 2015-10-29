using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

		// load the shape skeleton object
		m_ResourceManager.LoadAssetBundleFromStream(sqlShape.Skeleton,
		                               delegate(string szAssetName, AssetBundleResource abResource) {
			
			GameObject resource = abResource.GetAsset<GameObject>(sqlShape.Skeleton);
			if (!resource)
				throw new System.NullReferenceException();

			// create skeleton gameobject
			GameObject shape = GameObject.Instantiate(resource) as GameObject;
			if (shape)
			{
				shape.transform.position 			= Vector3.zero;
				shape.transform.localScale			= Vector3.one;
				shape.transform.localEulerAngles 	= Vector3.zero;
				
				IEntityShape entityShape = shape.AddComponent<IEntityShape>();
				if (!entityShape)
					throw new System.NullReferenceException();

				Apparel(sqlShape.Equip, entityShape);

				callback(entityShape);
			}
			
			return true;
		});
	}
	
	/// <summary>
	/// Apparel the specified aryEquip.
	/// </summary>
	/// <param name="aryEquip">Ary equip.</param>
	public virtual void 	Apparel(List<int> aryEquip, IEntityShape entityShape)
	{
		foreach(int equip in aryEquip)
		{
			SqlItem sqlItem = GameSqlLite.GetSingleton().Query<SqlItem>(equip);
			if (!string.IsNullOrEmpty(sqlItem.Url))
			{
				// load the equip mesh object
				m_ResourceManager.LoadAssetBundleFromStream(sqlItem.Url, 
				                                            delegate(string szMeshName, AssetBundleResource abMeshFile) {

					// load the equip mesh
					GameObject mesh = abMeshFile.GetAsset<GameObject>(sqlItem.Url);
					if (!mesh)
						throw new System.NullReferenceException(sqlItem.Url);
					
					if (sqlItem.Type == (int)ItemType.IT_EQUIP)
					{	
						// load the equip mesh object
						m_ResourceManager.LoadAssetBundleFromStream(sqlItem.ExtendUrl, 
						                                            delegate(string szBoneName, AssetBundleResource abBoneFile) {
							
							StringHolder holder = abBoneFile.GetAsset<StringHolder>(sqlItem.ExtendUrl);
							if (holder)
								entityShape.ChangeEquip(0, mesh, holder);
							
							return true;
						});
					}
					else
					{
						
					}
					
					return true;
				});
			}
		}
	}
}
