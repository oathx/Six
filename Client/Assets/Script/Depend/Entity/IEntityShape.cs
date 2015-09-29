using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MountType {
	HH_NONE,
}

/// <summary>
/// I entity shape.
/// </summary>
public class IEntityShape : MonoBehaviour
{
	/// <summary>
	/// The character mount type
	/// </summary>
	protected Dictionary<MountType, 
		Transform> m_dMount = new Dictionary<MountType, Transform> ();
	
	/// <summary>
	/// The animator.
	/// </summary>
	protected Animator			m_Animator;
	
	/// <summary>
	/// Awake this instance.
	/// </summary>
	protected virtual void 		Awake()
	{
		m_Animator 	= GetComponent<Animator> ();
		if (m_Animator)
			m_Animator.applyRootMotion = true;
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	protected virtual void 		Update()
	{
		
	}
	
	/// <summary>
	/// Applies the root motion.
	/// </summary>
	/// <param name="bFlag">If set to <c>true</c> b flag.</param>
	public virtual void 			ApplyRootMotion(bool bFlag)
	{
		m_Animator.applyRootMotion = bFlag;
	}
	
	/// <summary>
	/// Installs the mount.
	/// </summary>
	/// <param name="dmt">Dmt.</param>
	public virtual void 			InstallMount(Dictionary<MountType, string> dmt)
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
	/// <param name="szName">Size name.</param>
	public virtual Transform	GetMount(MountType type)
	{
		if (type == MountType.HH_NONE || !m_dMount.ContainsKey(type))
			return transform;
		
		return m_dMount [type];
	}
	
	/// <summary>
	/// Gets the state info.
	/// </summary>
	/// <returns>The state info.</returns>
	public AnimatorStateInfo	GetStateInfo()
	{
		return m_Animator.IsInTransition (0) ? m_Animator.GetNextAnimatorStateInfo (0) : m_Animator.GetCurrentAnimatorStateInfo (0);
	}
	
	/// <summary>
	/// Play the specified name, transition and replay.
	/// </summary>
	/// <param name="name">Name.</param>
	/// <param name="transition">Transition.</param>
	/// <param name="replay">If set to <c>true</c> replay.</param>
	public virtual void 		Play(string szClipName, float fTransition, bool bReplay)
	{
		try{
			if (m_Animator && !string.IsNullOrEmpty(szClipName))
			{
#if OPEN_DEBUG_LOG
				Debug.Log("Play animation name " + name + " clip " + szClipName);
#endif
				
				if (fTransition > 0)
				{
					AnimatorStateInfo state = GetStateInfo();
					m_Animator.CrossFade(szClipName, fTransition / state.length, 0, 0);
				}
				else
				{	
					m_Animator.Play(szClipName, 0, 0);
				}
			}
		}
		catch(System.Exception e)
		{
			Debug.Log("State could not be found " + szClipName + " Message " + e.Message);
		}
	}

	/// <summary>
	/// Is current animation clip loop.
	/// </summary>
	public bool 	IsLoop 
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
	public float 	NormalizedTime
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
	public bool 	IsPlaying 
	{
		get { 
			return IsLoop ? true : (NormalizedTime < 1);
		} 
	}
	
	/// <summary>
	/// Gets or sets the animation time.
	/// </summary>
	/// <value>The animation time.</value>
	public float 	AnimationTime
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
	public float 	AnimationLength 
	{ 
		get { 
			AnimatorStateInfo info = GetStateInfo();
			return info.length;
		}
	}
	
	/// <summary>
	/// Current animator play speed.
	/// </summary>
	public float 	AnimationSpeed 
	{ 
		get { 
			return m_Animator.speed; 
		} 
		set { 
			m_Animator.speed = value; 
		} 
	}
}
