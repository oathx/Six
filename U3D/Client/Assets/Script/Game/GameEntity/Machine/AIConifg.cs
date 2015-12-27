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

