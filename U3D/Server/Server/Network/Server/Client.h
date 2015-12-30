#pragma once

#include "PlayerEvent.h"

namespace Server
{
	/**
	* \ingroup : Server
	*
	* \os&IDE  : Microsoft Windows XP (SP3)  &  Microsoft Visual C++ .NET 2008
	*
	* \VERSION : 1.0
	*
	* \@date   : 2014-05-09
	*
	* \Author  : 
	*
	* \Desc    : 
	*
	* \bug     : 
	*
	*/
	class Client : public EventSet
	{
	public:
		/**
		 *
		 * \param void 
		 * \return 
		 */
		Client(int nID);

		/**
		 *
		 * \param void 
		 * \return 
		 */
		virtual ~Client(void);

	public:
		/**
		 *
		 * \return 
		 */
		virtual int			GetID() const;
		/**
		 *
		 * \param ipAddress 
		 * \param nPort 
		 * \return 
		 */
		virtual bool		Connect(const String& ipAddress, int nPort);

		/**
		 *
		 * \param nID 
		 * \param subscriber 
		 */
		virtual void		SubscribeEvent(int nID, FunctorEvent::Subscriber subscriber);

		/**
		 *
		 * \param nID 
		 * \return 
		 */
		virtual	bool		HasEvent(int nID);

		/**
		 *
		 * \param nID 
		 * \param args 
		 * \param /* 
		 * \return 
		 */
		virtual void		FireEvent(int nID, PlayerEventArgs& args);

		/**
		 *
		 */
		virtual void		Close();

		/**
		 *
		 * \param fElapsed 
		 * \return 
		 */
		virtual void		Update();

		/**
		 *
		 * \return 
		 */
		virtual	bool		IsConnected() const;

		/**
		 *
		 * \return 
		 */
		virtual String		GetIPAdress() const;

		/**
		 *
		 * \return 
		 */
		virtual int			GetPort() const;

		/**
		 *
		 * \param pData 
		 * \param nLength 
		 * \return 
		 */
		virtual int			Send(const char* pData, int nLength);

		/**
		 *
		 * \param nID 
		 * \return 
		 */
		virtual int			Send(int nID);

		/**
		 *
		 * \param nID 
		 * \param jp 
		 * \return 
		 */
		virtual	int			Send(int nID, JsonProtocol& jp);
	protected:
		int					m_nID;
		String				m_IPAdress;
		int					m_nPort;
		SOCKET				m_hSocket;
		char*				m_pRecvBuf;
		char*				m_pDataBuf;
		int					m_nRecvSize;
	};
}
