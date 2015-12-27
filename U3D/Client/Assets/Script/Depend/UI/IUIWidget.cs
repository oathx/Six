using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using NLua;

/// <summary>
/// IUI widget.
/// </summary>
public class IUIWidget : MonoBehaviour
{
	public Dictionary<string, GameObject> Child = new Dictionary<string, GameObject>();

	/// <summary>
	/// Install the specified aryName.
	/// </summary>
	/// <param name="aryName">Ary name.</param>
	public void Install(string[] aryName)
	{
		Transform[] aryTransform = GetComponentsInChildren<Transform>();
		foreach(Transform child in aryTransform)
		{
			foreach(string curName in aryName)
			{
				if (child.name == curName)
				{
#if OPEN_DEBUG_LOG
					Debug.Log("widget install child name : " + curName);
#endif
					Child.Add(
						curName, child.gameObject
						);
				}
			}
		}
	}

	/// <summary>
	/// Active this instance.
	/// </summary>
	public virtual bool Show()
	{
		SetVisible(true); return gameObject.activeSelf;
	}

	/// <summary>
	/// Detive this instance.
	/// </summary>
	public virtual void Hide()
	{
		SetVisible(false);
	}

	/// <summary>
	/// Sets the visible.
	/// </summary>
	/// <param name="szName">Size name.</param>
	/// <param name="bVisible">If set to <c>true</c> b visible.</param>
	public void Show(string szName, bool bVisible)
	{
		Child[szName].SetActive(bVisible);
	}

	/// <summary>
	/// Sets the visible.
	/// </summary>
	/// <param name="bVisible">If set to <c>true</c> b visible.</param>
	public void SetVisible(bool bVisible)
	{
		gameObject.SetActive(bVisible);
	}

	/// <summary>
	/// Gets the visible.
	/// </summary>
	/// <returns><c>true</c>, if visible was gotten, <c>false</c> otherwise.</returns>
	public bool	GetVisible()
	{
		return gameObject.activeSelf;
	}

	/// <summary>
	/// Registers the click event.
	/// </summary>
	/// <param name="szName">Size name.</param>
	public void RegisterClickEvent(string szName, IUIEventTriggerListener.VoidDelegate callback)
	{
		IUIEventTriggerListener.Get(Child[szName]).onClick = callback;
	}

	/// <summary>
	/// Registers the click event.
	/// </summary>
	/// <param name="go">Go.</param>
	/// <param name="callback">Callback.</param>
	public void RegisterClickEvent(GameObject target, IUIEventTriggerListener.VoidDelegate callback)
	{
		IUIEventTriggerListener.Get(target).onClick = callback;
	}

	/// <summary>
	/// Registers the begin drag event.
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="callback">Callback.</param>
    public void RegisterBeginDragEvent(GameObject target, IUIEventTriggerListener.VoidDelegate callback)
    {
        IUIEventTriggerListener.Get(target).onBeginDrag = callback;
    }

	/// <summary>
	/// Registers the begin drag event.
	/// </summary>
	/// <param name="szName">Size name.</param>
	/// <param name="callback">Callback.</param>
    public void RegisterBeginDragEvent(string szName, IUIEventTriggerListener.VoidDelegate callback)
    {
        IUIEventTriggerListener.Get(Child[szName]).onBeginDrag = callback;
    }

	/// <summary>
	/// Registers the drag event.
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="callback">Callback.</param>
    public void RegisterDragEvent(GameObject target, IUIEventTriggerListener.VoidDelegate callback)
    {
        IUIEventTriggerListener.Get(target).onDrag = callback;
    }

	/// <summary>
	/// Registers the drag event.
	/// </summary>
	/// <param name="szName">Size name.</param>
	/// <param name="callback">Callback.</param>
    public void RegisterDragEvent(string szName, IUIEventTriggerListener.VoidDelegate callback)
    {
        IUIEventTriggerListener.Get(Child[szName]).onDrag = callback;
    }

	/// <summary>
	/// Registers the end drag event.
	/// </summary>
	/// <param name="target">Target.</param>
	/// <param name="callback">Callback.</param>
    public void RegisterEndDragEvent(GameObject target, IUIEventTriggerListener.VoidDelegate callback)
    {
        IUIEventTriggerListener.Get(target).onEndDrag = callback;
    }

	/// <summary>
	/// Registers the end drag event.
	/// </summary>
	/// <param name="szName">Size name.</param>
	/// <param name="callback">Callback.</param>
    public void RegisterEndDragEvent(string szName, IUIEventTriggerListener.VoidDelegate callback)
    {
        IUIEventTriggerListener.Get(Child[szName]).onEndDrag = callback;
    }

    /// <summary>
    /// Adds the child.
    /// </summary>
    /// <returns>The child.</returns>
    /// <param name="go">Go.</param>
    /// <param name="prefab">Prefab.</param>
    /// <typeparam name="T">The 1st type parameter.</typeparam>
    public T	AddChild<T>(string widgetName, GameObject prefab) where T : IUIWidget
	{
		if (Child.ContainsKey(widgetName))
		{
			GameObject child = GameObject.Instantiate(prefab) as GameObject;
			if (child)
			{
				child.transform.parent 			= Child[widgetName].transform;
				child.transform.localScale 		= Vector3.one;
				child.transform.name			= prefab.name;
				child.transform.position		= Vector3.zero;
				child.transform.localPosition	= Vector3.zero;

				T cmp = child.AddComponent<T>();
				if (!cmp)
					throw new System.NullReferenceException();

				return cmp;
			}
		}

		return default(T);
	}

	/// <summary>
	/// Gets the child component.
	/// </summary>
	/// <returns>The child component.</returns>
	/// <param name="szName">Size name.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public T	GetChildComponent<T>(string szName) where T : Component
	{
		return Child[szName].GetComponent<T>();
	}

	/// <summary>
	/// Sets the text.
	/// </summary>
	/// <param name="szName">Size name.</param>
	/// <param name="text">Text.</param>
	public void 	SetText(string szName, string text)
	{
		Text lab = Child[szName].GetComponent<Text>();
		if (lab)
			lab.text = text;
	}

	/// <summary>
	/// Gets the text.
	/// </summary>
	/// <returns>The text.</returns>
	/// <param name="szName">Size name.</param>
	public string 	GetText(string szName)
	{
		Text lab = Child[szName].GetComponent<Text>();
		if (lab)
			return lab.text;

		return string.Empty;
	}

	/// <summary>
	/// Gets the input text.
	/// </summary>
	/// <returns>The input text.</returns>
	/// <param name="szName">Size name.</param>
	public string	GetInputText(string szName)
	{
		InputField ipt = GetChildComponent<InputField>(szName);
		if (ipt)
			return ipt.text;

		return string.Empty;
	}

	/// <summary>
	/// Sets the input text.
	/// </summary>
	/// <param name="szName">Size name.</param>
	/// <param name="text">Text.</param>
	public void 	SetInputText(string szName, string text)
	{
		InputField ipt = GetChildComponent<InputField>(szName);
		if (ipt)
			ipt.text = text;
	}

	/// <summary>
	/// Sets the slider.
	/// </summary>
	/// <param name="szName">Size name.</param>
	/// <param name="fProgress">F progress.</param>
	public void 	SetSlider(string szName, float fProgress)
	{
		Slider slider = GetChildComponent<Slider>(szName);
		if (slider)
			slider.value = fProgress;
	}

	/// <summary>
	/// Sets the disibled.
	/// </summary>
	/// <param name="szName">Size name.</param>
	/// <param name="disibled">If set to <c>true</c> disibled.</param>
	public void 	SetDisibled(string szName, bool disibled)
	{
		Button b = GetChildComponent<Button>(szName);
		if (b)
			b.enabled = disibled;
	}
}
