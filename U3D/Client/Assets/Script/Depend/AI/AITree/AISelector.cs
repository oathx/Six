using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AI
{
	/// <summary>
	/// I selector.
	/// </summary>
	public class AISelector : AIBehaviour
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ISequence"/> class.
		/// </summary>
		public AISelector()
			: base()
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ISelector`1"/> class.
		/// </summary>
		/// <param name="aryBehaviour">Ary behaviour.</param>
		public AISelector(params AIBehaviour[] aryBehaviour)
			: base(aryBehaviour)
		{
		}
		
		/// <summary>
		/// Raises the start event.
		/// </summary>
		/// <param name="context">Context.</param>
		public override void 			OnStart(AIContext context)
		{
			base.OnStart (context);
		}
		
		/// <summary>
		/// Raises the update event.
		/// </summary>
		/// <param name="context">Context.</param>
		public override BehaviourStatus	OnUpdate(AIContext context)
		{
			while(Index < Children.Length)
			{
				BehaviourStatus status = Children[Index].Run(context);
				if (status == BehaviourStatus.RUNNING || status == BehaviourStatus.FAILURE)
					return status;
				
				Index ++;
			}
			
			return BehaviourStatus.SUCCESS;
		}
		
		/// <summary>
		/// Raises the exit event.
		/// </summary>
		/// <param name="context">Context.</param>
		public override void 			OnExit(AIContext context)
		{
			if (Index < Children.Length)
			{
				Children[Index].Stop(context);
			}
			
			base.OnExit (context);
		}
	}
}