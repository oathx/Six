using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Mathf ex.
/// </summary>
public class MathfEx
{
	/// <summary>
	/// Split the specified szText and ch.
	/// </summary>
	/// <param name="szText">Size text.</param>
	/// <param name="ch">Ch.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static List<int>		SplitInt32(string szText, char ch)
	{
		List<int> aryReturn = new List<int>();
		
		string[] arySplit = szText.Split(ch);
		foreach(string s in arySplit)
		{
			int nValue = System.Convert.ToInt32(s);
			if (nValue >= 0)
				aryReturn.Add(nValue);
		}
		
		return aryReturn;
	}

	/// <summary>
	/// Splits the single.
	/// </summary>
	/// <returns>The single.</returns>
	/// <param name="szText">Size text.</param>
	/// <param name="ch">Ch.</param>
	public static List<float>	SplitSingle(string szText, char ch)
	{
		List<float> aryReturn = new List<float>();
		
		string[] arySplit = szText.Split(ch);
		foreach(string s in arySplit)
		{
			float fValue = System.Convert.ToSingle(s);
			if (fValue >= 0)
				aryReturn.Add(fValue);
		}
		
		return aryReturn;
	}

	/// <summary>
	/// Tos the vector3.
	/// </summary>
	/// <returns>The vector3.</returns>
	/// <param name="szText">Size text.</param>
	public static Vector3	ToVector3(string szText)
	{
		string[] arySplit = szText.Split(',');
		if (arySplit.Length >= 3)
		{
			return new Vector3(float.Parse(arySplit[0]), float.Parse(arySplit[1]), float.Parse(arySplit[2]));
		}

		return Vector3.zero;
	}

	/// <summary>
	/// Tos the color.
	/// </summary>
	/// <returns>The color.</returns>
	/// <param name="szText">Size text.</param>
	public static Color		ToColor(string szText)
	{
		string[] arySplit = szText.Split(',');
		if (arySplit.Length >= 4)
		{
			return new Color(int.Parse(arySplit[0]), int.Parse(arySplit[1]), int.Parse(arySplit[2]), int.Parse(arySplit[3]));
		}
		
		return Color.white;
	}

	/// <summary>
	/// Direction2s the angle.
	/// </summary>
	/// <returns>The angle.</returns>
	/// <param name="direction">Direction.</param>
	public static float 	Direction2Angle(Vector3 direction)
	{
		direction.Normalize();
		return 90 - ((float)System.Math.Atan2(direction.z, direction.x) * Mathf.Rad2Deg);
	}

	/// <summary>
	/// Angle2s the forward.
	/// </summary>
	/// <returns>The forward.</returns>
	/// <param name="angle">Angle.</param>
	public static Vector3 	Angle2Forward(float angle)
	{
		angle = (angle + 90) * Mathf.Deg2Rad;
		return new Vector3(-Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;
	}

	/// <summary>
	/// Coordinates the convert.
	/// </summary>
	/// <returns>The convert.</returns>
	/// <param name="resCoordinate">Res coordinate.</param>
	/// <param name="destCoordinate">Destination coordinate.</param>
	/// <param name="resPosition">Res position.</param>
	public static Vector2 	CoordinateConvert(Vector2 resCoordinate, Vector2 destCoordinate, Vector2 resPosition)
	{
		return new Vector2(resPosition.x * destCoordinate.x / resCoordinate.x, resPosition.y * destCoordinate.y / resCoordinate.y);
	}

	/// <summary>
	/// Radians to angle.
	/// </summary>
	/// <returns>The to angle.</returns>
	/// <param name="radian">Radian.</param>
	public static float		RadianToAngle(float radian)
	{
		return radian * 180 / (float)System.Math.PI;
	}

	/// <summary>
	/// Directions to angle.
	/// </summary>
	/// <returns>The to angle.</returns>
	/// <param name="direction">Direction.</param>
	public static float 	DirectionToAngle(Vector3 vDirection)
	{
		vDirection.Normalize();
		return 90 - RadianToAngle((float)System.Math.Atan2(vDirection.z, vDirection.x));
	}

	/// <summary>
	/// Ins the circle.
	/// </summary>
	/// <returns>The circle.</returns>
	/// <param name="vCenter">V center.</param>
	/// <param name="fRadius">F radius.</param>
	/// <param name="nLayerMask">N layer mask.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T[]			InCircle<T>(Vector3 vCenter, float fRadius, int nLayerMask) where T : UnityEngine.Object
	{
		List<T> 
			aryReturn = new List<T>();

		Collider[] aryCollider = Physics.OverlapSphere(vCenter, fRadius, nLayerMask);
		foreach(Collider c in aryCollider)
		{
			T cmp = c.GetComponent<T>();
			if (cmp)
				aryReturn.Add(cmp);
		}

		return aryReturn.ToArray();
	}

	/// <summary>
	/// Overlaies the bounds.
	/// </summary>
	/// <returns>The bounds.</returns>
	/// <param name="boundsList">Bounds list.</param>
	public static Bounds OverlayBounds(Bounds[] boundsList)
	{
		if (null == boundsList
			|| 0 == boundsList.Length)
		{
			return new Bounds(Vector3.zero, Vector3.zero);
		}

		Bounds bounds = boundsList[0];
		for (int i = boundsList.Length; --i > 0;)
		{
			Bounds b = boundsList[i];
			bounds.Encapsulate(b.center + new Vector3(b.extents.x, b.extents.y, b.extents.z));
			bounds.Encapsulate(b.center + new Vector3(b.extents.x, -b.extents.y, b.extents.z));
			bounds.Encapsulate(b.center + new Vector3(b.extents.x, b.extents.y, -b.extents.z));
			bounds.Encapsulate(b.center + new Vector3(b.extents.x, -b.extents.y, -b.extents.z));
			bounds.Encapsulate(b.center + new Vector3(-b.extents.x, b.extents.y, b.extents.z));
			bounds.Encapsulate(b.center + new Vector3(-b.extents.x, -b.extents.y, b.extents.z));
			bounds.Encapsulate(b.center + new Vector3(-b.extents.x, b.extents.y, -b.extents.z));
			bounds.Encapsulate(b.center + new Vector3(-b.extents.x, -b.extents.y, -b.extents.z));
		}

		return bounds;
	}

	public static void AnchorTo(GameObject src, UGUISide srcSide, GameObject tar, UGUISide tarSide, Rect area = default(Rect))
	{
		if (null == tar || tar == src)
		{
			return;
		}
		Bounds srcBounds = UGUIMath.calculateWorldBounds(src);
		Bounds destBounds = UGUIMath.calculateWorldBounds(tar);

		Vector3 srcOffset = GetBoundsOffset(srcBounds, srcSide);
		Vector3 destOffset = GetBoundsOffset(destBounds, tarSide);

		src.transform.position += (destBounds.center - srcBounds.center) + srcOffset - destOffset;

		SetUIArea(src.transform, area);
	}

	public static bool SetUIArea(Transform transform, Rect area = default(Rect))
	{
		Bounds bounds = UGUIMath.calculateWorldBounds(transform.gameObject);

		if (default(Rect) == area)
		{
			area = UISystem.GetSingleton().ScreenRect;
		}

		Vector3 delta = default(Vector3);
		if (bounds.center.x - bounds.extents.x < area.x + area.width / 2)
		{
			delta.x += Mathf.Abs(bounds.center.x - bounds.extents.x);
		}
		else if (bounds.center.x + bounds.extents.x > area.width)
		{
			delta.x -= Mathf.Abs(bounds.center.x + bounds.extents.x) - area.width;
		}

		if (bounds.center.y - bounds.extents.y < area.y + area.height / 2)
		{
			delta.y += Mathf.Abs(bounds.extents.y - bounds.center.y);
		}
		else if (bounds.center.y + bounds.extents.y > area.height)
		{
			delta.y -= Mathf.Abs(bounds.center.y + bounds.extents.y) - area.height;
		}

		transform.position += delta;

		return delta != default(Vector3);
	}

	public static Vector3 GetBoundsOffset(Bounds bounds, UGUISide side)
	{
		Vector3 offset = Vector3.zero;

		switch (side)
		{
			case UGUISide.Bottom:
				offset.y = bounds.extents.y;
				break;
			case UGUISide.BottomLeft:
				offset.x = bounds.extents.x;
				offset.y = bounds.extents.y;
				break;
			case UGUISide.BottomRight:
				offset.x = -bounds.extents.x;
				offset.y = bounds.extents.y;
				break;
			case UGUISide.Left:
				offset.x = bounds.extents.x;
				break;
			case UGUISide.Right:
				offset.x = -bounds.extents.x;
				break;
			case UGUISide.Top:
				offset.y = -bounds.extents.y;
				break;
			case UGUISide.TopLeft:
				offset.x = bounds.extents.x;
				offset.y = -bounds.extents.y;
				break;
			case UGUISide.TopRight:
				offset.x = -bounds.extents.x;
				offset.y = -bounds.extents.y;
				break;
		}

		return offset;
	}

    public static Vector3 GetDragPoint(UnityEngine.EventSystems.PointerEventData data, RectTransform DragPanel)
    {
        Vector3 GlobalPoint;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(DragPanel, data.position, data.pressEventCamera, out GlobalPoint);
        return GlobalPoint;
    }
}
