using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AI
{
	/// <summary>
	/// AI speak.
	/// </summary>
	public class AIWaitForTime : AIBehaviour
	{
		/// <summary>
		/// Gets or sets the duration.
		/// </summary>
		/// <value>The duration.</value>
		public float					MinWaitTime;

		/// <summary>
		/// The wait time.
		/// </summary>
		public float					WaitTime;

		/// <summary>
		/// The max wait time.
		/// </summary>
		public float					MaxWaitTime;

		/// <summary>
		/// Gets the start time.
		/// </summary>
		/// <value>The start time.</value>
		public float					StartTime
		{ get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="IRandomSelector"/> class.
		/// </summary>
		public AIWaitForTime()
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

			// set start time
			StartTime = Time.time;

			if (MaxWaitTime != 0)
			{
				WaitTime = Random.Range(MinWaitTime, MaxWaitTime);
			}
		}
		
		/// <summary>
		/// Raises the update event.
		/// </summary>
		/// <param name="context">Context.</param>
		public override BehaviourStatus	OnUpdate(AIContext context)
		{
			float fElapsed = Time.time - StartTime;
			return fElapsed > WaitTime ? BehaviourStatus.SUCCESS : BehaviourStatus.RUNNING;
		}
	}
}


