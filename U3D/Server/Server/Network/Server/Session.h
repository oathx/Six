#ifndef _____Session_H
#define _____Session_H

#include "NetProtocol.h"

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
	class Session
	{
	public:
		/**
		 *
		 * \param hSocket 
		 * \return 
		 */
		Session(SOCKET hSocket);

		/**
		 *
		 * \param void 
		 * \return 
		 */
		virtual ~Session(void);

	public:
		/**
		 *
		 * \return 
		 */
		virtual	SOCKET			GetSocket() const;
		
		/**
		 *
		 * \param nID 
		 * \param pData 
		 * \param nLength 
		 * \return 
		 */
		virtual int				DispatchEvent(int nID, void* pData, int nLength);

		/**
		 *
		 * \param pBuffer 
		 * \param nLength 
		 * \return 
		 */
		virtual int				Update(void* pBuffer, short nLength);

		/**
		 *
		 * \param pBufferEvent 
		 */
		virtual void			SetBufferEvent(bufferevent* pBufferEvent);

		/**
		 *
		 * \return 
		 */
		virtual bufferevent*	GetBufferEvent() const;

		/**
		 *
		 * \param pData 
		 * \param nLength 
		 * \return 
		 */
		virtual int				SendBuffer(const void* pData, int nLength);

	protected:
		bufferevent*			m_pBufferEvent;
		SOCKET					m_hSocket;
		char*					m_pRecvBuf;
		char*					m_pDataBuf;
		int						m_nRecvSize;
	};
}

#endif