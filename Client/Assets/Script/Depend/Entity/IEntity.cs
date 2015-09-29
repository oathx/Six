using UnityEngine;
using System.Collections;

/// <summary>
/// I entity.
/// </summary>
public class IEntity : IEntityProperty
{
	/// <summary>
	/// Gets the position.
	/// </summary>
	/// <returns>The position.</returns>
	public Vector3				GetPosition()
	{
		return transform.position;
	}
	
	/// <summary>
	/// Sets the position.
	/// </summary>
	/// <param name="vPosition">V position.</param>
	public void 				SetPosition(Vector3 vPosition)
	{
		transform.position = vPosition;
	}
	
	/// <summary>
	/// Gets the euler angle.
	/// </summary>
	/// <returns>The euler angle.</returns>
	public virtual Vector3		GetEulerAngle()
	{
		return transform.localEulerAngles;
	}
	
	/// <summary>
	/// Sets the euler angle.
	/// </summary>
	/// <param name="vEuler">V euler.</param>
	public virtual void 		SetEulerAngle(Vector3 vEuler)
	{
		transform.localEulerAngles = vEuler;
	}
	
	/// <summary>
	/// Gets the forward.
	/// </summary>
	/// <returns>The forward.</returns>
	public virtual Vector3		GetForward()
	{
		return transform.forward;
	}
	
	/// <summary>
	/// Gets the direction.
	/// </summary>
	/// <returns>The direction.</returns>
	/// <param name="vDir">V dir.</param>
	public virtual Vector3		GetDirection(Vector3 vDir)
	{
		return transform.TransformDirection(vDir);
	}
	
	/// <summary>
	/// Gets the target angle.
	/// </summary>
	/// <returns>The target angle.</returns>
	/// <param name="target">Target.</param>
	public virtual float		GetTargetAngle(IEntity target)
	{
		return Vector3.Angle(transform.forward, target.GetPosition() - GetPosition());
	}
	
	/// <summary>
	/// Move the specified vDirection.
	/// </summary>
	/// <param name="vDirection">V direction.</param>
	public virtual void			MoveTo(Vector3 vPosition)
	{
		transform.position = vPosition;
	}
	
	/// <summary>
	/// Rotates to.
	/// </summary>
	/// <param name="vRotation">V rotation.</param>
	/// <param name="fSpeed">F speed.</param>
	public virtual void 		RotateTo (Vector3 vRotation, float fSpeed)
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
	public virtual void			SetRotateTo(Vector3 vTarget)
	{
		transform.eulerAngles = vTarget;
	}
	
	/// <summary>
	/// Look at target.
	/// </summary>
	/// <param name="target"></param>
	public virtual void			LookAt(IEntity target)
	{
		transform.LookAt(target.transform);
		transform.eulerAngles = Vector3.up * transform.eulerAngles.y;
	}
	
	/// <summary>
	/// Looks at postion.
	/// </summary>
	/// <param name="postion">Postion.</param>
	public virtual void 		LookAt(Vector3 vPostion)
	{
		transform.LookAt(vPostion);
		transform.eulerAngles = Vector3.up * transform.eulerAngles.y;
	}
	
	/// <summary>
	/// Sets the layer.
	/// </summary>
	/// <param name="nLayer">N layer.</param>
	public virtual void 		SetLayer(int nLayer)
	{
		Transform[] aryTransform = GetComponentsInChildren<Transform> ();
		foreach (Transform child in aryTransform)
			child.gameObject.layer = nLayer;
	}
}
