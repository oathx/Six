using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace AI
{
	/// <summary>
	/// AI entity context.
	/// </summary>
	public class AIEntityContext : AIContext
	{
		/// <summary>
		/// Gets the player manager.
		/// </summary>
		/// <value>The player manager.</value>
		public PlayerManager	PlayerMgr
		{ get; private set; }

		/// <summary>
		/// Gets the monster manager.
		/// </summary>
		/// <value>The monster manager.</value>
		public MonsterManager	MonsterMgr
		{ get; private set; }
		
		/// <summary>
		/// Gets the owner.
		/// </summary>
		/// <value>The owner.</value>
		public BaseUnitEntity	Owner
		{ get; private set; }
		
		/// <summary>
		/// Gets or sets the target.
		/// </summary>
		/// <value>The target.</value>
		public IEntity			Target
		{ get; set; }

		/// <summary>
		/// Gets or sets the leader.
		/// </summary>
		/// <value>The leader.</value>
		public IEntity			Leader
		{ get; set; }

		/// <summary>
		/// Gets the session.
		/// </summary>
		/// <value>The session.</value>
		public LogicPlugin		Session
		{ get; private set; }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="AI.AIEntityContext"/> class.
		/// </summary>
		public AIEntityContext (BaseUnitEntity owner)
		{
			PlayerMgr 	= GameEngine.GetSingleton ().QueryPlugin<PlayerManager> ();
			MonsterMgr 	= GameEngine.GetSingleton ().QueryPlugin<MonsterManager> ();
			Session 	= GameEngine.GetSingleton ().QueryPlugin<LogicPlugin> ();
			
			// bind behaviour owner
			Owner 			= owner;
		}
	}
}

