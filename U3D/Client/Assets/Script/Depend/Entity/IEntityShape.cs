using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MountType
{
	None				= 0,
	DUMMY_GIDDINESS		= 1,
	DUMMY_HORSE			= 2,
	DUMMY_WING			= 3,
	DUMMY_PICK			= 4,
	DUMMY_ARM			= 5,
}

// shape type define
public enum PartType
{
	PT_NONE 			= 0,
	PT_HAIR 			= 1,
	PT_FACE 			= 2,
	PT_BODY 			= 3,
	PT_LEG 				= 4,
	PT_ARM 				= 5,
	PT_PROTECT 			= 6,
}

public class EntityLayer
{
	public static int UI 		= LayerMask.NameToLayer("UI");
	public static int MAIN 		= LayerMask.NameToLayer("Main");
	public static int NPC		= LayerMask.NameToLayer("Npc");
	public static int MONSTER	= LayerMask.NameToLayer("Monster");
	public static int PLAYER	= LayerMask.NameToLayer("Player");
	public static int EFFECT	= LayerMask.NameToLayer("Effect");
	public static int TRIGGER	= LayerMask.NameToLayer("Trigger");
}

/// <summary>
/// I entity shape.
/// </summary>
public class IEntityShape : IEntity
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
	/// The animator.
	/// </summary>
	protected Animator m_Animator;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	protected override void Awake()
	{
		m_Animator 	= GetComponent<Animator> ();
		if (m_Animator)
			m_Animator.applyRootMotion = true;
	}

	/// <summary>
	/// Applies the root motion.
	/// </summary>
	/// <param name="bFlag">If set to <c>true</c> b flag.</param>
	public void 		ApplyRootMotion(bool bFlag)
	{
		m_Animator.applyRootMotion = bFlag;
	}

	/// <summary>
	/// Gets the apply root motion.
	/// </summary>
	/// <returns><c>true</c>, if apply root motion was gotten, <c>false</c> otherwise.</returns>
	public bool			GetApplyRootMotion()
	{
		return m_Animator.applyRootMotion;
	}

	/// <summary>
	/// Sets the enabled.
	/// </summary>
	/// <param name="bEnabled">If set to <c>true</c> b enabled.</param>
	public void			SetEnabled(bool bEnabled)
	{
		gameObject.SetActive(bEnabled);
	}

	/// <summary>
	/// Gets the enabled.
	/// </summary>
	/// <returns><c>true</c>, if enabled was gotten, <c>false</c> otherwise.</returns>
	public bool			GetEnabled()
	{
		return gameObject.activeSelf;
	}

	/// <summary>
	/// Installs the mount.
	/// </summary>
	/// <param name="aryMountName">Ary mount name.</param>
	public void 		InstallMount(Dictionary<MountType, string> dmt)
	{
		Transform[] aryTransform = GetComponentsInChildren<Transform> ();
		foreach(KeyValuePair<MountType, string> it in dmt)
		{
			foreach(Transform t in aryTransform)
			{
				if (it.Value == t.name && !m_dMount.ContainsKey(it.Key))
				{
					m_dMount.Add(it.Key, t);
					break;
				}
			}
		}
	}

	/// <summary>
	/// Gets the mount.
	/// </summary>
	/// <returns>The mount.</returns>
	/// <param name="type">Type.</param>
	public Transform	GetMount(MountType type)
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
	public void 		Bind(MountType type, Object resource, float fDuration,  bool bFollow, string szBindName)
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
	public void 		ChangeEquip(PartType part, GameObject equipMesh, StringHolder holder)
	{	
		SkinnedMeshRenderer skinMeshRender = equipMesh.GetComponent<SkinnedMeshRenderer>();
		if (!skinMeshRender)
			skinMeshRender = equipMesh.AddComponent<SkinnedMeshRenderer>();

		m_dMesh [part] = new BoneBuffer (
			skinMeshRender, holder
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

	/// <summary>
	/// Play the specified szAnimationName, fTransition and bReplay.
	/// </summary>
	/// <param name="szAnimationName">Size animation name.</param>
	/// <param name="fTransition">F transition.</param>
	/// <param name="bReplay">If set to <c>true</c> b replay.</param>
	public virtual void			Play(string szAnimationName, float fTransition, bool bReplay)
	{
#if OPEN_DEBUG_LOG
		Debug.Log("Play charecter name " + name + " clip " + szAnimationName);
#endif

		if (m_Animator && !string.IsNullOrEmpty(szAnimationName))
		{
			if (fTransition > 0)
			{
				AnimatorStateInfo state = GetStateInfo();
				m_Animator.CrossFade(szAnimationName, fTransition / state.length, 0, 0);
			}
			else
			{	
				m_Animator.Play(szAnimationName, 0, 0);
			}
			
			CurrentClipName = szAnimationName;
		}
	}

	/// <summary>
	/// Gets the name of the current clip.
	/// </summary>
	/// <value>The name of the current clip.</value>
	public string		CurrentClipName
	{ get; private set; }
	
	/// <summary>
	/// Is current animation clip loop.
	/// </summary>
	public bool 		IsLoop 
	{
		get {
			AnimatorStateInfo state = GetStateInfo();
			return state.loop;
		} 
	}
	
	/// <summary>
	/// Gets the normalized time.
	/// </summary>
	/// <value>The normalized time.</value>
	public float 		NormalizedTime
	{
		get { 
			AnimatorStateInfo state = GetStateInfo();
			return state.normalizedTime;
		} 
	}
	
	/// <summary>
	/// Gets a value indicating whether this instance is playing.
	/// </summary>
	/// <value><c>true</c> if this instance is playing; otherwise, <c>false</c>.</value>
	public bool 		IsPlaying 
	{
		get { 
			return IsLoop ? true : (NormalizedTime < 1);
		} 
	}
	
	/// <summary>
	/// Gets or sets the animation time.
	/// </summary>
	/// <value>The animation time.</value>
	public float 		AnimationTime
	{
		get{
			return NormalizedTime * AnimationLength;
		}
		set{
			m_Animator.ForceStateNormalizedTime(value / AnimationLength);
		}
	}
	
	/// Current animation clip length
	/// </summary>
	public float 		AnimationLength 
	{ 
		get { 
			AnimatorStateInfo info = GetStateInfo();
			return info.length;
		}
	}
	
	/// <summary>
	/// Current animator play speed.
	/// </summary>
	public float 		AnimationSpeed 
	{ 
		get { 
			return m_Animator.speed; 
		} 
		set { 
			m_Animator.speed = value; 
		} 
	}

	/// <summary>
	/// Gets the state info.
	/// </summary>
	/// <returns>The state info.</returns>
	public AnimatorStateInfo	GetStateInfo()
	{
		return m_Animator.IsInTransition (0) ? m_Animator.GetNextAnimatorStateInfo (0) : m_Animator.GetCurrentAnimatorStateInfo (0);
	}
}
