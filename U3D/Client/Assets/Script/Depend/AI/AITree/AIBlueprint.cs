using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AI
{
	// the expression
	public class Exp : INullObject
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string	Name
		{ get; set; }
		
		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>The value.</value>
		public string	Value
		{ get; set; }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="IEquate"/> class.
		/// </summary>
		/// <param name="text">Text.</param>
		public Exp(string text)
		{
			string[] aryText = text.Split('=');
			if (aryText.Length == 2)
			{
				Name = aryText[0]; Value = aryText[1];
			}
		}
	}

	/// <summary>
	/// Chunk.
	/// </summary>
	public class Chunk : INullObject
	{
		/// <summary>
		/// The ary equate.
		/// </summary>
		public List<Exp>		AryExp = new List<Exp>();
		
		/// <summary>
		/// Gets or sets the name of the type.
		/// </summary>
		/// <value>The name of the type.</value>
		public string			TypeName
		{ get; set; }
		
		/// <summary>
		/// The ary spoken.
		/// </summary>
		public List<Chunk>		AryChunk = new List<Chunk>();
	}

	/// <summary>
	/// AI blueprint.
	/// </summary>
	public class AIBlueprint : INullObject
	{
		public Chunk			Root
		{ get; private set; }
		
		/// <summary>
		/// Initializes a new instance of the <see cref="IBlueprint"/> class.
		/// </summary>
		/// <param name="text">Text.</param>
		public AIBlueprint(string text)
		{
			string[] aryLine = text.Split('\n');
			if (aryLine.Length > 0)
			{
				Stack<Chunk> stack = new Stack<Chunk>();
				
				Chunk prev = default(Chunk);
				
				foreach(string line in aryLine)
				{
					string l = line.Replace("\t", string.Empty).Replace("\r", string.Empty);
					if (!string.IsNullOrEmpty(l) && l.IndexOf("//") < 0 && l[0] != ';')
					{
						Chunk chunk = new Chunk();
						
						string[] aryToken = l.Split(' ');
						if (aryToken.Length > 0)
							chunk.TypeName = aryToken[0];
						
						int nDepth = GetTreeDepth(line);
						if (nDepth > stack.Count)
							stack.Push(prev);
						
						while (nDepth < stack.Count)
							stack.Pop();
						
						if (stack.Count == 0)
						{
							Root = chunk;
						}
						else
						{
							Chunk parent = stack.Peek();
							parent.AryChunk.Add(chunk);
						}
						
						// read the spkoen equate
						for(int idx=1; idx<aryToken.Length; idx++)
						{
							chunk.AryExp.Add(new Exp(aryToken[idx]));
						}
						
						prev = chunk;
					}
				}
			}
		}
		
		/// <summary>
		/// Gets the tree depth.
		/// </summary>
		/// <returns>The tree depth.</returns>
		/// <param name="cmd">Cmd.</param>
		private int 		GetTreeDepth(string cmd)
		{
			int nLevel = 0;
			
			for (int i=0; i<cmd.Length; ++i)
			{
				if (!char.IsWhiteSpace(cmd[i]))
					break;
				
				nLevel ++;
			}
			
			return nLevel;
		}
		
		/// <summary>
		/// Creates the instance.
		/// </summary>
		/// <returns>The instance.</returns>
		public AIBehaviour	CreateInstance()
		{
			AIBehaviour behaviour = AIBehaviourFactory.GetSingleton().CreateBehaviour (Root.TypeName);
			if (!behaviour)
				throw new System.NullReferenceException ();

			behaviour.Deserialize (Root);
			
			return behaviour;
		}
	}
}