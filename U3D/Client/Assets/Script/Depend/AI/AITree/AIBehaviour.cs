using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AI
{
	public enum BehaviourStatus
	{
		SUCCESS,
		RUNNING,
		FAILURE,
	}

	/// <summary>
	/// I context.
	/// </summary>
	public class AIContext : INullObject
	{
		public AIMemory		Memory
		{ get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="AI.AIContext"/> class.
		/// </summary>
		public AIContext()
		{
			Memory = new AIMemory ();
		}
	}

	/// <summary>
	/// AI memory.
	/// </summary>
	public class AIMemory
	{
		private Dictionary<string, object> memory = new Dictionary<string, object>();

		/// <summary>
		/// Initializes a new instance of the <see cref="AI.AIMemory"/> class.
		/// </summary>
		public AIMemory()
		{

		}

		/// <summary>
		/// Remember the specified name and val.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="val">Value.</param>
		public void Remember(string name, object val)
		{
			memory [name] = val;
		}

		/// <summary>
		/// Recall the specified name and val.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="val">Value.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public bool Recall<T>(string name, out T val)
		{
			object result = default(T);
			if (memory.TryGetValue(name, out result))
			{
				val = (T)result; 
				return true;
			}
			else
			{
				throw new System.NullReferenceException("Can't recall " + name);
			}

			val = (T)result;
			return false;
		}
	}

	/// <summary>
	/// AI behaviour.
	/// </summary>
	public class AIBehaviour : INullObject
	{
		/// <summary>
		/// Gets the children.
		/// </summary>
		/// <value>The children.</value>
		public AIBehaviour[]			Children
		{ get; protected set;}

		/// <summary>
		/// Gets the index.
		/// </summary>
		/// <value>The index.</value>
		public int 						Index
		{ get; protected set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="IBehaviour`1"/> is started.
		/// </summary>
		/// <value><c>true</c> if started; otherwise, <c>false</c>.</value>
		public bool						Started
		{ get; protected set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="IBehaviour"/> class.
		/// </summary>
		public AIBehaviour()
		{
			Started = false; Index = 0;
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="IBehaviour`1"/> class.
		/// </summary>
		/// <param name="aryBehaviour">Ary behaviour.</param>
		public AIBehaviour(params AIBehaviour[] aryBehaviour)
		{
			Started 	= false;
			Children 	= aryBehaviour;
			Index 		= 0;
		}
		
		/// <summary>
		/// Run the specified context.
		/// </summary>
		/// <param name="context">Context.</param>
		public virtual BehaviourStatus	Run(AIContext context)
		{
			if (!Started)
			{
				OnStart(context); Started = true;
			}

			BehaviourStatus status = OnUpdate (context);
			
			// if the behaviour execute fnishied then stop
			if (status != BehaviourStatus.RUNNING)
			{
				Stop(context);
			}
			
			return status;
		}
		
		/// <summary>
		/// Raises the start event.
		/// </summary>
		/// <param name="context">Context.</param>
		public virtual void 			OnStart(AIContext context)
		{
	#if OPEN_DEBUG_AILOG
			Debug.Log("Behaviour --------------------------> OnStart")
	#endif
			Index = 0;
		}
		
		/// <summary>
		/// Raises the update event.
		/// </summary>
		/// <param name="context">Context.</param>
		public virtual BehaviourStatus	OnUpdate(AIContext context)
		{
			return BehaviourStatus.SUCCESS;
		}
		
		/// <summary>
		/// Raises the exit event.
		/// </summary>
		/// <param name="context">Context.</param>
		public virtual void 			OnExit(AIContext context)
		{
			#if OPEN_DEBUG_AILOG
			Debug.Log("Behaviour --------------------------> OnStop")
			#endif
		}

		/*
		/// <summary>
		/// Start the specified context.
		/// </summary>
		/// <param name="context">Context.</param>
		public virtual void 			Start(AIContext context)
		{
			if (!Started)
			{
				OnStart(context); Started = true;
			}
		}
		*/

		/// <summary>
		/// Stop the specified context.
		/// </summary>
		/// <param name="context">Context.</param>
		public virtual void 			Stop(AIContext context)
		{
			if (Started)
			{
				OnExit(context); Started = false;
			}
		}

		/// <summary>
		/// Deserialize the specified behaviour.
		/// </summary>
		/// <param name="behaviour">Behaviour.</param>
		public virtual void 			Deserialize(Chunk chunk)
		{
			InstallChunkField (chunk);
			
			List<AIBehaviour> 
				aryBehaviour = new List<AIBehaviour> ();
			
			for(int idx=0; idx<chunk.AryChunk.Count; idx++)
			{
				AIBehaviour b = AIBehaviourFactory.GetSingleton().CreateBehaviour(
					chunk.AryChunk[idx].TypeName
					);
				
				// parse childe behaviour
				b.Deserialize(chunk.AryChunk[idx]);
				
				// add to child
				aryBehaviour.Add(b);
			}
			
			Children = aryBehaviour.ToArray ();
		}
		
		/// <summary>
		/// Sets the field.
		/// </summary>
		/// <param name="spoken">Spoken.</param>
		public virtual void 			InstallChunkField(Chunk chunk)
		{
			System.Reflection.FieldInfo[] aryField = GetType().GetFields();
			foreach (System.Reflection.FieldInfo field in aryField)
			{
				foreach (Exp exp in chunk.AryExp)
				{
					if (exp.Name.ToLower() == field.Name.ToLower())
					{
						if (field.FieldType == typeof(int)) {
							field.SetValue(this, int.Parse(exp.Value));
						} else if (field.FieldType == typeof(float)) {
							field.SetValue(this, float.Parse(exp.Value));
						} else if (field.FieldType == typeof(string)) {
							field.SetValue(this, exp.Value);
						} else if (field.FieldType == typeof(bool)) {
							field.SetValue(this, bool.Parse(exp.Value));
						}
					}
				}
			}
		}
	}
}