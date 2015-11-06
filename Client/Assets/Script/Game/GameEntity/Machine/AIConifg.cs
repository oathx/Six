using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// AI type ID.
/// </summary>
public class AITypeID
{
	public const int AI_IDLE 			= 1 << 1;
	public const int AI_MOVE 			= 1 << 2;
	public const int AI_GLOBAL 			= 1 << 3;
	public const int AI_JUMP 			= 1 << 4;
	public const int AI_FLY 			= 1 << 5;
	public const int AI_DIE 			= 1 << 6;
	public const int AI_INSPECT 		= 1 << 7;
	public const int AI_ATTACK 			= 1 << 8;
	public const int AI_ALERT 			= 1 << 9;
	public const int AI_BATTLE 			= 1 << 10;
	public const int AI_HIT 			= 1 << 11;
	public const int AI_PATH 			= 1 << 12;
	public const int AI_BORN 			= 1 << 13;
	public const int AI_CHARGE 			= 1 << 14;
}

public class AIActionID
{
	public const int AI_IDLE_ACTION		= 10000;
	public const int AI_MOVE_ACTION		= 10001;
	public const int AI_BORN_ACTION		= 61001;
	public const int AI_FLY_GETUP 		= 10003;
	public const int AI_DEAD_ACTION 	= 70001;
}

public class AITimeDef
{
	public const float	AI_MIN_IDLE 	= 1.0f;
	public const float	AI_MAX_IDLE		= 10.0f;
}

public class AIFlyState
{
	public const int 	AFS_STAND 		= -1;
	public const int 	AFS_FLY 		= 2;
}

