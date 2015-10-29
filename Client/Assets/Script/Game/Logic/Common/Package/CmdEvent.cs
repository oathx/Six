using UnityEngine;

/// <summary>
/// Net cmd.
/// </summary>
public class CmdEvent
{

	public const int CMD_ERROR				= -99999;
	public class ErrorEventArgs : IEventArgs
	{
		public int			Code
		{ get; set; }
	}

	/// <summary>
	/// Creates the error event.
	/// </summary>
	/// <returns>The error event.</returns>
	/// <param name="code">Code.</param>
	public static IEvent	CreateErrorEvent(int error)
	{
		ErrorEventArgs v = new ErrorEventArgs();
		v.Code = error;

		return new IEvent(
			EngineEventType.EVENT_GLOBAL, CMD_ERROR, v
			);
	}
	
	public class UIClickEventArgs : IEventArgs
	{
		public IUIWidget	Widget
		{ get; set; }
	}

	public const int CMD_UI_LOGIN 			= -30000;
	public class UILoginEventArgs : UIClickEventArgs
	{
		public string		UserName
		{ get; set; }

		public string		Password
		{ get; set; }
	}

	public const int CMD_UI_REGISTER		= -30001;

	public const int CMD_UI_JOIN			= -30002;
	public class UIJoinEventArgs : UIClickEventArgs
	{
		public int 			PlayerID
		{ get; set; }
	}

	// notify load scene
	public const int CMD_LOGIC_LOADSCENE	= -31001;
	public const int CMD_SCENE_LOADSTART	= -31002;
	public const int CMD_SCENE_LOADING		= -31003;
	public const int CMD_SCENE_LOADFINISH	= -31004;
	public class SceneLoadEventArgs : IEventArgs
	{
		public int 			SceneID
		{ get; set; }

		public float		Progress
		{ get; set; }
	}

	public const int EVT_LOGIC_OPENSYSTEM 	= -41000;
	public class OpenSystemEventArys : IEventArgs
	{
		public int 			ID
		{ get; set; }
	}
}

