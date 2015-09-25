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
}
