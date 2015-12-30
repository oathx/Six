#ifndef _____ClientManager_H
#define _____ClientManager_H

#include "Singleton.h"
#include "Client.h"

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
	class ClientManager : public Singleton<ClientManager>
	{
		typedef std::map<int, Client*>	ClientRegister;
	public:
		/**
		 *
		 * \return 
		 */
		static ClientManager*	GetSingleton();

	public:
		/**
		 *
		 * \param void 
		 * \return 
		 */
		ClientManager();

		/**
		 *
		 * \param void 
		 * \return 
		 */
		virtual ~ClientManager(void);

		/**
		 *
		 * \param nID 
		 * \param ipAddress 
		 * \param nPort 
		 * \return 
		 */
		virtual void			RegisterServerClient(Client* pClient);

		/**
		 *
		 */
		virtual void			Run();

		/**
		 *
		 * \param nID 
		 */
		virtual void			UnregisterServerClient(int nID);

		/**
		 *
		 * \param nID 
		 * \param jp 
		 * \return 
		 */
		virtual int				Send(int nClientID, int nProtocolID, JsonProtocol& jp);

		/**
		 *
		 * \param nClientID 
		 * \param nProtocolID 
		 * \return 
		 */
		virtual int				Send(int nClientID, int nProtocolID);
	protected:
		ClientRegister			m_ClientRegister;
	};
}

#endif