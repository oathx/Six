
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AI
{
	/// <summary>
	/// I random selector.
	/// </summary>
	public class AIRandom : AIBehaviour
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="IRandomSelector"/> class.
		/// </summary>
		public AIRandom()
			: base()
		{
			
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="ISelector`1"/> class.
		/// </summary>
		/// <param name="aryBehaviour">Ary behaviour.</param>
		public AIRandom(params AIBehaviour[] aryBehaviour)
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
			
			// move to first child
			Index = Random.Range(0, Children.Length);
		}
		
		/// <summary>
		/// Raises the update event.
		/// </summary>
		/// <param name="context">Context.</param>
		public override BehaviourStatus	OnUpdate(AIContext context)
		{
			return Children [Index].Run (context);
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