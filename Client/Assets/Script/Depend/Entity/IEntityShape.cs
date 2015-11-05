using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MountType
{
	None				= 0,
	Dummy_L_Hand		= 1,
	Dummy_R_Hand		= 2,
	Dummy_L_Foot		= 3,
	Dummy_R_Foot		= 4,
	Dummy_L_Clavicle	= 5,
	Dummy_R_Clavicle	= 6,
	Dummy_Spine1		= 7,
	Dummy_NPower		= 8,
	Dummy_Pelvis		= 9,
	Dummy_Head			= 10,
	Dummy_Spine			= 11,
	Dummy_Tail			= 12,
	Dummy_L_Wings		= 13,
	Dummy_R_Wings		= 14,
	Dummy_R_HandGun		= 15,
}

// shape type define
public enum PartType
{
	PT_NONE 			= 0,
	PT_ARMAMENT 		= 1,
	PT_ENGINE 			= 2,
	PT_CONTROL 			= 3,
	PT_HEAD 			= 4,
	PT_ARMOUR 			= 5,
	PT_ARM 				= 6,
	PT_LEG 				= 7,
	PT_PROTECT 			= 8,
}

/// <summary>
/// I entity shape.
/// </summary>
public class IEntityShape : MonoBehaviour
{
	/// <summary>
	/// The mount.
	/// </summary>
	protected Dictionary<MountType, 
		Transform> m_dMount = new Dictionary<MountType, Transform>();

	// bone buffer
	protected class BoneBuffer
	{
		public SkinnedMeshRenderer 	smr;
		public StringHolder			holder;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="CharacterEntity+BoneInfo"/> class.
		/// </summary>
		/// <param name="m">M.</param>
		/// <param name="h">The height.</param>
		public BoneBuffer(SkinnedMeshRenderer m, StringHolder h)
		{
			smr 	= m;
			holder 	= h;
		}
	}
	
	/// <summary>
	/// The d skin mesh.
	/// </summary>
	protected Dictionary<PartType, 
		BoneBuffer> m_dMesh = new Dictionary<PartType, BoneBuffer> ();

	/// <summary>
	/// Sets the enabled.
	/// </summary>
	/// <param name="bEnabled">If set to <c>true</c> b enabled.</param>
	public virtual void		SetEnabled(bool bEnabled)
	{
		gameObject.SetActive(bEnabled);
	}

	/// <summary>
	/// Gets the enabled.
	/// </summary>
	/// <returns><c>true</c>, if enabled was gotten, <c>false</c> otherwise.</returns>
	public bool				GetEnabled()
	{
		return gameObject.activeSelf;
	}

	/// <summary>
	/// Sets the scale.
	/// </summary>
	/// <param name="fScale">F scale.</param>
	public virtual	void 	SetScale(float fScale)
	{
		transform.localScale = Vector3.one * fScale;
	}

	/// <summary>
	/// Sets the parent.
	/// </summary>
	/// <param name="parent">Parent.</param>
	public virtual void 	SetParent(Transform parent)
	{
		transform.SetParent(parent);
	}

	/// <summary>
	/// Sets the position.
	/// </summary>
	/// <param name="vTarget">V target.</param>
	public virtual void 	SetPosition(Vector3 vTarget)
	{
		transform.localPosition = vTarget;
	}

	/// <summary>
	/// Installs the mount.
	/// </summary>
	/// <param name="aryMountName">Ary mount name.</param>
	public void 			InstallMount(Dictionary<MountType, string> dmt)
	{
		Transform[] aryTransform = GetComponentsInChildren<Transform> ();
		foreach(KeyValuePair<MountType, string> it in dmt)
		{
			foreach(Transform t in aryTransform)
			{
				if (it.Value == t.name && !m_dMount.ContainsKey(it.Key))
				{
					m_dMount.Add(it.Key, t);
				}
			}
		}
	}

	/// <summary>
	/// Gets the mount.
	/// </summary>
	/// <returns>The mount.</returns>
	/// <param name="type">Type.</param>
	public Transform		GetMount(MountType type)
	{
		if (type == MountType.None)
			return transform;

		return m_dMount[type];
	}

	/// <summary>
	/// Bind the specified type and bind.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="bind">Bind.</param>
	public void 			Bind(MountType type, Object resource, float fDuration,  bool bFollow, string szBindName)
	{
		Transform mount = GetMount(type);
		if (mount)
		{
			string szAppBindName = string.IsNullOrEmpty(szBindName) ? resource.name : szBindName;

			// destroy old bind object
			Transform bind = mount.FindChild(szAppBindName);
			if (bind)
				GameObject.Destroy(bind);

			// create a new bind object
			GameObject goBind = GameObject.Instantiate(resource) as GameObject;
			if (!goBind)
				throw new System.NullReferenceException("Can't instantiate resource " + resource.name);

			goBind.transform.name		= szAppBindName;
			goBind.transform.position	= mount.position;
			goBind.transform.rotation	= mount.rotation;
			
			if (bFollow)
				goBind.transform.parent = mount;

			// wait destroy
			if (fDuration > 0.0f)
			{
				StartCoroutine(OnAppBind(goBind, fDuration));
			}
		}
	}

	/// <summary>
	/// Raises the app bind event.
	/// </summary>
	/// <param name="goBind">Go bind.</param>
	/// <param name="fWaitTime">F wait time.</param>
	IEnumerator			OnAppBind(GameObject goBind, float fWaitTime)
	{
		yield return new WaitForSeconds (fWaitTime);
		
		if (goBind)
		{
			GameObject.Destroy(goBind);
		}
	}

	/// <summary>
	/// Determines whether this instance cancel bind the specified type szBindName.
	/// </summary>
	/// <returns><c>true</c> if this instance cancel bind the specified type szBindName; otherwise, <c>false</c>.</returns>
	/// <param name="type">Type.</param>
	/// <param name="szBindName">Size bind name.</param>
	public void 		CancelBind(MountType type, string szBindName)
	{
		Transform mount = GetMount (type);
		if (mount)
		{
			Transform bind = mount.FindChild(szBindName);
			if (bind)
			{
				GameObject.Destroy(bind.gameObject);
			}
		}
	}

	/// <summary>
	/// Megres the equip.
	/// </summary>
	public void 		MegreEquip()
	{
		// add bones list
		List<Transform> aryBones 	= new List<Transform>();
		// save all mesh
		List<CombineInstance> 
			aryCombineInstances 	= new List<CombineInstance>();
		// save all material
		List<Material> aryMaterial 	= new List<Material>();
		
		foreach(KeyValuePair<PartType, BoneBuffer> it in m_dMesh)
		{
			foreach(string bone in it.Value.holder.content)
			{
				foreach(Transform hip in GetComponentsInChildren<Transform>())
				{
					if (hip.name != bone)
						continue;
					
					aryBones.Add(hip);
					break;
				}
			}
			
			// create combin instance
			CombineInstance c = new CombineInstance();
			c.mesh = it.Value.smr.sharedMesh;
			aryCombineInstances.Add(c);
			
			aryMaterial.AddRange(
				it.Value.smr.sharedMaterials
				);
		}
		
		SkinnedMeshRenderer smr = GetComponent<SkinnedMeshRenderer>();
		if (smr)
		{
			smr.sharedMesh = new Mesh();
			smr.sharedMesh.CombineMeshes(aryCombineInstances.ToArray(), false, false);
			
			smr.bones 			= aryBones.ToArray();
			smr.materials 		= aryMaterial.ToArray();
			smr.useLightProbes 	= true;
		}
	}
	
	/// <summary>
	/// Changes the equip.
	/// </summary>
	/// <param name="part">Part.</param>
	/// <param name="szEquipName">Size equip name.</param>
	/// <param name="assetBundle">Asset bundle.</param>
	public virtual void 	ChangeEquip(PartType part, GameObject equipMesh, StringHolder holder)
	{
		/*
		if (part == PartType.PT_ARM)
		{
			Transform mount = GetMount(MountType.Dummy_R_HandGun);
			if (!mount)
				throw new System.NullReferenceException(MountType.Dummy_R_HandGun.ToString());

			foreach(string content in holder.content)
			{
				Debug.Log(content);
			}
		}
		*/
	
		m_dMesh [part] = new BoneBuffer (
			equipMesh.GetComponent<SkinnedMeshRenderer>(), holder
			);
		
		// megre equip
		MegreEquip ();

	}
	
	/// <summary>
	/// Removes the equip.
	/// </summary>
	/// <param name="part">Part.</param>
	public void 		RemoveEquip(PartType part)
	{
		if (m_dMesh.ContainsKey(part))
		{
			// remove the equip
			m_dMesh.Remove(part);
			
			// reset the all equip
			MegreEquip();
		}
	}
}
