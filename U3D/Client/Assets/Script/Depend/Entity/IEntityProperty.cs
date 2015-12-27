using UnityEngine;
using System.Collections;

public class EntityStyle
{
	public const int ES_NAME		= 1 << 1;
	public const int ES_LEVEL		= 1 << 2;
	public const int ES_TASK		= 1 << 3;
	public const int ES_FNISHTASK	= 1 << 4;
	public const int ES_LIFE		= 1 << 5;
	public const int ES_PING		= 1 << 6;
	public const int ES_BOSS		= 1 << 7;
	
	public const int ES_NPC			= ES_NAME|ES_TASK;
	public const int ES_MONSTER		= ES_NAME|ES_LIFE;
	public const int ES_PLAYER		= ES_NAME|ES_LIFE|ES_LEVEL;
}

public enum EntityType
{
	ET_NONE			= 0,
	ET_ACTOR		= 1,
	ET_NPC			= 2,
	ET_PLAYER		= 3,
	ET_MAIN			= 4,
	ET_EFFECT		= 5,
	ET_DURATION		= 6,
	ET_TRIGGER		= 7,
	ET_DOOR			= 8,
	ET_DESTRUCTIVE	= 9,
	ET_MONSTER		= 10,
	ET_BOSS			= 11,
}

/// <summary>
/// I enity property.
/// </summary>
public class IEntityProperty : MonoBehaviour
{
	public int 			ID;
	public EntityType	Type;
	public int			Style;
	public float 		Gravity;
	public string		Name;
	public string		Describe;
	public float		MaxMoveSpeed;
	public float		MaxRotateSpeed;
	public int 			ShapeID;
}
