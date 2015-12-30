#include "stdafx.h"
#include "PlayerManager.h"
#include "LoginEvent.h"
#include "ScriptManager.h"
#include "ClientManager.h"
#include "DataBaseClient.h"

// server event base
struct event_base*	eb;
// accept event
struct event		ev;

// time loop
struct event		clientloop;
struct timeval		tv;
struct timeval		clienttime;

// import server namespace
using namespace		Server;

/**
 *
 * \param *bev 
 * \param *arg 
 */
void on_event_buffer_read(struct bufferevent* bev, void* args)
{ 
	Player* player		= (Player*)(args);

	// get current socket 
	evutil_socket_t fd	= bufferevent_getfd(bev);  
	if (fd == INVALID_SOCKET)
	{
		// free buffer event
		bufferevent_free(bev);
	
		// destroy the error player
		PlayerManager::GetSingleton()->DestroyPlayer(player->GetSocket());		
	}
	else
	{
		char chMemory[MAX_LINE];

		// read socket
		int	nLength = bufferevent_read(bev, chMemory, MAX_LINE);
		if (nLength > 0)
		{
			if (player != NULL && nLength > 0)
				player->Update(chMemory, nLength);
		}
	}
}

/**
 *
 * \param *bev 
 * \param *arg 
 */
void on_evnet_buffer_write(struct bufferevent* bev, void* args)
{
	
}

/**
 *
 * \param *bev 
 * \param what 
 * \param *arg 
 */
void on_evnet_buffer_error(struct bufferevent* bev, short what, void* args)
{
	Player* player = (Player*)(args);
	if (what & EVBUFFER_EOF) 
	{
		printf("socket %d client disconnected.\n", player->GetSocket());
	}
	else 
	{
		printf("client socket error, disconnecting.\n");
	}

	bufferevent* pBufferEvent = player->GetBufferEvent();
	if (pBufferEvent)
	{
		// free buffer event
		bufferevent_free(pBufferEvent);
	}

	// destroy the error player
	PlayerManager::GetSingleton()->DestroyPlayer(player->GetSocket());

#ifdef _DEBUG
	printf("current connect count %d \n", PlayerManager::GetSingleton()->GetCount());
#endif
}

/**
 *
 * \param sock 
 * \param event 
 * \param arg 
 */
void on_event_accept(int nSocket, short evt, void* arg)
{
	sockaddr_in addr;
	int			len	= sizeof(sockaddr_in);

	SOCKET hSocket	= accept(nSocket, (sockaddr*)&addr, &len);
	if (hSocket != INVALID_SOCKET)
	{
		printf("accept socket %d \n", hSocket);

		u_long uMode = 1;
		if (!ioctlsocket(hSocket, FIONBIO, &uMode))
		{
			Player* player = PlayerManager::GetSingleton()->CreatePlayer(hSocket);
			if (player)
			{
				bufferevent* pBufferEvent = bufferevent_socket_new(eb, hSocket, BEV_OPT_CLOSE_ON_FREE);
				if (pBufferEvent != NULL)
				{
					// set message callback
					bufferevent_setcb(pBufferEvent, on_event_buffer_read, on_evnet_buffer_write, on_evnet_buffer_error, player);
	
					// enable the buffer event
					bufferevent_enable(
						pBufferEvent, 
						EV_READ|EV_WRITE|EV_PERSIST
						);
					
					player->RegisterEventListener(new LoginEvent(IDP_LOGIN_EVENT));

					// set buffer event
					player->SetBufferEvent(pBufferEvent);
				}
				else
				{
					// destroy the error player
					PlayerManager::GetSingleton()->DestroyPlayer(hSocket);

					// free buffer event
					bufferevent_free(
						pBufferEvent
						);
				}

#ifdef _DEBUG
				printf("current connect count %d \n", PlayerManager::GetSingleton()->GetCount());
#endif
			}
		}
	}
}

/**
 *
 * \param fd 
 * \param event 
 * \param *arg 
 */
void on_client_loop(evutil_socket_t fd, short e, void *arg)
{
	ClientManager::GetSingleton()->Run();
}

/**
 *
 * \param fd 
 * \param e 
 * \param *arg 
 */
void on_heart_loop(evutil_socket_t fd, short e, void *arg)
{
	printf("Heart \n");
}

/**
 *
 * \param tv_usec 
 */
void add_client_timer(int tv_usec)
{
	event_assign(&clientloop, eb, -1, EV_PERSIST, on_client_loop, (void*)&clientloop);

	// set client loop frame time
	evutil_timerclear(&tv);
	tv.tv_usec = tv_usec;
	event_add(&clientloop, &tv);

	evutil_gettimeofday(&clienttime, NULL);
}

/**
 *
 * \param argc 
 * \param argv[] 
 * \return 
 */
int main(int argc, _TCHAR* argv[])
{
	WORD	myVersionRequest = MAKEWORD(1,1);
	
	WSADATA wsaData;
	int nError = WSAStartup(myVersionRequest,&wsaData);
	if (!nError)
	{
		// create player manager
		PlayerManager* pMgr = new PlayerManager();
		// server client manager
		ClientManager* pClt = new ClientManager();

		// create server socket
		SOCKET hSocket = socket(AF_INET, SOCK_STREAM, 0);
		if (hSocket != INVALID_SOCKET)
		{
			SOCKADDR_IN		addr;
			addr.sin_family				= AF_INET;
			addr.sin_addr.S_un.S_addr	= htonl(INADDR_ANY);
			addr.sin_port				= htons(9527);

			if (!bind(hSocket, (SOCKADDR*)&addr, sizeof(SOCKADDR)))
			{
				listen(hSocket, 5);

				u_long uMode = 1;
				if (!ioctlsocket(hSocket, FIONBIO, &uMode))
				{
					// create lib event
					eb = event_base_new();
					if (eb != NULL)
					{
						event_assign(&ev, eb, hSocket, EV_READ|EV_PERSIST, on_event_accept, NULL);
						event_add(&ev, NULL);
						
						DataBaseClient* pDataBase = new DataBaseClient(0, "127.0.0.1", 9526);
						if (pDataBase)
							ClientManager::GetSingleton()->RegisterServerClient(pDataBase);

						// add client server
						add_client_timer(IDT_CLEINT_TIEMER);
					
						// message loop
						event_base_dispatch(eb);
					}
				}
			}
		}
		
		event_base_free(eb);

		// destroy player manager
		if (pMgr != NULL)
		{
			delete pMgr;
			pMgr = NULL;
		}

		if (pClt != NULL)
		{
			delete pClt;
			pClt = NULL;
		}
	}

	// close windows socket
	WSACleanup();

	return 0;
}
