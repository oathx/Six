using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum BoxStyle
{
	YES,
	YESNO,
}

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

	/// <summary>
	/// Unloads the widget.
	/// </summary>
	/// <param name="szName">Size name.</param>
	public void 		UnloadWidget(string szName)
	{
		if (DictWidget.ContainsKey(szName))
		{
			GameObject.Destroy(
				DictWidget[szName].gameObject
				);

#if OPEN_DEBUG_LOG
			Debug.Log("UISystem unload widget " + szName);
#endif
			DictWidget.Remove(szName);
		}
	}

	/// <summary>
	/// Box the specified style, text, callback and args.
	/// </summary>
	/// <param name="style">Style.</param>
	/// <param name="text">Text.</param>
	/// <param name="callback">Callback.</param>
	/// <param name="args">Arguments.</param>
	public void			Box(BoxStyle style, string text, object args, MessageBoxCallback callback)
	{
		IUIBox box = default(IUIBox);

		switch(style)
		{
		case BoxStyle.YES:
			box = LoadWidget<UIYes>(ResourceDef.UI_YES);
			break;
		}

		box.Args 	= args;
		box.Enter 	= callback;

		// set display text
		box.SetText(text);
	}

	/// <summary>
	/// Box the specified text.
	/// </summary>
	/// <param name="text">Text.</param>
	public void 		Box(string text)
	{
		Box(BoxStyle.YES, text, 0, delegate(bool bFlag, object args) {

			// unload box widget
			UnloadWidget(ResourceDef.UI_YES); 

			return true;
		});
	}
}
