#include "PlayerManager.h"

namespace Server
{

	PlayerManager*	PlayerManager::mpSingleton = NULL;
	/**
	 *
	 * \return 
	 */
	PlayerManager*	PlayerManager::GetSingleton()
	{
		return mpSingleton;
	}

	/**
	 *
	 * \param void 
	 * \return 
	 */
	PlayerManager::PlayerManager(void)
	{
	}

	/**
	 *
	 * \param void 
	 * \return 
	 */
	PlayerManager::~PlayerManager(void)
	{
	}

	/**
	 *
	 * \param hSocket 
	 * \return 
	 */
	Player*		PlayerManager::CreatePlayer(SOCKET hSocket)
	{
		Player* player = new Player(hSocket);
		if (player != NULL)
		{
			m_SocketPlayer.insert(
				SocketPlayer::value_type(hSocket, player)
				);

			return player;
		}

		return NULL;
	}

	/**
	 *
	 * \param hSocket 
	 * \return 
	 */
	Player*		PlayerManager::Query(SOCKET hSocket)
	{
		SocketPlayer::iterator it = m_SocketPlayer.find(hSocket);
		if (it != m_SocketPlayer.end())
		{
			return it->second;
		}

		return NULL;
	}

	/**
	 *
	 * \param hSocket 
	 */
	void		PlayerManager::DestroyPlayer(SOCKET hSocket)
	{
		SocketPlayer::iterator it = m_SocketPlayer.find(hSocket);
		if (it != m_SocketPlayer.end())
		{
			delete it->second; m_SocketPlayer.erase(it);
		}	
	}

	/**
	 *
	 * \return 
	 */
	size_t		PlayerManager::GetCount() const
	{
		return m_SocketPlayer.size();
	}
}
