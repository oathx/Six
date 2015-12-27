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
	/// Gets the scaler.
	/// </summary>
	/// <value>The scaler.</value>
	public CanvasScaler		Scaler
	{ get; private set; }

	/// <summary>
	/// Gets the user interface rect trans from.
	/// </summary>
	/// <value>The user interface rect trans from.</value>
	public RectTransform 	UIRectTransFrom 
	{ get; private set; }


	/// <summary>
	/// Gets the screen rect.
	/// </summary>
	/// <value>The screen rect.</value>
	public Rect 			ScreenRect
	{ get; private set; }

	/// <summary>
	/// Gets the world camera.
	/// </summary>
	/// <value>The world camera.</value>
	public Camera 			UGuiCamera
	{ get; private set; }

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		gameObject.layer = LayerMask.NameToLayer("UI");

		Canvas canvas = GetComponent<Canvas>();
		if (!canvas)
			throw new System.NullReferenceException();

		GameObject go = new GameObject(typeof(Camera).Name);
		if (go)
		{
			GameObject.DontDestroyOnLoad(go);

			UGuiCamera = go.AddComponent<Camera>();
			if (!UGuiCamera)
				throw new System.NullReferenceException();

			UGuiCamera.cullingMask = 1 << gameObject.layer;
			UGuiCamera.depth 		= short.MaxValue;
			UGuiCamera.clearFlags 	= CameraClearFlags.Depth;

			canvas.worldCamera		= UGuiCamera;
			canvas.renderMode 		= RenderMode.ScreenSpaceCamera;
		}
		else
		{
			canvas.renderMode 		= RenderMode.ScreenSpaceOverlay;
		}

		Scaler = GetComponent<CanvasScaler>();
		if (!Scaler)
			throw new System.NullReferenceException();

		Scaler.uiScaleMode 	= CanvasScaler.ScaleMode.ScaleWithScreenSize;
		Scaler.referenceResolution = new Vector2(960, 640);

		UIRectTransFrom = GetComponent<RectTransform>();
		ScreenRect 		= UIRectTransFrom.rect;
	}

	/// <summary>
	/// Raises the destroy event.
	/// </summary>
	void OnDestroy()
	{
		Debug.Log("**************************** ui system close *****************************");
	}

	public virtual T	LoadWidget<T>(string szResourceName, Transform parent, string szName, bool visible) where T : IUIWidget
	{
		string szWidgetName = string.IsNullOrEmpty(szName) ? szResourceName : szName;

		if (DictWidget.ContainsKey(szWidgetName))
			return DictWidget[szWidgetName] as T;
		
		Object resource = Resources.Load<Object>(szResourceName);
		if (!resource)
			throw new System.NullReferenceException();
		
		GameObject widget = GameObject.Instantiate(resource) as GameObject;
		if (widget)
		{
			widget.name = szWidgetName;
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
				
				DictWidget.Add(szWidgetName, cmp);
			}
		}
		
		return DictWidget[szWidgetName] as T;
	}

	/// <summary>
	/// Load the specified szName.
	/// </summary>
	/// <param name="szName">Size name.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public virtual T	LoadWidget<T>(string szName, Transform parent, bool visible) where T : IUIWidget
	{
		return LoadWidget<T>(szName, parent, string.Empty, visible);
	}

	/// <summary>
	/// Gets the widget.
	/// </summary>
	/// <returns>The widget.</returns>
	/// <param name="szName">Size name.</param>
	public IUIWidget	GetWidget(string szName)
	{
		return DictWidget[szName];
	}

	/// <summary>
	/// Query the specified szName.
	/// </summary>
	/// <param name="szName">Size name.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public T			Query<T>(string szName) where T : IUIWidget
	{
		return (T)GetWidget(szName);
	}

	/// <summary>
	/// Gets the camera.
	/// </summary>
	/// <returns>The camera.</returns>
	public Camera		GetCamera()
	{
		return UGuiCamera;
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
	/// <param name="type">Type.</param>
	/// <param name="szName">Size name.</param>
	/// <param name="parent">Parent.</param>
	/// <param name="visible">If set to <c>true</c> visible.</param>
	public IUIWidget	LoadWidget(System.Type type, string szName, Transform parent, bool visible)
	{
		if (DictWidget.ContainsKey(szName))
			return DictWidget[szName];
		
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
			
			IUIWidget cmp = widget.AddComponent(type) as IUIWidget;
			if (cmp) 
			{
				cmp.SetVisible(
					visible
					);

				DictWidget.Add(szName, cmp);
			}
		}
		
		return DictWidget[szName];
	}

	/// <summary>
	/// Loads the name of the widget from type.
	/// </summary>
	/// <returns>The widget from type name.</returns>
	/// <param name="typeName">Type name.</param>
	/// <param name="szResourceName">Size resource name.</param>
	/// <param name="szName">Size name.</param>
	public IUIWidget	LoadWidgetFromTypeName(string typeName, string szName, bool bVisible)
	{
		// get the widget type
		System.Type type = TypeHelp.GetType (typeName);

		// create widget
		return LoadWidget (type, szName, transform, bVisible);
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
	/// Loads the widget.
	/// </summary>
	/// <returns>The widget.</returns>
	/// <param name="szResourceName">Size resource name.</param>
	/// <param name="visible">If set to <c>true</c> visible.</param>
	/// <param name="szName">Size name.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public virtual T	LoadWidget<T>(string szResourceName, bool visible, string szName) where T : IUIWidget
	{
		return LoadWidget<T>(szResourceName, default(Transform), szName, visible);
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
	/// Worlds to user interface point.
	/// </summary>
	/// <returns><c>true</c>, if to user interface point was worlded, <c>false</c> otherwise.</returns>
	/// <param name="vScreen">V screen.</param>
	/// <param name="rect">Rect.</param>
	/// <param name="vResult">V result.</param>
	public Vector3		WorldToUIPoint(Vector3 vScreen, RectTransform rect)
	{
		if (!UGuiCamera)
			return Vector3.zero;

		Vector3 vResult = Vector3.zero;

		// convert world position to ui postion
		bool bFlag = RectTransformUtility.ScreenPointToWorldPointInRectangle(
			rect, vScreen, UGuiCamera, out vResult
			);

		return vResult;
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

		case BoxStyle.YESNO:
			box = LoadWidget<UIYesNo>(ResourceDef.UI_YESNO);
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

	/// <summary>
	/// Gets the scale.
	/// </summary>
	/// <returns>The scale.</returns>
	public Vector2		GetReferenceResolution()
	{
		return Scaler.referenceResolution;
	}
}
