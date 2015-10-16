using UnityEngine;

/// <summary>
/// Net cmd.
/// </summary>
public class CmdEvt
{
	public const int CMD_ERROR			= -99999;
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

	public const int CMD_UI_LOGIN 		= -30000;
	public class UILoginEventArgs : UIClickEventArgs
	{
		public string		UserName
		{ get; set; }

		public string		Password
		{ get; set; }
	}

	public const int CMD_UI_REGISTER	= -30001;
}

