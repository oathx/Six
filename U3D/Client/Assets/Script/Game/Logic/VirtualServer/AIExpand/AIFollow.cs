using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AI
{
	/// <summary>
	/// AI speak.
	/// </summary>
	public class AIFollow : AIBehaviour
	{
		public float					Radius;
		public float					Distance;
		public float					ErrorRange;
		
		/// <summary>
		/// Gets the machine.
		/// </summary>
		/// <value>The machine.</value>
		public IAIMachine				Machine
		{ get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="IRandomSelector"/> class.
		/// </summary>
		public AIFollow()
			: base()
		{
			
		}

		/// <summary>
		/// Cacls the length of the path.
		/// </summary>
		/// <returns>The path length.</returns>
		/// <param name="vPath">V path.</param>
		public virtual bool			CalcTargetPoint(List<Vector3> vPath, ref Vector3 vTarget, float fTestDistance)
		{
			if (vPath.Count < 2)
				return false;

			int nLastIndex 	= vPath.Count - 1;
			int nStart 		= nLastIndex;
			int nEnd		= 0;
			float fDistance	= 0;

			while(nStart > nEnd)
			{
				fDistance += Vector3.Distance(vPath[nStart], vPath[--nStart]);
				if (fDistance > fTestDistance)
				{
					float fLerp = (fDistance - fTestDistance * 0.5f ) / fDistance;
					vTarget = Vector3.Lerp(vPath[nStart], vPath[nLastIndex], fLerp);

					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Follows the target.
		/// </summary>
		/// <param name="ec">Ec.</param>
		public virtual bool 			FollowTarget(AIEntityContext ec)
		{
			Vector3 vStart	= ec.Owner.GetPosition();
			Vector3 vEnd	= ec.Leader.GetPosition();
			Vector3 vTarget	= vStart;
			float	fError	= Distance > 0 ? Distance : Radius;

			// If the current is in the following range
			float fDistance = Vector3.Distance(vStart, vEnd);
			if (fError >= fDistance)
				return true;

			// If you set the distance, then calculate the point within this range.
			if (Distance > 0)
			{
				List<Vector3>
					vPath = new List<Vector3>();
				
				bool bResult = SceneSupport.GetSingleton().FindPath(vStart, vEnd, 0, ref vPath);
				if (bResult)
					CalcTargetPoint(vPath, ref vEnd, Distance);
			}

			// if set random radius then get a random position
			if (Radius > 0)
				SceneSupport.GetSingleton().GetRandomPosition(vEnd, Radius, ref vTarget);

			// change to find path state
			IAIState curState = Machine.GetCurrentState ();
			if (curState.StateID != AITypeID.AI_PATH)
				Machine.ChangeState (AITypeID.AI_PATH);
			
			// construct find path event
			CmdEvent.AIFindPathEventArgs v = new CmdEvent.AIFindPathEventArgs ();
			v.Target 		= vTarget;
			v.MinDistance 	= ErrorRange;
			v.DrawLine		= true;
			v.LineWidth		= 0.05f;
			
			// post the find path event to machine
			Machine.PostEvent(
				new IEvent(EngineEventType.EVENT_AI, CmdEvent.CMD_LOGIC_AIFINDPATH, v)
				);

			return true;
		}
		
		/// <summary>
		/// Raises the start event.
		/// </summary>
		/// <param name="context">Context.</param>
		public override void 			OnStart(AIContext context)
		{
			base.OnStart (context);
			
			AIEntityContext ec = context as AIEntityContext;
			if (!ec.Owner)
				throw new System.NullReferenceException ();
			
			Machine = ec.Owner.GetMachine ();
			if (!Machine)
				throw new System.NullReferenceException();
	
			// follow target
			if (ec.Leader)
				FollowTarget(ec);
		}
		
		/// <summary>
		/// Raises the update event.
		/// </summary>
		/// <param name="context">Context.</param>
		public override BehaviourStatus	OnUpdate(AIContext context)
		{
			AIEntityContext ec = context as AIEntityContext;
			if (!ec.Owner || !ec.Leader)
				return BehaviourStatus.FAILURE;
			
			IAIState curState = Machine.GetCurrentState();
			if (curState.StateID != AITypeID.AI_PATH)
			{
				return BehaviourStatus.SUCCESS;
			}
			
			return BehaviourStatus.RUNNING;
		}
	}
}






