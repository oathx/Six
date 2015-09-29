using UnityEngine;

/// <summary>
/// Mathf ex.
/// </summary>
public class MathfEx
{
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
}
