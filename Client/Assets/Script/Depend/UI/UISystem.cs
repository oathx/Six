using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
[RequireComponent(typeof(GraphicRaycaster))]
public class UISystem : MonoBehaviourSingleton<UISystem>
{
	public Dictionary<string, IUIWidget> DictWidget = new Dictionary<string, IUIWidget>();

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Canvas canvas = GetComponent<Canvas>();
		if (canvas)
		{
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		}
	}

	/// <summary>
	/// Load the specified szName.
	/// </summary>
	/// <param name="szName">Size name.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public virtual T	LoadWidget<T>(string szName, Transform parent) where T : IUIWidget
	{
		if (DictWidget.ContainsKey(szName))
			return DictWidget[szName] as T;

		Object resource = Resources.Load<Object>(szName);
		if (!resource)
			throw new System.NullReferenceException();

		GameObject widget = GameObject.Instantiate(resource) as GameObject;
		if (widget)
		{
			widget.name 	= szName;
			widget.transform.SetParent(parent ? parent : transform);

			RectTransform rt = widget.GetComponent<RectTransform>();
			rt.offsetMax = Vector2.zero;
			rt.offsetMin = Vector2.zero;

			T cmp = widget.AddComponent<T>();
			if (cmp)
			{
				DictWidget.Add(szName, cmp);
			}
		}

		return DictWidget[szName] as T;
	}

	/// <summary>
	/// Loads the widget.
	/// </summary>
	/// <returns>The widget.</returns>
	/// <param name="szName">Size name.</param>
	/// <param name="parent">Parent.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public virtual T	LoadWidget<T>(string szName) where T : IUIWidget
	{
		return LoadWidget<T>(szName, default(Transform));
	}
}
