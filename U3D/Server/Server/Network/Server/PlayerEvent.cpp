#include "PlayerEvent.h"
#include "Session.h"

namespace Server
{
	/**
	 *
	 * \param void 
	 * \return 
	 */
	PlayerEvent::PlayerEvent(int nID)
		: m_nID(nID), m_pSession(NULL)
	{
	
	}

	/**
	 *
	 * \param void 
	 * \return 
	 */
	PlayerEvent::~PlayerEvent(void)
	{
	}

	/**
	 *
	 * \return 
	 */
	int			PlayerEvent::GetID() const
	{
		return m_nID;
	}
	
	/**
	 *
	 * \param nID 
	 * \param subscriber 
	 */
	void		PlayerEvent::SubscribeEvent(int nID, FunctorEvent::Subscriber subscriber)
	{
		char chName[32];
		sprintf(chName, "%d", nID);

		subscribeEvent(chName, subscriber);
	}

	/**
	 *
	 * \param nID 
	 * \return 
	 */
	bool		PlayerEvent::HasEvent(int nID)
	{
		char chName[32];
		sprintf(chName, "%d", nID);

		return isEventPresent(chName);
	}

	/**
	 *
	 * \param nID 
	 * \param args 
	 * \param eventNamespace 
	 * \return 
	 */
	void		PlayerEvent::FireEvent(int nID, PlayerEventArgs& args)
	{
		char chName[32];
		sprintf(chName, "%d", nID);

		fireEvent_impl(chName, args);
	}

	/**
	 *
	 * \param pSession 
	 */
	void		PlayerEvent::SetSession(Session* pSession)
	{
		m_pSession = pSession;
	}

	/**
	 *
	 * \return 
	 */
	Session*	PlayerEvent::GetSession() const
	{
		return m_pSession;
	}

	/**
	 *
	 * \param pData 
	 * \param nLength 
	 * \return 
	 */
	int			PlayerEvent::Send(const void* pData, int nLength)
	{
		return m_pSession->SendBuffer(pData, nLength);
	}

	/**
	 *
	 * \param nID 
	 * \return 
	 */
	int			PlayerEvent::Send(int nID)
	{
		char chBuffer[HEAD_LENGTH];

		NetHead* pHead	= (NetHead*)(chBuffer);
		pHead->ID		= nID;
		pHead->Length	= HEAD_LENGTH;

		return Send(chBuffer, HEAD_LENGTH);
	}

	/**
	 *
	 * \param jp 
	 * \return 
	 */
	int			PlayerEvent::Send(int nID, JsonProtocol& jp)
	{
		int nLength	= jp.MakeNetStream(nID);

#if _DEBUG
		printf("Send ID: %d text:%s Length: %d\n", nID, jp.ToString().c_str(), nLength);
#endif
		if (nLength != 0)
			return m_pSession->SendBuffer(jp.GetBuffer(), nLength);

		return -1;
	}
}
