using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AI
{
	/// <summary>
	/// AI speak.
	/// </summary>
	public class AIPursue : AIBehaviour
	{
		public float					MaxDistance;
		
		/// <summary>
		/// Gets the machine.
		/// </summary>
		/// <value>The machine.</value>
		public IAIMachine				Machine
		{ get; private set; }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="IRandomSelector"/> class.
		/// </summary>
		public AIPursue()
			: base()
		{
			
		}
		
		/// <summary>
		/// Puesues the target.
		/// </summary>
		/// <param name="vCenter">V center.</param>
		/// <param name="fRadius">F radius.</param>
		public virtual void 			PuesueTarget(Vector3 vCenter, float fRadius)
		{
			Vector3 vTarget = SceneSupport.GetSingleton ().GetRandomPosition (vCenter, fRadius);
			if (vTarget != Vector3.zero)
			{
				// change to find path state
				IAIState curState = Machine.GetCurrentState ();
				if (curState.StateID != AITypeID.AI_PATH)
					Machine.ChangeState (AITypeID.AI_PATH);
				
				// construct find path event
				CmdEvent.AIFindPathEventArgs v = new CmdEvent.AIFindPathEventArgs ();
				v.Target 		= vTarget;
				v.MinDistance 	= fRadius;
				v.DrawLine		= true;
				v.LineWidth		= 0.1f;
				
				// post the find path event to machine
				Machine.PostEvent(
					new IEvent(EngineEventType.EVENT_AI, CmdEvent.CMD_LOGIC_AIFINDPATH, v)
					);
			}
		}
		
		/// <summary>
		/// Raises the start event.
		/// </summary>
		/// <param name="context">Context.</param>
		public override void 			OnStart(AIContext context)
		{
			base.OnStart (context);
			
			AIEntityContext ec = context as AIEntityContext;
			if (ec.Owner && ec.Target)
			{
				// save current machine ref
				Machine = ec.Owner.GetMachine();
				if (!Machine)
					throw new System.NullReferenceException();

				float fDistance = Vector3.Distance(ec.Owner.GetPosition(), ec.Target.GetPosition());
				if (fDistance > MaxDistance)
				{
					PuesueTarget(ec.Target.GetPosition(), MaxDistance);
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
			if (!ec.Owner || !ec.Target)
				return BehaviourStatus.FAILURE;
			
			// if find path stop
			IAIState curState = Machine.GetCurrentState ();
			if (curState.StateID != AITypeID.AI_PATH)
				return BehaviourStatus.SUCCESS;
			
			return BehaviourStatus.RUNNING;
		}
	}
}


