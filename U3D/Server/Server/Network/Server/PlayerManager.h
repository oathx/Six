#ifndef _____PlayerManager_H
#define _____PlayerManager_H

#include "Singleton.h"
#include "Player.h"

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
	class PlayerManager : public Singleton<PlayerManager>
	{
		// socket map table
		typedef std::map<SOCKET, Player*>	SocketPlayer;

	public:
		/**
		 *
		 * \return 
		 */
		static PlayerManager*	GetSingleton();

	public:
		/**
		 *
		 * \param void 
		 * \return 
		 */
		PlayerManager();

		/**
		 *
		 * \param void 
		 * \return 
		 */
		virtual ~PlayerManager(void);

		/**
		 *
		 * \param hSocket 
		 * \return 
		 */
		virtual	Player*			CreatePlayer(SOCKET hSocket);

		/**
		 *
		 * \param hSocket 
		 * \return 
		 */
		virtual Player*			Query(SOCKET hSocket);

		/**
		 *
		 * \param hSocket 
		 */
		virtual void			DestroyPlayer(SOCKET hSocket);

		/**
		 *
		 * \return 
		 */
		virtual size_t			GetCount() const;

	protected:
		SocketPlayer			m_SocketPlayer;
	};
}

#endif