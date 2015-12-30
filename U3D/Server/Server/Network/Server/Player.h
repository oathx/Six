#ifndef _____Player_H
#define _____Player_H

#include "Session.h"
#include "PlayerEvent.h"

namespace Server
{
	/**
	* \ingroup : Server
	*
	* \os&IDE  : Microsoft Windows XP (SP3)  &  Microsoft Visual C++ .NET 2008 & ogre1.8
	*
	* \VERSION : 1.0
	*
	* \@date   : 2014-05-11
	*
	* \Author  : lp
	*
	* \Desc    : 
	*
	* \bug     : 
	*
	*/
	class Player : public Session
	{
		typedef std::map<int, PlayerEvent*> PlayerEventList;
	public:
		/**
		 *
		 * \param nHandle 
		 * \return 
		 */
		Player(SOCKET hSocket);

		/**
		 *
		 * \param void 
		 * \return 
		 */
		virtual ~Player(void);

		/**
		 *
		 * \param pEvent 
		 */
		virtual void			RegisterEventListener(PlayerEvent* pEvent);

		/**
		 *
		 * \param pEvent 
		 */
		virtual void			UnregisterEventListener(PlayerEvent* pEvent);
	public:
		/**
		 *
		 * \param pBuffer 
		 * \param nLength 
		 * \return 
		 */
		virtual int				DispatchEvent(int nID, void* pData, int nLength);
	protected:
		PlayerEventList			m_PlayerEventList;
	};
}

#endif
