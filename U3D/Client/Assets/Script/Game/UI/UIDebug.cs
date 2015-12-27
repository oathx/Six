using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;

/// <summary>
/// User interface version.
/// </summary>
public class UIDebug : IUIWidget
{
	public const string	UD_TEXT 	= "UD_TEXT";
	public const string UD_SUBMIT	= "UD_SUBMIT";
	public const string UD_INPUT	= "UD_INPUT";

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Install(new string[]{
			UD_TEXT,
			UD_SUBMIT,
			UD_INPUT,
		});
	}

	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		RegisterClickEvent(UD_SUBMIT, OnSumbitClicked);
	}

	/// <summary>
	/// Append the specified type and text.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="text">Text.</param>
	public void 	Append(LogType type, string text)
	{
		Text outText = GetChildComponent<Text>(UD_TEXT);
		if (outText)
		{
			string newText = string.Empty;
			switch(type)
			{
			case LogType.Error:
				newText = string.Format("<color=#ff0000ff>Cmd >> {0}</color>", text);
				break;
			case LogType.Exception:
				newText = string.Format("<color=#ffff00ff>Cmd >> {0}</color>", text);
				break;
			case LogType.Log:
				newText = string.Format("<color=#ffffffff>Cmd >> {0}</color>", text);
				break;
			}

			StringBuilder builder = new StringBuilder();
			builder.AppendLine(outText.text);
			builder.AppendLine(newText);

			outText.text = builder.ToString();
		}
	}

	/// <summary>
	/// Raises the sumbit clicked event.
	/// </summary>
	/// <param name="goSend">Go send.</param>
	/// <param name="evtData">Evt data.</param>
	protected void OnSumbitClicked(GameObject goSend, BaseEventData evtData)
	{
		CmdEvent.DebugCmdEventArgs v = new CmdEvent.DebugCmdEventArgs();
		v.CmdText = GetInputText(UD_INPUT);

		GameEngine.GetSingleton().PostEvent(
			new IEvent(CmdEvent.CMD_DEBUG, v)
			);
	}
}
