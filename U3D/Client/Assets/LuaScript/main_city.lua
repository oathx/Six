import 'System'
import 'UnityEngine'
import 'Assembly-CSharp'	-- The user-code assembly generated by Unity.

module("main_city", package.seeall)
	
--[[
-- create main ctiy all npc object
-- 
-- @sqlScene 	current senen config data
-- @bindTrigger	is create a null trigger and bind the trigger to npc
-- @triggerOnce	trigger once?
--]]
local function create_scene_static_npc(sqlScene, bindTrigger, triggerOnce)
	
	-- query current scene all npc info
	local group = MonsterTable.GetSingleton():GetGroupMonster(sqlScene, 0)
	if not group then
		return false
	end
	
	-- create npc object
	local enumerator = group:GetEnumerator()
	while enumerator:MoveNext() do
		local npc 	= enumerator.Current
		if npc.Type == EntityType.ET_NPC then
			lua_export.create_monster("ET_NPC", EntityType.ET_NPC, npc.NpcID, 
								npc.Position, Vector3.one, Vector3.zero, 0, npc.NpcID)
			
			-- if need bind a null type trigger
			if bindTrigger then
				TriggerMananger.GetSingleton():CreateTrigger(npc.NpcID,
							TriggerType.TRIGGER_NULL, npc.Position, Vector3.zero, Vector3(4, 1, 4), triggerOnce)
			end
		end
	end
	
	return true
end

--[[
-- move main player to npc entity
--]]
local function move_to_entity(entity)
	local player = lua_export.get_main_player()
	if entity then
		lua_export.path_to_position(player, entity:GetPosition(), 2.0)
	end
end

--[[
-- create test actor
--
-- @count actor count
--]]
local function create_test_actor(count)
	Physics.IgnoreLayerCollision(EntityLayer.MONSTER, EntityLayer.MONSTER);
	
	for id=0, count, 1 do
		local actor = lua_export.create_monster("ET_MONSTER", EntityType.ET_MONSTER, -id, 
						Vector3(-4.3, 2, -32), Vector3.one, Vector3.zero, 0, 1001)
	end
end

-- scene loading finish
function OnSceneStart()
	-- load modle observer
	-- 		this module manages all game system will automatically 
	-- 		load in SqlSpread conform to the function modules of the open condition 
	-- if current in battle duplicate then unload this modle
	lua_export.load_observer('ModleObserver')

	-- open joystick
	lua_export.enabled_joystick(true)
	
	-- start ai server
	lua_export.start_ai_server()
end


--[[
-- scene update, 
-- Calls per frame, if you don't need to deal with, is the best bad statement this interface, 
-- affect efficiency 
]]
function OnSceneUpdate()
	
end

--[[
-- scene destroy
--]]
function OnSceneDestroy()

end


