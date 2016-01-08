using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AI
{
	/// <summary>
	/// AI attack.
	/// </summary>
	public class AIAttack : AIBehaviour
	{
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
		public AIAttack()
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
			if (ec.Owner && ec.Target)
			{
				// get owner character ctrl machine
				Machine = ec.Owner.GetMachine();
				if (!Machine)
					throw new System.NullReferenceException();

				if (!Machine.IsCurrentState(AITypeID.AI_BATTLE))
					Machine.ChangeState(AITypeID.AI_BATTLE);
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



			return BehaviourStatus.SUCCESS;
		}
	}
}

