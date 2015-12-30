#pragma once

#include "PlayerEvent.h"

namespace Server
{
	class LoginServerObserver : public PlayerEvent
	{
	public:
		LoginServerObserver(int nID);
		virtual ~LoginServerObserver();
	};
}
