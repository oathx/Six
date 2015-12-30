#include "Client.h"

namespace Server
{
	/**
	 *
	 * \param void 
	 * \return 
	 */
	Client::Client(int nID)
		: m_nID(nID), m_hSocket(INVALID_SOCKET), m_nPort(0), m_pRecvBuf(NULL), m_pDataBuf(NULL), m_nRecvSize(0)
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
	Client::~Client(void)
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

		Close();
	}

	/**
	 *
	 * \return 
	 */
	int		Client::GetID() const
	{
		return m_nID;
	}

	/**
	 *
	 * \param ipAddress 
	 * \param nPort 
	 * \return 
	 */
	bool	Client::Connect(const String& ipAddress, int nPort)
	{
		m_IPAdress	= ipAddress;
		m_nPort		= nPort;

		m_hSocket = ::socket(AF_INET,SOCK_STREAM,0);
		if (m_hSocket != INVALID_SOCKET)
		{
			SOCKADDR_IN addrSrv;

			addrSrv.sin_addr.S_un.S_addr	= inet_addr(ipAddress.c_str());
			addrSrv.sin_family				= AF_INET;
			addrSrv.sin_port				= htons(nPort);

			if (0 == ::connect(m_hSocket, (SOCKADDR*)&addrSrv, sizeof(SOCKADDR)))
				return true;
		}
		
		// close the socket handle
		Close();

		return 0;
	}

	/**
	 *
	 * \param nID 
	 * \param subscriber 
	 */
	void	Client::SubscribeEvent(int nID, FunctorEvent::Subscriber subscriber)
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
	bool	Client::HasEvent(int nID)
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
	void	Client::FireEvent(int nID, PlayerEventArgs& args)
	{
		char chName[32];
		sprintf(chName, "%d", nID);

		fireEvent_impl(chName, args);
	}

	/**
	 *
	 * \return 
	 */
	void	Client::Update()
	{
		int nLength = ::recv(m_hSocket, (char*)m_pRecvBuf + m_nRecvSize, sizeof(m_pRecvBuf) - m_nRecvSize, 0);
		if (nLength != SOCKET_ERROR)
		{
			m_nRecvSize += nLength;

			while(m_nRecvSize >= HEAD_LENGTH)
			{
				// get pack head
				NetHead* pHead = (NetHead*)m_pRecvBuf;
				if (m_nRecvSize >= pHead->Length)
				{
					// copy pack data to data buffer
					CopyMemory(
						m_pDataBuf, pHead + 1, pHead->Length - HEAD_LENGTH
						);

					m_nRecvSize -= pHead->Length;

					MoveMemory(
						m_pRecvBuf, m_pRecvBuf + pHead->Length, m_nRecvSize);

					if (HasEvent(pHead->ID))
						FireEvent(pHead->ID, PlayerEventArgs(pHead->ID, m_pDataBuf, pHead->Length - HEAD_LENGTH));

					m_pDataBuf[0] = '\0';
				}
			}
		}		
	}

	/**
	 *
	 * \return 
	 */
	bool	Client::IsConnected() const
	{
		return m_hSocket != INVALID_SOCKET;
	}

	/**
	 *
	 * \return 
	 */
	String	Client::GetIPAdress() const
	{
		return m_IPAdress;
	}

	/**
	 *
	 * \return 
	 */
	int		Client::GetPort() const
	{
		return m_nPort;
	}

	/**
	 *
	 * \param nID 
	 * \return 
	 */
	int		Client::Send(int nID)
	{
		char chBuffer[HEAD_LENGTH];

		NetHead* pHead	= (NetHead*)(chBuffer);
		pHead->ID		= nID;
		pHead->Length	= HEAD_LENGTH;

		return Send(chBuffer, HEAD_LENGTH);
	}

	/**
	 *
	 * \param nID 
	 * \param jp 
	 * \return 
	 */
	int		Client::Send(int nID, JsonProtocol& jp)
	{
		int nLength = jp.MakeNetStream(nID);

#if _DEBUG
		printf("Send ID: %d text:%s Length: %d\n", nID, jp.ToString().c_str(), nLength);
#endif
		if (nLength)
			return Send(jp.GetBuffer(), nLength);

		return 0;
	}

	/**
	 *
	 * \param pData 
	 * \param nLength 
	 * \return 
	 */
	int		Client::Send(const char* pData, int nLength)
	{
		return ::send(m_hSocket, pData, nLength, 0);
	}

	/**
	 *
	 */
	void	Client::Close()
	{
		if (m_hSocket != INVALID_SOCKET)
			::closesocket(m_hSocket);

		m_hSocket = INVALID_SOCKET;
	}
}
