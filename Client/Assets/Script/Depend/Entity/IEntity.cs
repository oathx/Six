using UnityEngine;
using System.Collections;

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
/// I entity.
/// </summary>
public class IEntity : MonoBehaviour
{

}
