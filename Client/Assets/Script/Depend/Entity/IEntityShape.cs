using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MountType
{
	HH_NONE,
	HH_WAIST,
	HH_WEAPON,
	HH_CHEST,
	HH_L_FOOT,
	HH_R_FOOT,
	HH_L_SHOULDER,
	HH_R_SHOULDER,
	HH_L_HAND,
	HH_R_HAND,
}

/// <summary>
/// I entity shape.
/// </summary>
public class IEntityShape : MonoBehaviour
{
	/// <summary>
	/// The mount.
	/// </summary>
	protected Dictionary<MountType, Transform> m_dMount = new Dictionary<MountType, Transform>();

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
		if (type == MountType.HH_NONE)
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
}
