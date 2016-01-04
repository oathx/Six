using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AI
{
	/// <summary>
	/// AI attack.
	/// </summary>
	public class AISearch : AIBehaviour
	{
		public float					Radius;
		public int 						Layer;

		/// <summary>
		/// Initializes a new instance of the <see cref="IRandomSelector"/> class.
		/// </summary>
		public AISearch()
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
			if (ec.Owner)
			{
				bool bSetLeader = true;

				if (!ec.Target)
				{
					IEntity[] aryTarget = ec.PlayerMgr.Select(ec.Owner.GetPosition(), Radius, Layer);
					if (aryTarget.Length > 0) 
					{
						ec.Target 	= aryTarget[0];
					}
					else
					{
						bSetLeader 	= false;
					}
				}

				if (bSetLeader)
				{
					ec.Leader = ec.PlayerMgr.GetPlayer();
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
			if (!ec.Owner && !ec.Target && !ec.Leader)
				return BehaviourStatus.FAILURE;
			
			return BehaviourStatus.SUCCESS;
		}
	}
}


