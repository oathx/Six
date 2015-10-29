using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

[CanEditMultipleObjects, CustomEditor (typeof(UIGrid), true)]
public class UIGirdEditor : Editor
{
	public override void OnInspectorGUI ()
	{
		UIGrid grid = target as UIGrid;
		if (grid)
		{
			grid.ResetPosition();
		}

		/*
		EditorGUILayout.PropertyField (this.serializedObject.FindProperty("GridLayout"), new GUILayoutOption[0]);
		EditorGUILayout.PropertyField (this.serializedObject.FindProperty("Duration"), new GUILayoutOption[0]);
		EditorGUILayout.PropertyField (this.serializedObject.FindProperty("From"), new GUILayoutOption[0]);
		EditorGUILayout.PropertyField (this.serializedObject.FindProperty("To"), new GUILayoutOption[0]);
		*/
	}

	[MenuItem("GameObject/UI/Grid")]
	static void 	CreateGirdEditor()
	{

		GameObject uiGrid = new GameObject("ScrollGird");
		if (uiGrid)
		{
			RectTransform rect = uiGrid.AddComponent<RectTransform>();
			if (!rect)
				throw new System.NullReferenceException();

			// create grid
			GameObject grid = CreateGridScrollRectObject();
			if (grid)
				grid.transform.SetParent(uiGrid.transform);

			if (Selection.activeGameObject)
			{
				uiGrid.transform.SetParent(
					Selection.activeGameObject.transform
					);
			}
		}
	}

	/// <summary>
	/// Creates the grid.
	/// </summary>
	/// <returns>The grid.</returns>
	static GameObject	CreateGridScrollRectObject()
	{
		GameObject gridScrollRect = new GameObject(typeof(UIGrid).Name);
		if (!gridScrollRect)
			throw new System.NullReferenceException();

		RectTransform rect = gridScrollRect.AddComponent<RectTransform>();
		rect.anchorMax 			= new Vector2(0.5f, 0.5f);
		rect.anchorMin 			= new Vector2(0.5f, 0.5f);
		rect.pivot				= new Vector2(0.5f, 0.5f);
		rect.localScale			= Vector3.one;
		rect.localPosition		= Vector3.zero;

		UIGrid grid = gridScrollRect.AddComponent<UIGrid>();
		if (grid)
		{
			GameObject gridLayout = new GameObject(typeof(GridLayoutGroup).Name);
			if (!gridLayout)
				throw new System.NullReferenceException();

			GridLayoutGroup layout = gridLayout.AddComponent<GridLayoutGroup>();
			if (layout)
			{
				ScrollRect scroll = gridScrollRect.GetComponent<ScrollRect>();
				if (scroll)
				{
					scroll.content 		= gridLayout.transform as RectTransform;
					scroll.horizontal	= false;
				}

				layout.cellSize 	= Vector2.one * 100;
				layout.startAxis	= GridLayoutGroup.Axis.Horizontal;

				gridLayout.transform.SetParent(grid.transform);
			}

			grid.transform.SetParent(gridScrollRect.transform);
		}

		return gridScrollRect;
	}
}
