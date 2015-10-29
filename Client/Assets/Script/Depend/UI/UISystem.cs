using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum BoxStyle
{
	YES,
	YESNO,
	STATUS,
}

[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(CanvasScaler))]
[RequireComponent(typeof(GraphicRaycaster))]
[RequireComponent(typeof(EventSystem))]
[RequireComponent(typeof(StandaloneInputModule))]
[RequireComponent(typeof(TouchInputModule))]
public class UISystem : MonoBehaviourSingleton<UISystem>
{
	/// <summary>
	/// The dict widget.
	/// </summary>
	public Dictionary<string, IUIWidget> 
		DictWidget = new Dictionary<string, IUIWidget>();

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		gameObject.layer = LayerMask.NameToLayer("UI");

		Canvas canvas = GetComponent<Canvas>();
		if (canvas)
		{
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
		}

		CanvasScaler scaler = GetComponent<CanvasScaler>();
		scaler.uiScaleMode 	= CanvasScaler.ScaleMode.ScaleWithScreenSize;
		scaler.referenceResolution = new Vector2(960, 640);
	}

	/// <summary>
	/// Load the specified szName.
	/// </summary>
	/// <param name="szName">Size name.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public virtual T	LoadWidget<T>(string szName, Transform parent, bool visible) where T : IUIWidget
	{
		if (DictWidget.ContainsKey(szName))
			return DictWidget[szName] as T;

		Object resource = Resources.Load<Object>(szName);
		if (!resource)
			throw new System.NullReferenceException();

		GameObject widget = GameObject.Instantiate(resource) as GameObject;
		if (widget)
		{
			widget.name = szName;
			widget.transform.SetParent(parent ? parent : transform);

			RectTransform rt = widget.GetComponent<RectTransform>();
			rt.offsetMax 	= Vector2.zero;
			rt.offsetMin 	= Vector2.zero;
			rt.localScale	= Vector3.one;
			rt.localPosition= Vector3.zero;

			T cmp = widget.AddComponent<T>();
			if (cmp)
			{
				cmp.SetVisible(
					visible
					);

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
		return LoadWidget<T>(szName, true);
	}

	/// <summary>
	/// Loads the widget.
	/// </summary>
	/// <returns>The widget.</returns>
	/// <param name="szName">Size name.</param>
	/// <param name="visible">If set to <c>true</c> visible.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public virtual T	LoadWidget<T>(string szName, bool visible) where T : IUIWidget
	{
		return LoadWidget<T>(szName, default(Transform), visible);
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

		case BoxStyle.STATUS:
			box = LoadWidget<UIStatus>(ResourceDef.UI_STATUS);
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

	/// <summary>
	/// Box the specified text.
	/// </summary>
	/// <param name="text">Text.</param>
	public void 		ShowStatus(string text)
	{
		Box(BoxStyle.STATUS, text, 0, delegate(bool bFlag, object args) {
			return true;
		});
	}

	/// <summary>
	/// Hides the status.
	/// </summary>
	public void 		HideStatus()
	{
		// unload box widget
		UnloadWidget(ResourceDef.UI_STATUS); 
	}
}
