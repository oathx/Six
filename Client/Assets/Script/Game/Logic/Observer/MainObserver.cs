using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Login observer.
/// </summary>
public class MainObserver : IEventObserver
{
	public IJoystickPlugin	Joystick
	{ get; private set; }

	/// <summary>
	/// Gets the C.
	/// </summary>
	/// <value>The C.</value>
	public ICameraPlugin	MainCamrea
	{ get; private set; }

	/// <summary>
	/// Gets the player mgr.
	/// </summary>
	/// <value>The player mgr.</value>
	public PlayerManager	PlayerMgr
	{ get; private set; }

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		Joystick = GameEngine.GetSingleton().LoadPlugin<IJoystickPlugin>();
		if (!Joystick)
			throw new System.NullReferenceException();
		
		IJoystickShape joystickShape = Joystick.GetJoystickShape();
		if (!joystickShape)
		{
			Joystick.SetJoysticyShape(
				UISystem.GetSingleton().LoadWidget<IJoystickShape>(ResourceDef.UI_JOYSTICK), new Vector3(88, 88, 0)
				);
		}

		// load game main camera
		MainCamrea 	= GameEngine.GetSingleton().LoadPlugin<ICameraPlugin>();
		if (!MainCamrea)
			throw new System.NullReferenceException();

		PlayerMgr	= GameEngine.GetSingleton().LoadPlugin<PlayerManager>();
		if (!PlayerMgr)
			throw new System.NullReferenceException();
	}

	/// <summary>
	/// Active this instance.
	/// </summary>
	public override void 	Active()
	{
		// create main player
		InstallMainPlayer();

		// start game joystick
		if (Joystick)
		{
			Joystick.RegisterObserver<JoystickObserver>(
				typeof(JoystickObserver).Name, true
				);

			Joystick.Startup();
		}
	}

	/// <summary>
	/// Detive this instance.
	/// </summary>
	public override void 	Detive()
	{
		if (Joystick)
		{
			Joystick.UnregisterObserver(
				typeof(JoystickObserver).Name
				);

			Joystick.Shutdown();
		}
	}

	/// <summary>
	/// Installs the main player.
	/// </summary>
	protected void 			InstallMainPlayer()
	{
		int nCurSceneID 	= SceneSupport.GetSingleton().GetSceneID();

		// Query current scene config
		SqlScene sqlScene 	= GameSqlLite.GetSingleton().Query<SqlScene>(nCurSceneID);
		if (!sqlScene)
			throw new System.NullReferenceException();
	
		// Query main player job info
		SqlJob sqlJob = GameSqlLite.GetSingleton().Query<SqlJob>(GlobalUserInfo.Job);
		if (!sqlJob)
			throw new System.NullReferenceException();

		// create mian player entity
		PlayerEntity entity = PlayerMgr.CreateEntity(EntityType.ET_MAIN.ToString(), 
		                                             EntityType.ET_MAIN, 
		                                             0, 
		                                             string.Empty, 
		                                             sqlScene.Born, 
		                                             Vector3.one, 
		                                             Vector3.zero,
		                                             0, 
		                                             sqlJob.ShapeID) as PlayerEntity;

		SqlCamera sqlCamera = GameSqlLite.GetSingleton().Query<SqlCamera>(sqlScene.CameraID);
		if (sqlCamera)
		{
			CameraSetting cs = new CameraSetting();
			cs.ID 						= sqlCamera.ID;
			cs.FieldOfView				= sqlCamera.FieldOfView;
			cs.IsRelative				= sqlCamera.IsRelative;
			cs.OffsetEulerAngles		= sqlCamera.OffsetEulerAngles;
			cs.OffsetEulerAnglesLerp	= sqlCamera.OffsetEulerAnglesLerp;
			cs.OffsetPosition			= sqlCamera.OffsetPosition;
			cs.OffsetPositionLerp		= sqlCamera.OffsetPositionLerp;
			
			// config the camera param
			MainCamrea.Configurable(cs);

			// bind character
			MainCamrea.SetTarget(entity, entity.transform, entity.transform);
			MainCamrea.Startup();
		}

		PlayerMgr.SetPlayer(entity);
	}
}