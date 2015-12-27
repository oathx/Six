using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Login observer.
/// </summary>
public class MainObserver : IEventObserver
{
	protected IJoystickPlugin	m_Joystick;
	
	/// <summary>
	/// Gets the player mgr.
	/// </summary>
	/// <value>The player mgr.</value>
	protected PlayerManager		m_PlayerManager;

	/// <summary>
	/// Gets the C.
	/// </summary>
	/// <value>The C.</value>
	protected ICameraPlugin		m_MainCamera;

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake()
	{
		m_Joystick = GameEngine.GetSingleton().LoadPlugin<IJoystickPlugin>();
		if (!m_Joystick)
			throw new System.NullReferenceException();
		
		IJoystickShape joystickShape = m_Joystick.GetJoystickShape();
		if (!joystickShape)
		{
			m_Joystick.SetJoysticyShape(
				UISystem.GetSingleton().LoadWidget<IJoystickShape>(ResourceDef.UI_JOYSTICK), new Vector3(88, 88, 0)
				);
		}

		// load game main camera
		m_MainCamera 	= GameEngine.GetSingleton().LoadPlugin<ICameraPlugin>();
		if (!m_MainCamera)
			throw new System.NullReferenceException();

		m_PlayerManager	= GameEngine.GetSingleton().LoadPlugin<PlayerManager>();
		if (!m_PlayerManager)
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
		if (m_Joystick)
		{
			m_Joystick.RegisterObserver<JoystickObserver>(
				typeof(JoystickObserver).Name, true
				);

			m_Joystick.Startup();
		}

	}

	/// <summary>
	/// Detive this instance.
	/// </summary>
	public override void 	Detive()
	{
		if (m_Joystick)
		{
			m_Joystick.UnregisterObserver(
				typeof(JoystickObserver).Name
				);

			m_Joystick.Shutdown();
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

		// create mian player entity
		PlayerEntity entity = m_PlayerManager.CreateEntity(EntityType.ET_MAIN.ToString(), 
		                                             EntityType.ET_MAIN, 
		                                             GlobalUserInfo.PlayerID, 
		                                             string.Empty, 
		                                             sqlScene.Born, 
		                                             Vector3.one, 
		                                             Vector3.zero,
		                                             0, 
		                                             GlobalUserInfo.Job) as PlayerEntity;
		// reset main palyer born point
		entity.SetPosition(sqlScene.Born);

		// config character camera
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
			m_MainCamera.Configurable(cs);

			// bind character
			m_MainCamera.SetTarget(entity, entity.transform, entity.transform);
			m_MainCamera.Startup();
		}

		m_PlayerManager.SetPlayer(entity);
	}
}