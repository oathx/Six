#ifndef _____PlayerEvent_H
#define _____PlayerEvent_H

#include "JsonProtocol.h"
#include "EventSet.h"
#include "EventArgs.h"
#include "DefProtocol.h"

namespace Server
{
	/**
	* \ingroup : Server
	*
	* \os&IDE  : Microsoft Windows XP (SP3)  &  Microsoft Visual C++ .NET 2008 & ogre1.8
	*
	* \VERSION : 1.0
	*
	* \@date   : 2014-05-07
	*
	* \Author  : 
	*
	* \Desc    : 
	*
	* \bug     : 
	*
	*/
	class PlayerEventArgs : public EventArgs
	{
	public:
		/**
		 *
		 * \param nID 
		 * \param text 
		 * \param nLength 
		 * \return 
		 */
		PlayerEventArgs(int nID, char* pBuffer, int nLength)
			: ID(nID), Buffer(pBuffer), Length(nLength)
		{
		}

	public:
		int		ID;
		char*	Buffer;
		int		Length;
	};

	//////////////////////////////////////////////////////////////////////////
	//
	//////////////////////////////////////////////////////////////////////////
	class Session;

	/**
	* \ingroup : Server
	*
	* \os&IDE  : Microsoft Windows XP (SP3)  &  Microsoft Visual C++ .NET 2008 & ogre1.8
	*
	* \VERSION : 1.0
	*
	* \@date   : 2014-05-07
	*
	* \Author  : 
	*
	* \Desc    : 
	*
	* \bug     : 
	*
	*/
	class PlayerEvent : public EventSet
	{
	public:
		/**
		 *
		 * \return 
		 */
		PlayerEvent(int nID);
		/**
		 *
		 * \return 
		 */
		virtual ~PlayerEvent();
	
		/**
		 *
		 * \return 
		 */
		virtual int			GetID() const;
	
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
		 * \param pSession 
		 */
		virtual void		SetSession(Session* pSession);

		/**
		 *
		 * \return 
		 */
		virtual Session*	GetSession() const;

		/**
		 *
		 * \param pMemory 
		 * \param nLength 
		 * \return 
		 */
		virtual int			Send(const void* pData, int nLength);

		/**
		 *
		 * \param nID 
		 * \return 
		 */
		virtual int			Send(int nID);

		/**
		 *
		 * \param jp 
		 * \return 
		 */
		virtual int			Send(int nID, JsonProtocol& jp);
	protected:
		int					m_nID;
		Session*			m_pSession;
	};
}

#endif
