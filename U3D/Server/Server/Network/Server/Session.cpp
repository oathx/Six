#include "Session.h"

namespace Server
{
	/**
	 *
	 * \param hSocket 
	 * \return 
	 */
	Session::Session(SOCKET hSocket)
		: m_hSocket(hSocket), m_pRecvBuf(NULL), m_nRecvSize(0)
	{
		// net buffer
		m_pRecvBuf = new char[MAX_BUFFERLENGTH];
		memset(
			m_pRecvBuf, 0, MAX_BUFFERLENGTH
			);

		// pack buffer
		m_pDataBuf = new char[MAX_BUFFERLENGTH];
		memset(
			m_pDataBuf, 0, MAX_BUFFERLENGTH
			);
	}

	/**
	 *
	 * \param void 
	 * \return 
	 */
	Session::~Session(void)
	{
		if (m_pRecvBuf)
		{
			delete[] m_pRecvBuf;
			m_pRecvBuf = NULL;
		}

		if (m_pDataBuf)
		{
			delete[] m_pDataBuf;
			m_pDataBuf = NULL;
		}
	}
	
	/**
	 *
	 * \return 
	 */
	SOCKET			Session::GetSocket() const
	{
		return m_hSocket;
	}
	
	
	/**
	 *
	 * \param pBuffer 
	 * \param nLength 
	 * \return 
	 */
	int				Session::Update(void* pBuffer, short nLength)
	{
		memcpy(
			m_pRecvBuf + m_nRecvSize, pBuffer, nLength
			);

		m_nRecvSize += nLength;

		while(m_nRecvSize >= HEAD_LENGTH)
		{
			// get pack head
			NetHead* pHead = (NetHead*)m_pRecvBuf;
			if (m_nRecvSize >= pHead->Length)
			{
				int nCopyLength = pHead->Length - HEAD_LENGTH;

				// copy pack data to data buffer
				CopyMemory(
					m_pDataBuf, pHead + 1, nCopyLength
					);
	
				m_nRecvSize -= pHead->Length;

				MoveMemory(
					m_pRecvBuf, m_pRecvBuf + pHead->Length, m_nRecvSize);
				
				// dispatch net event
				DispatchEvent(
					pHead->ID, m_pDataBuf, pHead->Length - HEAD_LENGTH
					);
				memset(m_pDataBuf, 0, MAX_BUFFERLENGTH);
			}
		}

		return 0;
	}

	
	/**
	 *
	 * \param nID 
	 * \param pData 
	 * \param nLength 
	 * \return 
	 */
	int				Session::DispatchEvent(int nID, void* pData, int nLength)
	{
		return 0;
	}


	/**
	 *
	 * \param pBufferEvent 
	 */
	void			Session::SetBufferEvent(bufferevent* pBufferEvent)
	{
		m_pBufferEvent = pBufferEvent;
	}

	/**
	 *
	 * \return 
	 */
	bufferevent*	Session::GetBufferEvent() const
	{
		return m_pBufferEvent;
	}

	/**
	 *
	 * \param pData 
	 * \param nLength 
	 * \return 
	 */
	int				Session::SendBuffer(const void* pData, int nLength)
	{
		return bufferevent_write(m_pBufferEvent, pData, nLength);
	}
}
