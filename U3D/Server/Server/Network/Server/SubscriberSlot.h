#ifndef _____SubscriberSlot_h_
#define _____SubscriberSlot_h_

#include "FreeFunctionSlot.h"
#include "FunctorCopySlot.h"
#include "FunctorReferenceSlot.h"
#include "FunctorPointerSlot.h"
#include "MemberFunctionSlot.h"
#include "FunctorReferenceBinder.h"

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
	class SubscriberSlot
	{
	public:
	
		/**
		 *
		 * \return 
		 */
		SubscriberSlot();

		/**
		 *
		 * \param func 
		 * \return 
		 */
		SubscriberSlot(FreeFunctionSlot::SlotFunction* func);

		/**
		 *
		 * \return 
		 */
		~SubscriberSlot();

		/**
		 *
		 * \return 
		 */
		bool	operator()(const EventArgs& args) const
		{
			return (*m_functor_impl)(args);
		}

		/**
		 *
		 * \return 
		 */
		bool	connected() const
		{
			return m_functor_impl != 0;
		}

		/**
		 *
		 */
		void	cleanup();

		// templatised constructors
		/*!
		\brief
			Creates a SubscriberSlot that is bound to a member function.
		*/
		template<typename T>
		SubscriberSlot(bool (T::*function)(const EventArgs&), T* obj) :
			m_functor_impl(new MemberFunctionSlot<T>(function, obj))
		{}

		/*!
		\brief
			Creates a SubscriberSlot that is bound to a functor object reference.
		*/
		template<typename T>
		SubscriberSlot(const FunctorReferenceBinder<T>& binder) :
			m_functor_impl(new FunctorReferenceSlot<T>(binder.d_functor))
		{}

		/*!
		\brief
			Creates a SubscriberSlot that is bound to a copy of a functor object.
		*/
		template<typename T>
		SubscriberSlot(const T& functor) :
			m_functor_impl(new FunctorCopySlot<T>(functor))
		{}

		/*!
		\brief
			Creates a SubscriberSlot that is bound to a functor pointer.
		*/
		template<typename T>
		SubscriberSlot(T* functor) :
			m_functor_impl(new FunctorPointerSlot<T>(functor))
		{}

	private:
		//! Points to the internal functor object to which we are bound
		SlotFunctorBase* m_functor_impl;
	};

}

#endif
