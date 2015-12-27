using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

/// <summary>
/// Game engine.
/// </summary>
public class GameTimer : MonoBehaviourSingleton<GameTimer>
{
	public abstract class TimerStruct
	{
		/// <summary>
		/// Gets or sets the timer I.
		/// </summary>
		/// <value>The timer I.</value>
		public int 			timerID
		{ get; set; }
		
		/// <summary>
		/// Gets or sets the interval.
		/// </summary>
		/// <value>The interval.</value>
		public float		interval
		{ get; set; }
		
		/// <summary>
		/// Gets or sets the elapsed.
		/// </summary>
		/// <value>The elapsed.</value>
		public float		elapsed
		{ get; set ;}
		
		/// <summary>
		/// Gets or sets the repeat.
		/// </summary>
		/// <value>The repeat.</value>
		public int 			repeat
		{ get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ITimerPlugin+TimerInfo"/> class.
		/// </summary>
		/// <param name="nID">N I.</param>
		/// <param name="fTime">F time.</param>
		/// <param name="nRepat">N repat.</param>
		/// <param name="func">Func.</param>
		public TimerStruct(int nID, float fInterval, int nRepat)
		{
			interval 	= fInterval;
			elapsed 	= 0.0f;
			repeat 		= nRepat;
			timerID 	= nID;
		}

		public abstract void 	Call();
	}
	
	public class LuaScriptTimer : TimerStruct
	{
		/// <summary>
		/// Gets or sets the func.
		/// </summary>
		/// <value>The func.</value>
		public NLua.LuaFunction Func
		{ get; set; }
		
		/// <summary>
		/// Gets or sets the arguments.
		/// </summary>
		/// <value>The arguments.</value>
		public object[] 		Args
		{ get; set; }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="LuaScriptTimer"/> class.
		/// </summary>
		/// <param name="interval">Interval.</param>
		/// <param name="bAutoReset">If set to <c>true</c> b auto reset.</param>
		/// <param name="luaFunc">Lua func.</param>
		/// <param name="args">Arguments.</param>
		public LuaScriptTimer(int nID, float fInterval, int nRepat, NLua.LuaFunction luaFunc, params object[] args) 
			: base(nID, fInterval, nRepat)
		{
			Func 		= luaFunc;
			Args 		= args;
		}

		/// <summary>
		/// Call this instance.
		/// </summary>
		public override void 	Call()
		{
			Func.Call(Args);
		}
	}

	/// <summary>
	/// The m_d timer.
	/// </summary>
	protected List<TimerStruct> 
		m_dTimer = new List<TimerStruct>();

	/// <summary>
	/// Add the specified nTimerID, fInterval, nRepat, luaFunc and args.
	/// </summary>
	/// <param name="nTimerID">N timer I.</param>
	/// <param name="fInterval">F interval.</param>
	/// <param name="nRepat">N repat.</param>
	/// <param name="luaFunc">Lua func.</param>
	/// <param name="args">Arguments.</param>
	public void Add(int nTimerID, float fInterval, int nRepat, NLua.LuaFunction luaFunc, params object[] args)
	{
		TimerStruct timer = m_dTimer.Find(delegate(TimerStruct obj) {
			return nTimerID == obj.timerID;
		});
		if (timer == null)
		{
			m_dTimer.Add(new LuaScriptTimer(nTimerID, fInterval, nRepat, luaFunc, args));
		}
	}

	/// <summary>
	/// Remove the specified nTimerID.
	/// </summary>
	/// <param name="nTimerID">N timer I.</param>
	public void Remove(int nTimerID)
	{
		for(int i=0; i<m_dTimer.Count; i++)
		{
			if (m_dTimer[i].timerID == nTimerID)
			{
				m_dTimer.RemoveAt(i);
				break;
			}
		}
	}

	/// <summary>
	/// Clearup this instance.
	/// </summary>
	public void Clearup()
	{
		m_dTimer.Clear();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update()
	{
		for(int i=0; i<m_dTimer.Count; i++)
		{
			TimerStruct info = m_dTimer[i];
			if (info.elapsed >= info.interval)
			{	
				info.Call();

				info.repeat = Mathf.Max(--info.repeat, 0);
				
				// reset the timer count
				if (info.repeat > 0)
					info.elapsed = 0.0f;
				
				// remove the timer
				if (info.repeat <= 0)
					m_dTimer.RemoveAt(i);
			}
			else
			{
				info.elapsed += Time.deltaTime;
			}
		}
	}


}
