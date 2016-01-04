using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using org.critterai.nav;
using org.critterai.nav.u3d;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Logic plugin.
/// </summary>
public class DebugObserver : IEventObserver
{
	public delegate bool	ParseCmd(string szOpt, string[] args);
	
	/// <summary>
	/// The cmd parse.
	/// </summary>
	protected Dictionary<string, ParseCmd> 
		m_dCmdParse = new Dictionary<string, ParseCmd>();
	
	// log buffer
	protected class LogBuffer
	{
		public string 	message
		{ get; set; }
		
		public LogType	type
		{ get; set; }
	}
	
	protected List<LogBuffer> m_dBuffer = new List<LogBuffer>();

	/// <summary>
	/// The debug U.
	/// </summary>
	public UIDebug DebugUI { get; private set; }

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		DebugUI = UISystem.GetSingleton().LoadWidget<UIDebug>(ResourceDef.UI_DEBUG, false);
		if (!DebugUI)
			throw new System.NullReferenceException();

		Application.RegisterLogCallback (OnLogCallback);

		// subscribe loigc gm cmd evnet
		SubscribeEvent (CmdEvent.CMD_DEBUG, OnExecuteCmd);
	}
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	void Start()
	{
		RegisterCmd ("nav", OnNavCmdParsed);
		RegisterCmd ("log", OnLogCmdParsed);
		RegisterCmd ("lua", OnLuaCmdParsed);
		RegisterCmd ("sec", OnSecCmdParsed);
	}

	/// <summary>
	/// Raises the GU event.
	/// </summary>
	void OnGUI()
	{
		if (GUI.Button(new Rect(Screen.width / 2, 0, 50, 50), "Debug"))
		{
			bool visible = !DebugUI.GetVisible();
			DebugUI.SetVisible(visible);
		}
	}
	
	/// <summary>
	/// Registers the cmd.
	/// </summary>
	/// <param name="szCmd">Size cmd.</param>
	/// <param name="callback">Callback.</param>
	protected void RegisterCmd(string szCmd, ParseCmd callback)
	{
		if (!m_dCmdParse.ContainsKey(szCmd))
		{
			m_dCmdParse.Add(szCmd, callback);
		}
	}
	
	/// <summary>
	/// Raises the execute cmd event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool OnExecuteCmd(IEvent evt)
	{
		CmdEvent.DebugCmdEventArgs v = evt.Args as CmdEvent.DebugCmdEventArgs;
		if (!string.IsNullOrEmpty(v.CmdText))
		{
			IGlobalPlugin global = GameEngine.GetSingleton ().QueryPlugin<IGlobalPlugin> ();
			if (!global)
				throw new System.NullReferenceException();
			
			string[] arySplit = v.CmdText.Split(' ');
			if (arySplit.Length > 0)
			{
				string szCmd = arySplit[0];
				if (m_dCmdParse.ContainsKey(szCmd))
				{
					List<string> args = new List<string>();
					for(int i=0; i<arySplit.Length; i++)
						args.Add(arySplit[i]);
					
					if (args.Count > 0)
						args.RemoveAt(0);
					
					// execute the cmd
					m_dCmdParse[szCmd](
						szCmd, args.ToArray()
						);
					
					// echo current cmd
					Echo(LogType.Log, v.CmdText);
				}
				else
				{
					Echo(LogType.Error, "no support the " + szCmd);
				}
			}
		}
		
		return true;
	}
	
	/// <summary>
	/// Convert the specified szOpt and args.
	/// </summary>
	/// <param name="szOpt">Size opt.</param>
	/// <param name="args">Arguments.</param>
	protected int 	Convert(string szOpt, string[] args)
	{
		for(int i=0; i<args.Length; i++)
		{
			if (szOpt == args[i])
				return i;
		}
		
		return -1;
	}
	
	/// <summary>
	/// Echo the specified type and szText.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="szText">Size text.</param>
	protected void	Echo(LogType type, string szText)
	{
		if (DebugUI)
			DebugUI.Append(type, szText);
	}

	/// <summary>
	/// Raises the nav cmd parsed event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	protected bool	OnNavCmdParsed(string szCmd, string[] args)
	{
		if (args.Length > 0)
		{
			string[] aryPm = {
				"-o",
				"-d"
			};
			
			int nCmd = Convert(args[0], aryPm);
			switch(nCmd)
			{
			case 0:
				Dispatcher.RegisterObserver<TestNavmeshObserver>(typeof(TestNavmeshObserver).Name);
				break;
			case 1:
				Dispatcher.UnregisterObserver(typeof(TestNavmeshObserver).Name);
				break;
			}
		}
		
		return true;
	}
	
	/// <summary>
	/// Raises the log cmd parsed event.
	/// </summary>
	/// <param name="szCmd">Size cmd.</param>
	/// <param name="args">Arguments.</param>
	protected bool	OnLogCmdParsed(string szCmd, string[] args)
	{
		if (args.Length > 0)
		{
			string[] aryPm = {
				"-o",
				"-c",
				"-a"
			};
			
			int nCmd = Convert(args[0], aryPm);
			switch(nCmd)
			{
			case 0:
				for(int i=0; i<m_dBuffer.Count; i++)
					Echo(m_dBuffer[i].type, m_dBuffer[i].message);
				
				break;
				
			case 1:
				// clear log buffer
				if (m_dBuffer.Count > 0)
					m_dBuffer.Clear();

				break;
				
			case 2:
				break;
			}
		}
		
		return true;
	}
	
	/// <summary>
	/// Raises the log callback event.
	/// </summary>
	/// <param name="logString">Log string.</param>
	/// <param name="stackTrace">Stack trace.</param>
	/// <param name="type">Type.</param>
	protected void 	OnLogCallback(string logString, string stackTrace, LogType type)
	{
		if (type == LogType.Error || type == LogType.Exception)
		{
			LogBuffer buffer = new LogBuffer ();
			buffer.message 	= System.DateTime.Now.ToString () + ":" + logString.ToString () + "<<" + stackTrace + ">>";
			buffer.type 	= type;
			
			m_dBuffer.Add (buffer);
		}
	}
	
	/// <summary>
	/// Raises the lua cmd parsed event.
	/// </summary>
	/// <param name="szCmd">Size cmd.</param>
	/// <param name="args">Arguments.</param>
	protected bool	OnLuaCmdParsed(string szCmd, string[] args)
	{
		if (args.Length >= 2)
		{
			System.Object[] aryResult = new object[0];
			
			string[] aryPm = {
				"-f",
				"-s",
			};
			
			int nCmd = Convert(args[0], aryPm);
			switch(nCmd)
			{
			case 0:
				aryResult = GameScript.GetSingleton().DoScript(args[1]);
				break;
			case 1:
				string text = string.Empty;
				for(int i=1; i<args.Length; i++)
					text += args[i] + " ";
				
				aryResult = GameScript.GetSingleton().DoString(text);
				break;
			}
			
			if (aryResult != null)
			{
				for(int i=0; i<aryResult.Length; i++)
					Echo(LogType.Log, " result " + (aryResult[i] == null ? "null" : aryResult[i].ToString()));
			}
		}
		
		return true;
	}

	/// <summary>
	/// Raises the sec cmd parsed event.
	/// </summary>
	/// <param name="szCmd">Size cmd.</param>
	/// <param name="args">Arguments.</param>
	protected bool OnSecCmdParsed(string szCmd, string[] args)
	{
		if (args.Length >= 2)
		{
			System.Object[] aryResult = new object[0];
			
			string[] aryPm = {
				"-c",
			};

			int nCmd = Convert(args[0], aryPm);
			switch(nCmd)
			{
			case 0:
				SqlScene sqlScene = GameSqlLite.GetSingleton().Query<SqlScene>(int.Parse(args[1]));
				if (!sqlScene)
					throw new System.NullReferenceException();

				TcpEvent.SCNetSceneChangeReply v = new TcpEvent.SCNetSceneChangeReply();
				v.MapID 	= sqlScene.ID;
				v.Position	= sqlScene.Born;
				v.Angle		= 0;

				GameEngine.GetSingleton().PostEvent(
					new IEvent(EngineEventType.EVENT_NET, TcpEvent.CMD_REPLY_SCENE_CHANGE, v)
					);
				break;
			}
		}

		return true;
	}
}

