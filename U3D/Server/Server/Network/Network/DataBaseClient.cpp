#include "StdAfx.h"
#include "DataBaseClient.h"

namespace Server
{
	/**
	 *
	 * \param nID 
	 * \param ipAddress 
	 * \param nPort 
	 * \return 
	 */
	DataBaseClient::DataBaseClient(int nID, const String& ipAddress, int nPort)
		: Client(nID)
	{
		// connect to server
		Connect(ipAddress, nPort);
	}

	/**
	 *
	 * \param void 
	 * \return 
	 */
	DataBaseClient::~DataBaseClient(void)
	{

	}

}
