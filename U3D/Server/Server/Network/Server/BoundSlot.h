#ifndef _____BoundSlot_h_
#define _____BoundSlot_h_

#include "SubscriberSlot.h"

namespace Server
{
	class FunctorEvent;

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
	class BoundSlot
	{
	public:
		typedef unsigned int Group;

		/**
		 *
		 * \param group 
		 * \param subscriber 
		 * \param event 
		 * \return 
		 */
		BoundSlot(Group group, const SubscriberSlot& subscriber, FunctorEvent& fevent);

		/**
		 *
		 * \param other 
		 * \return 
		 */
		BoundSlot(const BoundSlot& other);

		/**
		 *
		 * \return 
		 */
		~BoundSlot();

		/**
		 *
		 * \return 
		 */
		bool connected() const;

		/**
		 *
		 */
		void disconnect();

		/**
		 *
		 * \param other 
		 * \return 
		 */
		bool	operator==(const BoundSlot& other) const;

		/**
		 *
		 * \param other 
		 * \return 
		 */
		bool	operator!=(const BoundSlot& other) const;

	private:
		friend class FunctorEvent;

		/**
		 *
		 * \param other 
		 * \return 
		 */
		BoundSlot& operator=(const BoundSlot& other);

		Group			m_group;                //! The group the slot subscription used.
		SubscriberSlot* m_subscriber;			//! The actual slot object.
		FunctorEvent*	m_event;                //! The event to which the slot was attached
	};
}

#endif
