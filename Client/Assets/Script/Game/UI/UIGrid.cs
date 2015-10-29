using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// User interface version.
/// </summary>
[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(ScrollRect))]
[RequireComponent(typeof(Mask))]
[RequireComponent(typeof(CanvasRenderer))]
[RequireComponent(typeof(Image))]
public class UIGrid : IUIWidget
{
	[SerializeField]
	public GridLayoutGroup		GridLayout;

	/// <summary>
	/// The grid list.
	/// </summary>
	public Dictionary<int, 
		IUIWidget> GridList = new Dictionary<int, IUIWidget>();

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		GridLayout = GetComponentInChildren<GridLayoutGroup>();
		if (!GridLayout)
			throw new System.NullReferenceException();
	}

	/// <summary>
	/// Resets the position.
	/// </summary>
	public void ResetPosition()
	{
		RectTransform layout = GridLayout.GetComponent<RectTransform>();
		if (layout)
		{
			ScrollRect scroll = GetComponent<ScrollRect>();
			if (scroll.horizontal)
			{
				Vector2 vDelta = layout.sizeDelta;
				vDelta.x = GridLayout.cellSize.x * GridLayout.transform.childCount + GridLayout.spacing.x * GridLayout.transform.childCount;

				layout.sizeDelta = vDelta;
			}
			else
			{
				Vector2 vDelta = layout.sizeDelta;
				vDelta.y = GridLayout.cellSize.y * GridLayout.transform.childCount + GridLayout.spacing.y * GridLayout.transform.childCount;
				
				layout.sizeDelta = vDelta;
			}

			scroll.verticalNormalizedPosition 	= 1.0f;
			scroll.horizontalNormalizedPosition	= 1.0f;
		}
	}

	/// <summary>
	/// Plaies the tween.
	/// </summary>
	public void 	RotateTween(Vector3 add, float fDuration)
	{
		List<IUIWidget> aryList = new List<IUIWidget>(GridList.Values);

		//iTween.Stop();
		for(int i=0; i<aryList.Count; i++)
		{
			iTween.RotateAdd(aryList[i].gameObject, add, (i + 1) * fDuration);
		}
	}

	/// <summary>
	/// Adds the child.
	/// </summary>
	/// <returns>The child.</returns>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public T	AddChild<T>(int nID, string szResourceName) where T : IUIWidget
	{
		GameObject resource = Resources.Load<GameObject>(szResourceName);
		if (!resource)
			throw new System.NullReferenceException();

		GameObject child = GameObject.Instantiate(resource) as GameObject;
		if (child)
		{
			child.transform.SetParent(GridLayout.transform);
		
			// reset child scale value
			child.transform.localScale 	= Vector3.one;
			child.transform.name		= nID.ToString();
		}

		// bind external script
		T cmp = child.AddComponent<T>();
		if (!cmp)
			throw new System.NullReferenceException();

		Resources.UnloadUnusedAssets();

		if (!GridList.ContainsKey(nID))
			GridList.Add(nID, cmp);

		ResetPosition();

		return cmp;
	}

	/// <summary>
	/// Removes the child.
	/// </summary>
	/// <param name="nID">N I.</param>
	public void RemoveChild(int nID)
	{
		if (GridList.ContainsKey(nID))
		{
			GameObject.Destroy(
				GridList[nID].gameObject
				);

			GridList.Remove(nID);

			// reset grid position
			ResetPosition();
		}
	}
}