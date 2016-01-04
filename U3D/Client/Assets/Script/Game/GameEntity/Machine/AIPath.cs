using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// I entity movable.
/// </summary>
public class AIPath : AIBaseState
{
	protected List<Vector3>		m_dWaypoint = new List<Vector3> ();
	protected float				m_fMinDistance = 1.0f;
	protected Vector3			m_LastPosition = Vector3.zero;
	/// <summary>
	/// Gets or sets the draw.
	/// </summary>
	/// <value>The draw.</value>
	public GameObject			Draw
	{ get; set; }
	
	/// <summary>
	/// Initializes a new instance of the <see cref="IdleState"/> class.
	/// </summary>
	/// <param name="nStateID">N state I.</param>
	/// <param name="entity">Entity.</param>
	public AIPath(int nStateID, IEntity entity)
		: base(nStateID, entity)
	{
		
	}
	
	/// <summary>
	/// Draws the line.
	/// </summary>
	/// <param name="aryPosition">Ary position.</param>
	protected void 			DrawLine(List<Vector3> aryPosition, float fWidth)
	{
		Draw = new GameObject(typeof(AIPath).Name);
		if (!Draw)
			throw new System.NullReferenceException();
		
		LineRenderer draw = Draw.AddComponent<LineRenderer>();
		if (draw)
		{
			draw.material = new Material(
				Shader.Find("Particles/Alpha Blended")
				);
			
			draw.SetWidth(fWidth, fWidth);
			draw.SetVertexCount(aryPosition.Count);
			draw.SetColors(Color.red, Color.red);
			
			for(int idx=0; idx<aryPosition.Count; idx++)
			{
				draw.SetPosition(idx, aryPosition[idx]);
			}
		}
	}
	
	/// <summary>
	/// Raises the start event.
	/// </summary>
	public override bool 	OnStart()
	{
		base.OnStart ();

		int[] aryLayer = {
			EntityLayer.PLAYER,
			EntityLayer.MAIN
		};
		foreach(int layer in aryLayer)
		{
			Physics.IgnoreLayerCollision(EntityLayer.MONSTER, layer, true);
		}
		
		SqlShape sqlShape = GameSqlLite.GetSingleton().Query<SqlShape>(Self.ShapeID);
		if (sqlShape)
		{
			if (Self.GetApplyRootMotion())
				Self.ApplyRootMotion(false);
			
			PlayAction(sqlShape.Move);
		}

		return true;
	}
	
	/// <summary>
	/// Raises the update event.
	/// </summary>
	public override bool 	OnUpdate(float fElapsed)
	{
		bool bUpdateResult = base.OnUpdate (fElapsed);
		if (bUpdateResult)
			return true;

		if (m_dWaypoint.Count > 0 && m_LastPosition != Self.GetPosition())
		{
			float fMaxErrorDistance = m_dWaypoint.Count == 1 ? m_fMinDistance : 1.0f;
			
			// get entity current position
			Vector3 vSource = Self.GetPosition();
			
			// calc remain move distance
			float fDistance = Vector3.Distance(vSource, m_dWaypoint[0]);
			if (fDistance > fMaxErrorDistance)
			{
				// move entity to target waypoint
				Self.Move(m_dWaypoint[0], Self.MaxMoveSpeed);
				
				// ctrl current direction
				Self.RotateTo(
					MathfEx.Direction2Angle(m_dWaypoint[0] - vSource) * Vector3.up, Self.MaxRotateSpeed);

				m_LastPosition = Self.GetPosition();
			}
			else
			{
				m_dWaypoint.RemoveAt(0);
			}
		}
		else
		{
			Self.GetMachine().ChangeState(AITypeID.AI_IDLE);
		}

		return true;
	}
	
	/// <summary>
	/// Raises the exit event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	public override bool	OnExit()
	{
		m_dWaypoint.Clear();
		m_LastPosition = Vector3.zero;

		if (!Self.GetApplyRootMotion())
			Self.ApplyRootMotion(true);
		
		// stop move
		Self.Move (Vector3.zero, 0);
		
		int[] aryLayer = {
			EntityLayer.PLAYER,
			EntityLayer.MAIN
		};
		foreach(int layer in aryLayer)
		{
			Physics.IgnoreLayerCollision(EntityLayer.MONSTER, layer, false);
		}
		
		if (Draw)
			GameObject.Destroy(Draw);
		
		return base.OnExit();
	}
	
	/// <summary>
	/// Raises the event event.
	/// </summary>
	/// <param name="evt">Evt.</param>
	public override bool 	OnEvent(IEvent evt)
	{
		base.OnEvent (evt);
		
		switch(evt.ID)
		{
		case CmdEvent.CMD_LOGIC_AIPATH:
			return OnLogicAIPath(evt);
			
		case CmdEvent.CMD_LOGIC_AIFINDPATH:
			return OnLogicAIFindPath(evt);
		}
		
		return true;
	}
	
	/// <summary>
	/// Raises the logic AI path event.
	/// </summary>
	/// <param name="">.</param>
	protected bool			OnLogicAIPath(IEvent evt)
	{
		CmdEvent.AIPathEventArgs v = evt.Args as CmdEvent.AIPathEventArgs;
		if (v.Path.Count > 0)
		{
			m_fMinDistance = v.MinDistance;
			
			// clear old point
			m_dWaypoint.Clear();
			
			// add new way point
			m_dWaypoint.AddRange(v.Path);
		}
		
		return true;
	}
	
	/// <summary>
	/// Raises the logic AI find path event.
	/// </summary>
	/// <param name="">.</param>
	protected bool			OnLogicAIFindPath(IEvent evt)
	{
		CmdEvent.AIFindPathEventArgs v = evt.Args as CmdEvent.AIFindPathEventArgs;
		
		// get self current position
		Vector3 vSource = Self.GetPosition ();
		
		// find path
		List<Vector3> vPath = new List<Vector3> ();
		bool bResult = SceneSupport.GetSingleton ().FindPath (vSource, v.Target, 0, ref vPath);
		if (bResult)
		{
			if (v.DrawLine)
				DrawLine(vPath, v.LineWidth);
			
			m_fMinDistance 	= v.MinDistance;
			
			// clear old way point
			m_dWaypoint.Clear();
			
			// add new way point
			m_dWaypoint.AddRange(vPath);
			
		}
		else
		{
			#if UNITY_EDITOR
			Debug.LogWarning(string.Format("Can't find path start={0} target={1}", vSource.ToString(), v.Target.ToString()));
			#endif
		}
		
		return true;
	}
}

