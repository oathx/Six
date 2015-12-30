#pragma once

#include "PlayerEvent.h"

namespace Server
{
	/**
	* \ingroup : LoginServer
	*
	* \os&IDE  : Microsoft Windows XP (SP3)  &  Microsoft Visual C++ .NET 2008 & ogre1.8
	*
	* \VERSION : 1.0
	*
	* \@date   : 2014-05-13
	*
	* \Author  : 
	*
	* \Desc    : 
	*
	* \bug     : 
	*
	*/
	class LoginEvent : public PlayerEvent
	{
	public:
		/**
		 *
		 * \param nID 
		 * \return 
		 */
		LoginEvent(int nID);

		/**
		 *
		 * \param void 
		 * \return 
		 */
		virtual ~LoginEvent(void);

	protected:
		/**
		 *
		 * \param args 
		 * \return 
		 */
		virtual bool		OnLoginRequest(const EventArgs& args);
	};

}
