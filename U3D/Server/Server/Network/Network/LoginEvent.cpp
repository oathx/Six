#include "StdAfx.h"
#include "LoginEvent.h"
#include "ClientManager.h"

namespace Server
{
	/**
	 *
	 * \param nID 
	 * \return 
	 */
	LoginEvent::LoginEvent(int nID)
		: PlayerEvent(nID)
	{
		SubscribeEvent(CS_NET_LOGIN,	SubscriberSlot(&LoginEvent::OnLoginRequest, this));
	}

	/**
	 *
	 * \param void 
	 * \return 
	 */
	LoginEvent::~LoginEvent(void)
	{

	}

	/**
	 *
	 * \param args 
	 * \return 
	 */
	bool	LoginEvent::OnLoginRequest(const EventArgs& args)
	{
		const PlayerEventArgs& evt = static_cast<const PlayerEventArgs&>(args);
		if (evt.Buffer != NULL)
		{

		}

		return true;
	}
}

