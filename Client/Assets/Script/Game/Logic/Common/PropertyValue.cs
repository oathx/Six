public enum  PropertyType
{
	NONE								= -1,
	PT_FightValue						= 0,
	PT_Combat							= 1,
	PT_Firecontrol						= 2,
	PT_Protection						= 3,
	PT_Performance						= 4,
	PT_Durable							= 5,
	PT_CloseinAttack					= 6,
	PT_CloseinDefense					= 7,
	PT_DistanceAttack					= 8,
	PT_DistanceDefense					= 9,
	PT_Crit								= 10,
	PT_AntiCrit							= 11,
	PT_Hit								= 12,
	PT_Dodge							= 13,
	PT_IncreasedDamage					= 14,
	PT_IncreasedDamagePercentage		= 15,
	PT_DamageReduction					= 16,
	PT_DamageReductionPercentage		= 17,
	PT_CritIncreasePercentage			= 18,
	PT_CritReducePercentage				= 19,
	PT_CritHurt							= 20,
	PT_CritHurtPercentage				= 21,
	PT_CritDamageReduction				= 22,
	PT_CritReductionPercentage			= 23,
	PT_HitIncreasePercentage			= 24,
	PT_HitReductionPercentage			= 25,
	PT_DodgeIncreasePercentage			= 26,
	PT_DodgeReductionPercentage			= 27,
	PT_IgnoringDefense					= 28,
	PT_IgnoringDefensePercentage		= 29,
	PT_Energy							= 30,
	PT_Attack                           = 31,
	PT_AttackPercent                    = 32,
	PT_Damage                           = 33,
	PT_DamagePercent                    = 34,
	PT_numberMAX                        = 35,
	PT_HP								= 50,
	PT_MP								= 51,
	PT_Money							= 100,
	PT_EMoney							= 101,
	PT_BagSlot							= 102,
	PT_StroeSlot						= 103,
	PT_Exp								= 104,
	PT_HONOR							= 105,
	PT_LEVEL							= 106,
	PT_MAKEINDEX						= 107,
	PT_ENERGYPIECE						= 108,
	PT_CORESTARTSTONE					= 109,
	PT_STONE_BUYTIMES					= 110,
	PT_MAX_POWER_TIME					= 111,
	PT_GET_GIFT_ID						= 112,
	PT_YWC_TIMES						= 113,
	PT_ONLINE_GIFT_ID					= 114,
	PT_YWC_BUY_TIMES					= 115,
	PT_NEXT_GIFT_TIME					= 116,
	PT_TOTAL_ATTEN_ID					= 117,
	PT_UNITL_ATTEN_ID					= 118,
	PT_LAST_ATTEN_TIME					= 119,
	PT_MAX_POWER_BUY_TIMES				= 120,
	PT_BUY_TOTAL_EMONEY					= 121,
	PT_LAST_VIP_GIFT_ID					= 122,
	PT_DBQB_PLAY_TIMES					= 123,
	PT_DBQB_BUY_TIMES					= 124,
	PT_GROW_ID							= 125,
	PT_XZSL_BUY_TIMES					= 126,
	PT_XZSL_PLAY_TIMES					= 127,
	PT_TRIAL_BUY_NUM                    = 128,
	PT_TOTAL_TOTAL_ATTENDANCE_NUM		= 129,
	PT_ATTR_VIGOR_EXTRA                 = 130,
	PT_WHITE_CHESTS_FREE_TIME           = 131,
	PT_TRIAL_REFRESH_NUM                = 132,
	PT_FINISH_TRIAL_NUM                 = 133,
	PT_UPDATE_TRIAL_ID                  = 134,
	PT_ACCEPT_TRIAL_NUM                 = 135,
	PT_JJC_BEST_RANK					= 137,
	PT_JJC_BUY_TIMES					= 138,
	PT_JJC_PLAY_TIMES					= 139,
	PT_JJC_PLAY_CD						= 140,
	PT_JJC_HONOR						= 141,
	PT_TRIAL_TOWER_CUR_FLOOR            = 142,
	PT_TRIAL_TOWER_MAX_FLOOR            = 143,
	PT_TRIAL_TOWER_RESET_NUM            = 144,
	PT_STORE_SERVER_REFRESH_TIME        = 145,
	PT_STORE_REFRESH_TIMES              = 146,
	PT_STORE_BUY_ITEM_NUMBER            = 147,
	PT_SKILL_POINT						= 148,
	PT_LIVENESS_POINT                   = 149,
	PT_LIVENESS_STATE                   = 150,
	PT_ENERGY_GIFT_STATE                = 151,
	PT_GOLD_TIMES                       = 152,
}

/// <summary>
/// Property value.
/// </summary>
public class PropertyValue
{
	public PropertyType		Type
	{ get; set; }
	
	public int 				Value
	{ get; set; }
	
	/// <summary>
	/// Initializes a new instance of the <see cref="PropertyValue"/> class.
	/// </summary>
	public PropertyValue()
	{}
	
	/// <summary>
	/// Initializes a new instance of the <see cref="PropertyValue"/> class.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="v">V.</param>
	public PropertyValue(PropertyType type, int v)
	{
		Type = type; Value = v;
	}
}
