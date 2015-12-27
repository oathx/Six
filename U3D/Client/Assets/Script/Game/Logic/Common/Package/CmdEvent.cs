using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

	public const int CMD_DEBUG				= -99998;
	public class DebugCmdEventArgs : IEventArgs
	{
		public string		CmdText
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

	public const int CMD_UI_REGBUTTON		= -30001;

	public const int CMD_UI_JOIN			= -30002;
	public class UIJoinEventArgs : UIClickEventArgs
	{
		public int 			PlayerID
		{ get; set; }
	}

	public const int CMD_UI_DIALOG			= -30003;
	public class UIDialogTextEventArgs : UIClickEventArgs
	{
		public string		Text
		{ get; set; }

		public int			Index
		{ get; set; }

		public int 			Count
		{ get; set; }
	}

	public const int CMD_UI_REGISTER		= -30005;
	public class UIRegisterEventArgs : UIClickEventArgs
	{
		public string		UserName
		{ get; set; }
		
		public string		Password
		{ get; set; }
	}

	public const int CMD_UI_REGRETURN		= -30006;

	public const int CMD_UI_SELECTJOB		= -30007;
	public class UISelectJobEventArgs : UIClickEventArgs
	{
		public int 			Index
		{ get; set; }
	}

	
	public const int CMD_UI_CREATEROLE 		= -30008;
	public class UICreateRoleEventArgs : UIClickEventArgs
	{
		public string		Name
		{ get; set; }

		public int 			ID
		{ get; set; }
	}

	public const int CMD_UI_CREATERAND 		= -30009;

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

	public const int CMD_SCENE_TRIGGER		= -31005;
	public class SceneTriggerEventArgs : IEventArgs
	{
		/// <summary>
		/// Gets or sets the I.
		/// </summary>
		/// <value>The I.</value>
		public int 			ID
		{ get; set; }

		/// <summary>
		/// Gets or sets the handle.
		/// </summary>
		/// <value>The handle.</value>
		public object		Handle
		{ get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="CmdEvent+SceneTriggerEventArgs"/> is enter.
		/// </summary>
		/// <value><c>true</c> if enter; otherwise, <c>false</c>.</value>
		public bool			Enter
		{ get; set; }

		/// <summary>
		/// Gets or sets the event I.
		/// </summary>
		/// <value>The event I.</value>
		public int 			EventID
		{ get; set; }

		/// <summary>
		/// Gets or sets the target.
		/// </summary>
		/// <value>The target.</value>
		public Collider 	Who
		{ get; set; }
	}

	public const int CMD_LOGIC_AIPATH	= -32000;
	public class AIPathEventArgs : IEventArgs
	{
		/// <summary>
		/// The path.
		/// </summary>
		public List<Vector3>
			Path = new List<Vector3>();
		
		/// <summary>
		/// Gets or sets the distance.
		/// </summary>
		/// <value>The distance.</value>
		public float		MinDistance
		{ get; set; }
	}

	public const int CMD_LOGIC_AIFINDPATH	= -32001;
	public class AIFindPathEventArgs : IEventArgs
	{
		public Vector3		Target
		{ get; set; }

		/// <summary>
		/// Gets or sets the distance.
		/// </summary>
		/// <value>The distance.</value>
		public float		MinDistance
		{ get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="CmdEvent+AIFindPathEventArgs"/> draw line.
		/// </summary>
		/// <value><c>true</c> if draw line; otherwise, <c>false</c>.</value>
		public bool			DrawLine
		{ get; set; }

		/// <summary>
		/// Gets or sets the width of the line.
		/// </summary>
		/// <value>The width of the line.</value>
		public float		LineWidth
		{ get; set; }
	}

	public const int CMD_LOGIC_CHARGE 	= -32002;
	public class AIChargeEventArgs : IEventArgs
	{
		public SqlSkill		Skill
		{ get; set; }
		
		public IEntity		Target
		{ get; set; }
	}

	public const int CMD_LOGIC_OPENSYSTEM 	= -41000;
	public class OpenSystemEventArys : IEventArgs
	{
		public int 			ID
		{ get; set; }
	}

	public const int CMD_UI_ATTACK		= -51000;
	public class UIAttackEventArgs : UIClickEventArgs
	{
		public AttackType	Type;
		public int 			MagicID;
	}

	public const int CMD_LOGIC_ATTACK	= -52000;
	public class AttackEventArgs : IEventArgs
	{
		/// <summary>
		/// Gets or sets the skill ID.
		/// </summary>
		/// <value>The skill I.</value>
		public int 			MagicID
		{ get; set; }
		
		/// <summary>
		/// Gets or sets the source ID.
		/// </summary>
		/// <value>The source I.</value>
		public int 			SourceID
		{ get; set; }
		
		/// <summary>
		/// Gets or sets the target ID.
		/// </summary>
		/// <value>The target I.</value>
		public int 			TargetID
		{ get; set; }
		
		/// <summary>
		/// Gets or sets the state I.
		/// </summary>
		/// <value>The state I.</value>
		public int 			SkillID
		{ get; set; }
		
		/// <summary>
		/// Gets or sets a value indicating whether this instance is player.
		/// </summary>
		/// <value><c>true</c> if this instance is player; otherwise, <c>false</c>.</value>
		public bool			IsTargetPlayer
		{ get; set; }
		
		/// <summary>
		/// Gets or sets a value indicating whether this instance is attacker player.
		/// </summary>
		/// <value><c>true</c> if this instance is attacker player; otherwise, <c>false</c>.</value>
		public bool			IsAttackerPlayer
		{ get; set; }
	}

	public const int CMD_LOGIC_TRIGGER	= -52001;
	public class TriggerEventArgs : IEventArgs
	{
		public IEntity		Source
		{ get; set; }
		
		public IEntity		Target
		{ get; set; }
		
		public float		TriggerTime
		{ get; set; }
		
		public SqlSkill		sqlSkill
		{ get; set; }
	}

	public const int CMD_LOGIC_HIT 		= -52002;
	public class AIHitEventArgs : IEventArgs
	{
		public IEntity		Source
		{ get; set; }
		
		public IEntity		Target
		{ get; set; }
		
		public SqlSkill		sqlSkill
		{ get; set; }
	}
}

