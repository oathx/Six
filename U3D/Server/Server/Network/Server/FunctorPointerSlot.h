#ifndef _____FunctorPointerSlot_h_
#define _____FunctorPointerSlot_h_

#include "SlotFunctorBase.h"

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
	template<typename T> class FunctorPointerSlot : public SlotFunctorBase
	{
	public:
		/**
		 *
		 * \param functor 
		 * \return 
		 */
		FunctorPointerSlot(T* functor) :
			m_functor(functor)
		{}

		/**
		 *
		 * \return 
		 */
		virtual bool operator()(const EventArgs& args)
		{
			return (*m_functor)(args);
		}

	private:
		T*			m_functor;
	};
}

#endif
