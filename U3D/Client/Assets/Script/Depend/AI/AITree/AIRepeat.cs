using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AI
{
	/// <summary>
	/// AI speak.
	/// </summary>
	public class AIRepeat : AIBehaviour
	{
		/// <summary>
		/// Gets or sets the duration.
		/// </summary>
		/// <value>The duration.</value>
		public int						Count;
		
		/// <summary>
		/// Gets the start time.
		/// </summary>
		/// <value>The start time.</value>
		public int						Repeat
		{ get; private set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="AI.AIRepeat"/> is forever.
		/// </summary>
		/// <value><c>true</c> if forever; otherwise, <c>false</c>.</value>
		public bool						Forever
		{ get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="IRandomSelector"/> class.
		/// </summary>
		public AIRepeat()
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

			// reset current repeat count
			Repeat 	= 0;
		}

		/// <summary>
		/// Raises the update event.
		/// </summary>
		/// <param name="context">Context.</param>
		public override BehaviourStatus	OnUpdate(AIContext context)
		{
			if (Children.Length <= 0)
				return BehaviourStatus.FAILURE;

			do {
				while (Index < Children.Length) {
					BehaviourStatus status = Children [Index].Run (context);
					if (status != BehaviourStatus.RUNNING)
						Index ++;
				}

				Index = 0;
				Repeat ++;

			} while( Count == 0 ? true : Repeat < Count);

			return BehaviourStatus.SUCCESS;
		}

		/// <summary>
		/// Raises the start event.
		/// </summary>
		/// <param name="context">Context.</param>
		public override void 			OnExit(AIContext context)
		{
			base.OnExit (context);

			// reset loop value
			Repeat 	= 0;
			Index 	= 0;
		}
	}
}



