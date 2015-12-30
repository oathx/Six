#include "Player.h"

namespace Server
{
	/**
	 *
	 * \param void 
	 * \return 
	 */
	Player::Player(SOCKET hSocket)
		: Session(hSocket)
	{

	}

	/**
	 *
	 * \param void 
	 * \return 
	 */
	Player::~Player(void)
	{
		PlayerEventList::iterator it = m_PlayerEventList.begin();
		while( it != m_PlayerEventList.end() )
		{
			delete it->second; it = m_PlayerEventList.erase(it);
		}
	}

	/**
	 *
	 * \param pEvent 
	 */
	void			Player::RegisterEventListener(PlayerEvent* pEvent)
	{
		PlayerEventList::iterator it = m_PlayerEventList.find(pEvent->GetID());
		if (it == m_PlayerEventList.end())
		{
			pEvent->SetSession(this);

			m_PlayerEventList.insert(
				PlayerEventList::value_type(pEvent->GetID(), pEvent)
				);
		}
	}

	/**
	 *
	 * \param pEvent 
	 */
	void			Player::UnregisterEventListener(PlayerEvent* pEvent)
	{
		PlayerEventList::iterator it = m_PlayerEventList.find(pEvent->GetID());
		if (it != m_PlayerEventList.end())
		{
			delete it->second; m_PlayerEventList.erase(it);
		}
	}

	/**
	 *
	 * \param pBuffer 
	 * \param nLength 
	 * \return 
	 */
	int				Player::DispatchEvent(int nID, void* pData, int nLength)
	{
#ifdef _DEBUG
		printf("Recv ID: %d Length: %d \n", nID, nLength);
#endif
		
		for (PlayerEventList::iterator it=m_PlayerEventList.begin();
			it!=m_PlayerEventList.end(); it++)
		{
			if (it->second->HasEvent(nID))
			{
				it->second->FireEvent(nID, PlayerEventArgs(nID, (char*)pData, nLength));
			}
		}

		return 0;
	}
}
