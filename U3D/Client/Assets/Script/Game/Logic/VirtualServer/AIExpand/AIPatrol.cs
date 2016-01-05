using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AI
{
	/// <summary>
	/// AI speak.
	/// </summary>
	public class AIPatrol : AIBehaviour
	{
		public float					Radius;
		public float					ErrorRange;
		public string					Return;
		
		/// <summary>
		/// Gets the machine.
		/// </summary>
		/// <value>The machine.</value>
		public IAIMachine				Machine
		{ get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="IRandomSelector"/> class.
		/// </summary>
		public AIPatrol()
			: base()
		{
			
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

			// set current patrol target
			if (ec.Leader)
			{
				float fDistance = Vector3.Distance(ec.Owner.GetPosition(), ec.Leader.GetPosition());
				if (fDistance > Radius)
				{
					Vector3 vTarget = Vector3.zero;
					
					// get random target point
					if (SceneSupport.GetSingleton().GetRandomPosition(ec.Leader.GetPosition(), Radius, ref vTarget))
					{
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
					}
				}
			}
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





