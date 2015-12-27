import 'System'
import 'UnityEngine'
import 'Assembly-CSharp'	-- The user-code assembly generated by Unity.
import 'AI'

--[[
--Client level control interface, the script exported the most core interface, you can use 
--All the logic - the interface control CARDS 
--
--]]

function string_split(str, delimiter)
	if str==nil or str=='' or delimiter==nil then
		return nil
	end
	
    local result = {}
    for match in (str..delimiter):gmatch("(.-)"..delimiter) do
        table.insert(result, match)
    end
	
    return result
end

local application_path = Application.dataPath
local package_path = package.path
package.path = string.format("%s;%s/LuaScript/?.lua.bytes", package_path, application_path)

local ary_path = string_split(package.path, ";")

for i, v in pairs(ary_path) do
	if (type(v) == "table") then
		--lua_export_print_table(v)
	else
		Debug.Log("----> Lua input package path "..v)
	end
end


local game_scene = SceneSupport.GetSingleton()
if (not game_scene) then
	Debug.LogError("lua script can't scene support class")
end

local game_engine = GameEngine.GetSingleton()
if (not game_engine) then
	Debug.LogError("lua script can't game engine class")
end

local game_ui = UISystem.GetSingleton()
if (not game_ui) then
	Debug.LogError("lua script can't ui system class")
end

local game_sql = GameSqlLite.GetSingleton()
if (not game_sql) then
	Debug.LogError("lua script can't game sql class")
end

--[[
	print a table key and value
--]]
local function lua_export_print_table( tab )
	for i, v in pairs(tab) do
		if (type(v) == "table") then
			lua_export_print_table(v)
		else
			Debug.Log(">>>>> Register lua name : "..tostring(i).." address : "..tostring(v))
		end
	end
end

--[[
-- add a timer
--
--	@id 		timer id
--	@interval
--	@repat
--	@callback,
-- 	@args
--]]
local function lua_export_add_timer(id, interval, repat, callback, args)
	GameTimer.GetSingleton():Add(id, interval, repat, callback, args)
end

--[[
-- remove a timer
--]]
local function lua_export_remove_timer(id)
	GameTimer.GetSingleton():Remove(id)
end

--[[
	get current scene sql date
--]]
local function lua_export_get_sql_scene()
	return game_sql:Query('SqlScene', game_scene.SceneID)
end

--[[
	load game observer
	
	@ szObserverName observer name
--]]
local function lua_exprot_load_observer(szObserverName)
	local plugin = game_engine:QueryPlugin("LogicPlugin");
	if plugin then
		local observer = plugin:LoadObserver(szObserverName)
		if observer then
			observer:Active()
		end
	end
end

--[[
	unload game observer
	
	@szObserverName observer name
--]]
local function lua_export_unload_observer(szObserverName)
	local plugin = game_engine:QueryPlugin("LogicPlugin");
	if plugin then
		plugin:UnregisterObserver(szObserverName)
	end
end

--[[
-- return current control main player
--]]
local function lua_export_get_main_player()
	local plugin = game_engine:QueryPlugin('PlayerManager')
	if plugin then
		return plugin:GetPlayer()
	end
end

local function lua_export_add_trigger(id, triggerType, vPos, vEuler, vScale, once)
	return TriggerMananger.GetSingleton():CreateTrigger(id, triggerType, vPos, vEuler, vScale, once)
end

local function lua_export_remove_trigger(id)
	TriggerMananger.GetSingleton():DestroyTrigger(id)
end

local function lua_export_get_trigger_array()
	TriggerMananger.GetSingleton():GetTriggerArray()
end

local function lua_export_query_trigger(nID)
	TriggerMananger.GetSingleton():QueryTrigger(nID)
end

--[[
	creat a entity object
	
	@factoryName 
		ET_NPC,		create npc object
		ET_MONSTER	create monster object
		
	@entityType		the entity type
	@nID 			the entity iD
	@vPos 			the entity world position
	@vScale 		the entity scale
	@vEuler 		the entity euler angle
	@nStyle			the entity title style
	@nSqlMonsterID	the entity design config table id
	
--]]
local function lua_export_create_monster(factoryName, entityType, nID, vPos, vScale, vEuler, nStyle, nSqlMonsterID)

	-- call factory create the entity object
	local plugin 	= game_engine:QueryPlugin('MonsterManager')
	if plugin then
		local entity = plugin:CreateEntity(factoryName, entityType, nID, "",
			vPos, vScale, vEuler, nStyle, nSqlMonsterID)
		if entity then
			return entity
		end
	end
end

--[[
-- query a monster
-- 
-- @nID the monster id
--]]
local function lua_export_get_monster(nID)
	local plugin = game_engine:QueryPlugin('MonsterManager')
	if plugin then
		return plugin:GetEntity(nID)
	end
end

--[[
-- destroy a monster
--
-- @nID the monster id
--]]
local function lua_export_destroy_entity(nID)
	local plugin = game_engine:QueryPlugin('MonsterManager')
	if plugin then
		return plugin:DestroyEntity(nID)
	end
end

--[[
-- open or close game joystick
--]]
local function lua_export_enabled_joystick(enabled)
	local plugin = game_engine:QueryPlugin("IJoystickPlugin")
	if plugin then
		if (enabled) then plugin:Activation() else plugin:Deactivate() end
	end
end

--[[
-- open ui
--
-- @szTypeName		ui class type Name
-- @szResourceName	ui layout resource name
--]]
local function lua_export_show_ui(szTypeName, szResourceName)
	return game_ui:LoadWidgetFromTypeName(szTypeName, szResourceName, true)
end

--[[
-- get ui window
--
-- @szResourceName	ui layout resource name
--]]
local function lua_export_get_ui(szResourceName)
	return game_ui:GetWidget(szResourceName)
end

--[[
-- close ui
--
-- @szResourceName	ui window name
--]]
local function lua_export_hide_ui(szResourceName)
	game_ui:UnloadWidget(szResourceName)
end

--[[
-- show game dialog
--
-- @aryText		the param type must lua table type
--
--				example:
--					local t = {
--						"Welcome!"
--					}
--					lua_export_show_dialog(t)	
--]]
local function lua_export_show_dialog(aryText)
	local dialog = lua_export_show_ui("UIDialog", ResourceDef.UI_DIALOG)
	if dialog then
		local aryLength = #aryText
		if aryLength >= 2 then
			for i=2, aryLength do
				dialog:AddText(i, aryText[i])
			end
		end
		
		dialog:SetText(aryText[1])
		dialog:Show()
	end
	
	return dialog
end

--[[
-- close npc dialog
--]]
local function lua_export_hide_dialog()
	local dialog = lua_export_get_ui(ResourceDef.UI_DIALOG)
	if dialog then
		dialog:Clearup()
		dialog:Hide()
	end
end

--[[
-- find a navmesh path, move to target position
-- 
-- @entity 				source entity
-- @targetPosition		move to target position
-- @minErrorDistance	min error distance
--]]
local function lua_export_path_to_position(entity, targetPosition, minErrorDistance)
	-- get the entity machine
	local machine = entity:GetMachine()
	
	-- check is support find path
	if machine:HasState(AITypeID.AI_PATH) == false then 
		return false 
	end

	machine:ChangeState(AITypeID.AI_PATH)

	local evt = CmdEvent.AIFindPathEventArgs()
	evt.Target 		= targetPosition
	evt.MinDistance	= minErrorDistance
	evt.DrawLine	= false
	evt.LineWidth	= 0.05
	
	-- send find path request to entit machine
	machine:PostEvent(
		IEvent(EngineEventType.EVENT_AI, CmdEvent.CMD_LOGIC_AIFINDPATH, evt)
		)
	
	return true
end

--[[
-- start ai server
--]]
local function lua_export_start_ai_server()
	local plugin = game_engine:QueryPlugin('ServerPlugin')
	if plugin then
		plugin:LoadObserver('AIServer')
	end
end

--[[
-- create ai tree
--]]
local function lua_export_create_ai_tree(id, entity, treeFile)
	local asset = Resources.Load(treeFile)
	if not asset then
		return nil
	end
	
	return AIBehaviourTreeManager.GetSingleton():CreateAIBehaviourTree(id, 
		AIEntityContext(entity), asset.text)
end

-- destroy ai tree
local function lua_export_destroy_ai_tree(id)
	AIBehaviourTreeManager.GetSingleton():DestroyAIBehaviourTree(id)
end

--[[
-- close ai server
--]]
local function lua_export_stop_ai_server()
	local plugin = game_engine:QueryPlugin('ServerPlugin')
	if plugin then
		plugin:UnregisterObserver('AIServer')
	end
end

-- export interface
lua_export = {
	print_table	= lua_export_print_table,
	get_sql_scene = lua_export_get_sql_scene,
	get_main_player = lua_export_get_main_player,
	create_monster = lua_export_create_monster,
	get_monster = lua_export_get_monster,
	destroy_entity = lua_export_destroy_entity,
	load_observer = lua_exprot_load_observer,
	unload_observer = lua_export_unload_observer,
	enabled_joystick = lua_export_enabled_joystick,
	show_dialog = lua_export_show_dialog,
	hide_dialog = lua_export_hide_dialog,
	show_ui = lua_export_show_ui,
	hide_ui = lua_export_hide_ui,
	path_to_position = lua_export_path_to_position,
	add_timer = lua_export_add_timer,
	remove_timer = lua_export_remove_timer,
	add_trigger = lua_export_add_trigger,
	remove_tigger = lua_export_remove_trigger,
	get_trigger_array = lua_export_get_trigger_array,
	query_trigger = lua_export_query_trigger,
	start_ai_server = lua_export_start_ai_server,
	create_ai_tree = lua_export_create_ai_tree,
	destroy_ai_tree = lua_export_destroy_ai_tree,
	stop_ai_server = lua_export_stop_ai_server,
}

Debug.Log("------------------------------------ Register lua function -------------------------")
lua_export_print_table(lua_export)
Debug.Log("------------------------------------------------------------------------------------")

--[[
-- Realize the object-oriented in LUA, inheritance, overloading 
	
	emample:
		-- use class() create a class base type
		local BaseClass = class()
		
		-- contruct function
		function BaseClass:ctor()
			Debug.Log("BaseClass ctor")
		end
		
		-- memer function
		function BaseClass:Print()
			Debug.Log("BaseClass Print")
		end

		-- call construct function, create a new object
		local t = BaseClass.new()
		-- call memer function
		t:Print()
		
		local childClass = class(BaseClass)
		
		function childClass:ctor()
			Debug.Log("childClass ctor")
		end
		
		function childClass:Print()
			Debug.Log("childClass Print")
		end
		
--]]
local _class= {
}
 
--[[
-- create a class
--
-- @super super class type
--]]
function class(super)
	local class_type = {
	}
	
	class_type.ctor		= false
	class_type.super	= super
	
	-- c++ new key word
	class_type.new		= function(...) 
			local obj={}
			do
				local create
				create = function(c,...)
					if c.super then
						create(c.super,...)
					end
					
					-- contruct function
					if c.ctor then
						c.ctor(obj,...)
					end
				end
 
				create(class_type,...)
			end
			
			setmetatable(obj, { __index=_class[class_type] })
			return obj
		end
		
	local vtbl={}
	_class[class_type]=vtbl
 
	setmetatable(class_type,{__newindex=
		function(t,k,v)
			vtbl[k]=v
		end
	})
 
	if super then
		setmetatable(vtbl,{__index=
			function(t,k)
				local ret=_class[super][k]
				vtbl[k]=ret
				return ret
			end
		})
	end
 
	return class_type
end