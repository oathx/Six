#include "ClientManager.h"

namespace Server
{
	
	ClientManager*	ClientManager::mpSingleton = NULL;
	/**
	 *
	 * \return 
	 */
	ClientManager*	ClientManager::GetSingleton()
	{
		return mpSingleton;
	}


	/**
	 *
	 * \param void 
	 * \return 
	 */
	ClientManager::ClientManager(void)
	{
	}

	/**
	 *
	 * \param void 
	 * \return 
	 */
	ClientManager::~ClientManager(void)
	{
		ClientRegister::iterator it = m_ClientRegister.begin();
		while ( it != m_ClientRegister.end() )
		{
			it->second->Close(); delete it->second; it = m_ClientRegister.erase(it);
		}
	}

	/**
	 *
	 * \param nID 
	 * \param ipAddress 
	 * \param nPort 
	 * \return 
	 */
	void		ClientManager::RegisterServerClient(Client* pClient)
	{
		ClientRegister::iterator it = m_ClientRegister.find(pClient->GetID());
		if (it == m_ClientRegister.end())
		{
			m_ClientRegister.insert(ClientRegister::value_type(pClient->GetID(), pClient));
		}
	}

	/**
	 *
	 */
	void		ClientManager::Run()
	{
		for (ClientRegister::iterator it=m_ClientRegister.begin(); 
			it!=m_ClientRegister.end(); it++)
		{
			if (!it->second->IsConnected())
			{
				it->second->Close();

				String	ipAddress	= it->second->GetIPAdress();
				int		nPort		= it->second->GetPort();

				if (!it->second->Connect(ipAddress, nPort))
				{
#if _DEBUG
					printf("Try connect center server %s:%d Failed\n", ipAddress.c_str(), nPort);
#endif
				}
				else
				{
#if _DEBUG
					printf("Try connect center server %s:%d Success\n", ipAddress.c_str(), nPort);
#endif
				}
			}
			else
			{
				it->second->Update();
			}
		}
	}

	/**
	 *
	 * \param nID 
	 */
	void		ClientManager::UnregisterServerClient(int nID)
	{
		ClientRegister::iterator it = m_ClientRegister.find(nID);
		if ( it != m_ClientRegister.end() )
		{
			it->second->Close(); delete it->second; it = m_ClientRegister.erase(it);
		}
	}

	/**
	 *
	 * \param nClientID 
	 * \param nProtocolID 
	 * \param jp 
	 * \return 
	 */
	int			ClientManager::Send(int nClientID, int nProtocolID, JsonProtocol& jp)
	{
		ClientRegister::iterator it = m_ClientRegister.find(nClientID);
		if ( it != m_ClientRegister.end() )
		{
			return it->second->Send(nProtocolID, jp);
		}

		return 0;
	}

	/**
	 *
	 * \param nClientID 
	 * \param nProtocolID 
	 * \return 
	 */
	int			ClientManager::Send(int nClientID, int nProtocolID)
	{
		ClientRegister::iterator it = m_ClientRegister.find(nClientID);
		if ( it != m_ClientRegister.end() )
		{
			return it->second->Send(nProtocolID);
		}

		return 0;
	}
}
