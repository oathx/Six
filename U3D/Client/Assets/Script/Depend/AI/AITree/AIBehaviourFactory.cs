using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace AI
{
	/// <summary>
	/// AI behaviour factory.
	/// </summary>
	public class AIBehaviourFactory : SimpleSingleton<AIBehaviourFactory>
	{
		/// <summary>
		/// Creates the behaviour.
		/// </summary>
		/// <returns>The behaviour.</returns>
		/// <param name="typeName">Type name.</param>
		public AIBehaviour 		CreateBehaviour(string typeName)
		{
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (var type in GetNodeTypes(assembly))
				{
					if (type.Name.ToLower() == typeName.ToLower())
						return (AIBehaviour)Activator.CreateInstance(type);
				}
			}
			
			throw new Exception(
				string.Format("{0} is an unknown node type", typeName)
				);
		}
		
		/// <summary>
		/// Gets the node types.
		/// </summary>
		/// <returns>The node types.</returns>
		/// <param name="assembly">Assembly.</param>
		private IEnumerable<Type> GetNodeTypes(Assembly assembly)
		{
			foreach (System.Type type in assembly.GetTypes())
			{
				if (type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(AIBehaviour)))
				{
					yield return type;
				}
			}
		}
	}
}