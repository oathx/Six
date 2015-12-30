#include "ScriptManager.h"

extern "C"{
#include "lua_cjson.h"
#include "lua_extensions.h"
};

namespace Server
{
	ScriptManager*	ScriptManager::mpSingleton = NULL;
	/**
	 *
	 * \return 
	 */
	ScriptManager*	ScriptManager::GetSingleton()
	{
		return mpSingleton;
	}

	/**
	 *
	 * \param void 
	 * \return 
	 */
	ScriptManager::ScriptManager(void)
		: m_pLuaState(NULL)
	{
		m_pLuaState = luaL_newstate();
		if (m_pLuaState != NULL)
		{
			luaL_openlibs(m_pLuaState);
			luaL_cjson(m_pLuaState);
		}
	}

	/**
	 *
	 * \param void 
	 * \return 
	 */
	ScriptManager::~ScriptManager(void)
	{
		if (m_pLuaState != NULL)
			lua_close(m_pLuaState);
	}

	/**
	 *
	 * \param szScriptFileName 
	 * \return 
	 */
	int		ScriptManager::executeScriptFile(const String& szScriptFileName)
	{
		if (!szScriptFileName.empty())
		{
			if(luaL_dofile(m_pLuaState, szScriptFileName.c_str()) != 0)
			{
				printf("lua error: file [%s] %s", szScriptFileName.c_str(), luaL_checkstring(m_pLuaState, -1));
				lua_pop(m_pLuaState, 1);
			}
		}

		return 0;
	}

	/**
	 *
	 * \param szScriptString 
	 * \return 
	 */
	int		ScriptManager::executeScriptString(const String& szScriptString)
	{
		if (!szScriptString.empty())
			return luaL_dostring(m_pLuaState, szScriptString.c_str());
	
		return 0;		
	}
}
