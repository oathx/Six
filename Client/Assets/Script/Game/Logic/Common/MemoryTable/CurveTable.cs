using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

/// <summary>
/// Curve type.
/// </summary>
public enum CurveType
{
	MOTION_JOYSTICKDEPTH_MOVESPEED = 0,
	MOTION_JOYSTICKDEPTH_MOVEACCELERATE,
	MOTION_MOVEANGLE_ROTATESPEED,
	MOTION_JOYSTICKDEPTH_ROTATESPEED,
	MOTION_MOVEANGLE_ROTATEACCELERATE,
	MOTION_JOYSTICKDEPTH_ROTATEACCELERATE,
	MOTION_ANIMATIONTIME_DODGEFACTOR,
	MOTION_ANIMATIONTIME_CHARGEFACTOR,
	MOTION_ANIMATIONTIME_HITFACTOR,
	
	ANIMATION_JOYSTICKDEPTH_DAMPTIME,
	ANIMATION_MOVEANGLE_DAMPTIME,
	ANIMATION_MOVESPEED_VELOCITYPARAM,
	ANIMATION_MOVEANGLE_ANGLEPARAM,
	
	CAMERA_MOVEANGLE_LOOKAROUNDSPEED,
	CAMERA_SHAKETIME_YAXISMOVE,
}

/// <summary>
/// Curve meta info.
/// </summary>
public sealed class CurveItem
{
	/// <summary>
	/// Curve id.
	/// </summary>
	public int 				ID 
	{ get; private set; }
	
	/// <summary>
	/// Curve name.
	/// </summary>
	public string			Name 
	{ get; private set; }
	
	/// <summary>
	/// Curve.
	/// </summary>
	public AnimationCurve 	Curve 
	{ get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="CurveMeta"/> class.
	/// </summary>
	public CurveItem()
	{
		Curve = new AnimationCurve ();
	}

	/// <summary>
	/// Get the curve time.
	/// </summary>
	public float 			CurveTime
	{
		get{
			if (Curve == null || Curve.length == 0)
				return 0;

			return Curve.keys[Curve.length - 1].time;
		}
	}
	
	/// <summary>
	/// Parse xml element.
	/// </summary>
	/// <param name="element"></param>
	public void 			Parse(XmlElement element)
	{
		ID 					= XmlParser.GetIntValue(element, "id");
		Name 				= XmlParser.GetStringValue(element, "name");

		Curve.preWrapMode 	= (WrapMode)XmlParser.GetIntValue(element, "preWrapMode");
		Curve.postWrapMode 	= (WrapMode)XmlParser.GetIntValue(element, "postWrapMode");
		
		foreach (XmlElement child in element.ChildNodes)
		{
			Keyframe keyframe = new Keyframe(XmlParser.GetFloatValue(child, "time"),
			                                 XmlParser.GetFloatValue(child, "value"),
			                                 XmlParser.GetFloatValue(child, "inTangent"),
			                                 XmlParser.GetFloatValue(child, "outTangent"));
			Curve.AddKey(keyframe);
		}
	}
}

/// <summary>
/// Character table.
/// </summary>
public class CurveTable : MemoryTable<CurveTable, int, CurveItem>
{
	public CurveItem motion_JoystickDepth_MoveSpeed 
	{ get; private set; }
	public CurveItem motion_JoystickDepth_MoveAccelerate 
	{ get; private set; }
	public CurveItem motion_MoveAngle_RotateSpeed
	{ get; private set; }
	public CurveItem motion_JoystickDepth_RotateSpeed 
	{ get; private set; }
	public CurveItem motion_MoveAngle_RotateAccelerate
	{ get; private set; }
	public CurveItem motion_JoystickDepth_RotateAccelerate
	{ get; private set; }
	public CurveItem motion_AnimationTime_DodgeFactor 
	{ get; private set; }
	public CurveItem motion_AnimationTime_ChargeFactor 
	{ get; private set; }
	public CurveItem motion_AnimationTime_HitFactor 
	{ get; private set; }
	public CurveItem animation_JoystickDepth_DampTime 
	{ get; private set; }
	public CurveItem animation_MoveAngle_DampTime
	{ get; private set; }
	public CurveItem animation_MoveSpeed_VelocityParam
	{ get; private set; }
	public CurveItem animation_MoveAngle_AngleParam
	{ get; private set; }
	public CurveItem camera_MoveAngle_LookAroundSpeed
	{ get; private set; }
	public CurveItem camera_ShakeTime_YAxisMove 
	{ get; private set; }

	/// <summary>
	/// Raises the enable event.
	/// </summary>
	void OnEnable()
	{
		TextAsset asset = Resources.Load ("Default/Config/Curve", typeof(TextAsset)) as TextAsset;
		if (asset)
		{
			XmlParser parser = new XmlParser(asset.text);
			foreach (XmlElement e in parser.root.GetElementsByTagName("curve"))
			{
				CurveItem cm = new CurveItem();
				cm.Parse(e);

				Add(cm.ID, cm);
			}

			// init default config
			InitConfig();

			Resources.UnloadAsset(asset);
		}
	}

	/// <summary>
	/// Inits the table.
	/// </summary>
	void InitConfig()
	{
		motion_JoystickDepth_MoveSpeed 			= Query((int)CurveType.MOTION_JOYSTICKDEPTH_MOVESPEED);
		motion_JoystickDepth_MoveAccelerate 	= Query((int)CurveType.MOTION_JOYSTICKDEPTH_MOVEACCELERATE);
		motion_MoveAngle_RotateSpeed 			= Query((int)CurveType.MOTION_MOVEANGLE_ROTATESPEED);
		motion_JoystickDepth_RotateSpeed 		= Query((int)CurveType.MOTION_JOYSTICKDEPTH_ROTATESPEED);
		motion_MoveAngle_RotateAccelerate 		= Query((int)CurveType.MOTION_MOVEANGLE_ROTATEACCELERATE);
		motion_JoystickDepth_RotateAccelerate 	= Query((int)CurveType.MOTION_JOYSTICKDEPTH_ROTATEACCELERATE);
		motion_AnimationTime_DodgeFactor 		= Query((int)CurveType.MOTION_ANIMATIONTIME_DODGEFACTOR);
		motion_AnimationTime_ChargeFactor 		= Query((int)CurveType.MOTION_ANIMATIONTIME_CHARGEFACTOR);
		motion_AnimationTime_HitFactor 			= Query((int)CurveType.MOTION_ANIMATIONTIME_HITFACTOR);
		
		animation_JoystickDepth_DampTime 		= Query((int)CurveType.ANIMATION_JOYSTICKDEPTH_DAMPTIME);
		animation_MoveAngle_DampTime 			= Query((int)CurveType.ANIMATION_MOVEANGLE_DAMPTIME);
		animation_MoveSpeed_VelocityParam 		= Query((int)CurveType.ANIMATION_MOVESPEED_VELOCITYPARAM);
		animation_MoveAngle_AngleParam 			= Query((int)CurveType.ANIMATION_MOVEANGLE_ANGLEPARAM);
		
		camera_MoveAngle_LookAroundSpeed 		= Query((int)CurveType.CAMERA_MOVEANGLE_LOOKAROUNDSPEED);
		camera_ShakeTime_YAxisMove 				= Query((int)CurveType.CAMERA_SHAKETIME_YAXISMOVE);
	}

	/// <summary>
	/// Move curve delegate.
	/// </summary>
	/// <param name="joystickDepth">Joystick depth.</param>
	/// <param name="moveSpeed">Move speed.</param>
	public void 	OnMoveCurve(float fJoystickDepth, ref float fMoveSpeed)
	{
		float fTargetSpeed 	= motion_JoystickDepth_MoveSpeed.Curve.Evaluate(fJoystickDepth) * fMoveSpeed;
		float fAccelerate 	= motion_JoystickDepth_MoveAccelerate.Curve.Evaluate(fJoystickDepth) * Time.deltaTime;
		
		if (fTargetSpeed > fMoveSpeed)
		{
			fMoveSpeed += fAccelerate;
			if (fMoveSpeed > fTargetSpeed)
			{
				fMoveSpeed = fTargetSpeed;
			}
		}
		else if (fTargetSpeed < fMoveSpeed)
		{
			fMoveSpeed -= fAccelerate;
			if (fMoveSpeed < fTargetSpeed)
			{
				fMoveSpeed = fTargetSpeed;
			}
		}
	}
	
	/// <summary>
	/// Rotate curve handle.
	/// </summary>
	/// <param name="moveAngle">Move angle.</param>
	/// <param name="joystickDepth">Joystick depth.</param>
	/// <param name="rotateSpeed">Rotate speed.</param>
	public void 	OnRotateCurve(float fMoveAngle, float fJoystickDepth, ref float fRotateSpeed)
	{
		float fTargetSpeed 	= motion_MoveAngle_RotateSpeed.Curve.Evaluate(Mathf.Abs(fMoveAngle));
		fTargetSpeed 		*= motion_JoystickDepth_RotateSpeed.Curve.Evaluate(fJoystickDepth);
		
		float fAccelerate 	= motion_MoveAngle_RotateAccelerate.Curve.Evaluate(Mathf.Abs(fMoveAngle));
		fAccelerate 		*= motion_JoystickDepth_RotateAccelerate.Curve.Evaluate(fJoystickDepth);
		fAccelerate 		*= Time.deltaTime;
		
		if (fTargetSpeed > fRotateSpeed)
		{
			fRotateSpeed += fAccelerate;
			if (fRotateSpeed > fTargetSpeed)
			{
				fRotateSpeed = fTargetSpeed;
			}
		}
		else if (fTargetSpeed < fRotateSpeed)
		{
			fRotateSpeed -= fAccelerate;
			if (fRotateSpeed < fTargetSpeed)
			{
				fRotateSpeed = fTargetSpeed;
			}
		}
	}
	
	/// <summary>
	/// Dodge curve handle.
	/// </summary>
	/// <param name="animationTime">Animation time.</param>
	/// <param name="moveSpeedFactor">Move speed factor.</param>
	public void 	OnDodgeCurve(float fAnimationTime, ref float fMoveSpeedFactor)
	{
		fMoveSpeedFactor = motion_AnimationTime_DodgeFactor.Curve.Evaluate(fAnimationTime);
	}
	
	/// <summary>
	/// Charge curve handle.
	/// </summary>
	/// <param name="animationTime">Animation time.</param>
	/// <param name="moveSpeedFactor">Move speed factor.</param>
	public void 	OnChargeCurve(float fAnimationTime, ref float fMoveSpeedFactor)
	{
		fMoveSpeedFactor = motion_AnimationTime_ChargeFactor.Curve.Evaluate(fAnimationTime);
	}
	
	/// <summary>
	/// Hit curve handle.
	/// </summary>
	/// <param name="animationTime">Animation time.</param>
	/// <param name="moveSpeedFactor">Move speed factor.</param>
	public void 	OnHitCurve(float fAnimationTime, ref float fMoveSpeedFactor)
	{
		fMoveSpeedFactor = motion_AnimationTime_HitFactor.Curve.Evaluate(fAnimationTime);
	}
	
	/// <summary>
	/// Animation curve handle.
	/// </summary>
	/// <param name="normalizedTime">Normalized time.</param>
	/// <param name="joystickDepth">Joystick depth.</param>
	/// <param name="dampTime">Damp time.</param>
	/// <param name="speedParam">Speed param.</param>
	/// <param name="angleParam">Angle param.</param>
	public void 	OnAnimationCurve(float fNormalizedTime, float fJoystickDepth, ref float fDampTime, ref float fSpeedParam, ref float fAngleParam)
	{
		fDampTime 	= animation_JoystickDepth_DampTime.Curve.Evaluate(fJoystickDepth);
		fDampTime 	*= animation_MoveAngle_DampTime.Curve.Evaluate(Mathf.Abs(fAngleParam));
		
		fSpeedParam = animation_MoveSpeed_VelocityParam.Curve.Evaluate(fSpeedParam);
		fAngleParam = fAngleParam * animation_MoveAngle_AngleParam.Curve.Evaluate(Mathf.Abs(fAngleParam));
	}
	
	/// <summary>
	/// Camera look around curve handle.
	/// </summary>
	/// <param name="moveAngle">Move angle.</param>
	/// <param name="lookAroundSpeed">Look around speed.</param>
	public void 	OnCameraLookAroundCurve(float fMoveAngle, ref float fLookAroundSpeed)
	{
		fLookAroundSpeed = camera_MoveAngle_LookAroundSpeed.Curve.Evaluate(Mathf.Abs(fMoveAngle));
	}
	
	/// <summary>
	/// Camera shake curve handle.
	/// </summary>
	/// <param name="timePercent">Shake time percent. [0 - 1]</param>
	/// <param name="y">Y axis move.</param>
	public void 	OnCameraShakeCurve(float fTimePercent, ref float y)
	{
		float time = Mathf.Clamp01(fTimePercent) * camera_ShakeTime_YAxisMove.CurveTime;
		y = camera_ShakeTime_YAxisMove.Curve.Evaluate(time);
	}
}
