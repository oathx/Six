using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MagicStudyState
{
	STUDY_NONE,
	STUDY_STUDY,
	STUDY_OWNER,
}

/// <summary>
/// Skill struct.
/// </summary>
public class MagicGroup : IEventArgs
{

}

/// <summary>
/// Character table.
/// </summary>
public class SkillTable : MemoryTable<SkillTable, int, MagicGroup>
{	

}


