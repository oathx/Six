using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AI
{
	/// <summary>
	/// AI behaviour tree.
	/// </summary>
	public class AIBehaviourTree : INullObject
	{
		/// <summary>
		/// Gets the root.
		/// </summary>
		/// <value>The root.</value>
		public AIBehaviour		Root
		{ get; private set; }
		
		/// <summary>
		/// Gets the context.
		/// </summary>
		/// <value>The context.</value>
		public AIContext		Context
		{ get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="AIBehaviourTree"/> class.
		/// </summary>
		public AIBehaviourTree (AIContext context, string scriptText)
		{
			AIBlueprint blueprint = new AIBlueprint (scriptText);
			if (blueprint)
				Root = blueprint.CreateInstance ();

			Context = context;
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		public virtual void 	Update()
		{
			Root.Run (Context);
		}
	}

	/// <summary>
	/// AI behaviour tree manager.
	/// </summary>
	public class AIBehaviourTreeManager : SimpleSingleton<AIBehaviourTreeManager>
	{
		/// <summary>
		/// The d tree.
		/// </summary>
		protected Dictionary<int, AIBehaviourTree> 
			dTree = new Dictionary<int, AIBehaviourTree> ();

		/// <summary>
		/// The d destroy.
		/// </summary>
		protected Queue<int> dQueue = new Queue<int>();

		/// <summary>
		/// Creates the AI behaviour tree.
		/// </summary>
		/// <returns>The AI behaviour tree.</returns>
		/// <param name="context">Context.</param>
		/// <param name="scriptText">Script text.</param>
		public virtual AIBehaviourTree	CreateAIBehaviourTree(int nID, AIContext context, string scriptText)
		{
			if (!dTree.ContainsKey(nID))
			{
				dTree.Add(nID, new AIBehaviourTree(context, scriptText));
			}

			return dTree[nID];
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		public virtual void 			Update()
		{
			foreach(KeyValuePair<int, AIBehaviourTree> it in dTree)
				it.Value.Update();

			while(dQueue.Count > 0)
			{
				int nID = dQueue.Dequeue();
				dTree.Remove(nID);
			}
		}

		/// <summary>
		/// Clearup this instance.
		/// </summary>
		public virtual void 			Clearup()
		{
			dTree.Clear ();
		}

		/// <summary>
		/// Destroies the AI behaviour tree.
		/// </summary>
		/// <param name="nID">N I.</param>
		public virtual void 			DestroyAIBehaviourTree(int nID)
		{
			if (dTree.ContainsKey(nID))
			{
				dQueue.Enqueue(nID);
			}
		}
	}
}


