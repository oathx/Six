using UnityEngine;
using System.Collections;

/// <summary>
/// I entity.
/// </summary>
public class IEntity : IEntityProperty
{
	/// <summary>
	/// Gets the machine.
	/// </summary>
	/// <value>The machine.</value>
	protected IAIMachine	m_Machine;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	protected virtual void 	Awake()
	{}

	/// <summary>
	/// Start this instance.
	/// </summary>
	protected virtual void 	Start()	
	{}

	/// <summary>
	/// Update this instance.
	/// </summary>
	protected virtual void 	Update() 	
	{
		if (m_Machine)
			m_Machine.UpdateMachine(Time.deltaTime);
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	protected virtual void 	OnDestroy()
	{
		if (m_Machine)
		{
			m_Machine.Clearup();
		}
	}

	/// <summary>
	/// Gets the ID.
	/// </summary>
	/// <returns>The I.</returns>
	public int 				GetID()
	{
		return EntityID;
	}

	/// <summary>
	/// Sets the machine.
	/// </summary>
	/// <param name="machine">Machine.</param>
	public void 			SetMachine(IAIMachine machine)
	{
		m_Machine = machine;
	}

	/// <summary>
	/// Gets the machine.
	/// </summary>
	/// <returns>The machine.</returns>
	public IAIMachine		GetMachine()
	{
		return m_Machine;
	}

	/// <summary>
	/// Gets the position.
	/// </summary>
	/// <returns>The position.</returns>
	public Vector3			GetPosition()
	{
		return transform.position;
	}
	
	/// <summary>
	/// Sets the position.
	/// </summary>
	/// <param name="vPosition">V position.</param>
	public void 			SetPosition(Vector3 vPosition)
	{
		transform.position = vPosition;
	}

	/// <summary>
	/// Gets the euler angle.
	/// </summary>
	/// <returns>The euler angle.</returns>
	public Vector3			GetEulerAngle()
	{
		return transform.eulerAngles;
	}

	/// <summary>
	/// Move the specified vDirection.
	/// </summary>
	/// <param name="vDirection">V direction.</param>
	public virtual void		MoveTo(Vector3 vPosition)
	{
		transform.position = vPosition;
	}
	
	/// <summary>
	/// Rotates to.
	/// </summary>
	/// <param name="vRotation">V rotation.</param>
	/// <param name="fSpeed">F speed.</param>
	public virtual void 	RotateTo (Vector3 vRotation, float fSpeed)
	{
		if (fSpeed > 0)
		{
			transform.rotation = Quaternion.RotateTowards(
				Quaternion.Euler(transform.eulerAngles), Quaternion.Euler(vRotation), fSpeed * Time.deltaTime
				);
		}
	}
	
	/// <summary>
	/// Rotate to a value.
	/// </summary>
	/// <param name="vTarget">Target rotation.</param>
	public virtual void		SetRotateTo(Vector3 vTarget)
	{
		transform.eulerAngles = vTarget;
	}
	
	/// <summary>
	/// Look at target.
	/// </summary>
	/// <param name="target"></param>
	public virtual void		LookAt(IEntity target)
	{
		transform.LookAt(target.transform);
		transform.eulerAngles = Vector3.up * transform.eulerAngles.y;
	}
	
	/// <summary>
	/// Looks at postion.
	/// </summary>
	/// <param name="postion">Postion.</param>
	public virtual void 	LookAt(Vector3 vPostion)
	{
		transform.LookAt(vPostion);
		transform.eulerAngles = Vector3.up * transform.eulerAngles.y;
	}
	
	/// <summary>
	/// Sets the layer.
	/// </summary>
	/// <param name="nLayer">N layer.</param>
	public virtual void 	SetLayer(int nLayer)
	{
		Transform[] aryTransform = GetComponentsInChildren<Transform> ();
		foreach (Transform child in aryTransform)
			child.gameObject.layer = nLayer;
	}

	/// <summary>
	/// Gets the layer.
	/// </summary>
	/// <returns>The layer.</returns>
	public int 				GetLayer()
	{
		return gameObject.layer;
	}
}
