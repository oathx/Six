using UnityEngine;
using System.Collections;

public enum UGUISide
{
		BottomLeft,
		Left,
		TopLeft,
		Top,
		TopRight,
		Right,
		BottomRight,
		Bottom,
		Center,
}

public class UGUIMath
{
	public static Bounds calculateWorldBounds(GameObject go)
	{
		if (null == go) { return new Bounds(Vector3.zero, Vector3.zero); }

		Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
		Vector2 max = new Vector2(float.MinValue, float.MinValue);
		Vector3 v;

		RectTransform[] rts = go.GetComponentsInChildren<RectTransform>();
		if (rts.Length == 0) return new Bounds(go.transform.position, Vector3.zero);

		for (int i = 0, imax = rts.Length; i < imax; ++i)
		{
			RectTransform t = rts[i];
			if (!t.gameObject.activeSelf) { continue; }

			Vector3[] corners = new Vector3[4];
			t.GetWorldCorners(corners);

			for (int j = 0; j < 4; ++j)
			{
				v = corners[j];
				if (v.x > max.x) max.x = v.x;
				if (v.y > max.y) max.y = v.y;

				if (v.x < min.x) min.x = v.x;
				if (v.y < min.y) min.y = v.y;
			}
		}

		Bounds b = new Bounds(min, Vector3.zero);
		b.Encapsulate(max);

		return b;
	}
}
