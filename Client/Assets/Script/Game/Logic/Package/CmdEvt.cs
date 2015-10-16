using UnityEngine;

/// <summary>
/// Net cmd.
/// </summary>
public class CmdEvt
{
	public const int 	 CMD_LOGIC_STATUS = -20000;
	public class LogicStatusEventArgs : IEventArgs
	{
		public int 		SceneID
		{ get; set; }
	}
}

