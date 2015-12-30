#pragma once

#include "Client.h"

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
	class DataBaseClient : public Client
	{
	public:
		/**
		 *
		 * \param nID 
		 * \param ipAddress 
		 * \param nPort 
		 * \return 
		 */
		DataBaseClient(int nID, const String& ipAddress, int nPort);

		/**
		 *
		 * \param void 
		 * \return 
		 */
		virtual ~DataBaseClient(void);
	};
}
