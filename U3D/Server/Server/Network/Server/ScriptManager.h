#ifndef _____ScriptManager_H
#define _____ScriptManager_H

#include "Singleton.h"

namespace Server
{
	//////////////////////////////////////////////////////////////////////////
	//
	//////////////////////////////////////////////////////////////////////////
	class ScriptManager : public Singleton<ScriptManager>
	{
	public:
		/**
		 *
		 * \return 
		 */
		static ScriptManager*	GetSingleton();

	public:
		/**
		 *
		 * \param void 
		 * \return 
		 */
		ScriptManager(void);

		/**
		 *
		 * \param void 
		 * \return 
		 */
		virtual ~ScriptManager(void);

	public:
		/**
		 *
		 * \param szScriptFileName 
		 * \return 
		 */
		virtual	int				executeScriptFile(const String& szScriptFileName);

		/**
		 *
		 * \param szScriptString 
		 * \return 
		 */
		virtual int				executeScriptString(const String& szScriptString);
	protected:
		lua_State*				m_pLuaState;
		
	};
}

#endif