#ifndef _____EventSet_h_
#define _____EventSet_h_

#include "FunctorEvent.h"

#if defined (_MSC_VER)
#	pragma warning(push)
#	pragma warning(disable : 4251)
#endif

namespace Server
{
	/*!
	\brief
		Class that collects together a set of FunctorEvent objects.

		The EventSet is a means for code to attach a handler function to some
		named event, and later, for that event to be fired and the subscribed
		handler(s) called.
		\par
		As of 0.5, the EventSet no longer needs to be filled with available events.
		Events are now added to the set as they are first used; that is, the first
		time a handler is subscribed to an event for a given EventSet, an FunctorEvent
		object is created and added to the EventSet.
		\par
		Instead of throwing an exception when firing an event that does not actually
		exist in the set, we now do nothing (if the FunctorEvent does not exist, then it
		has no handlers subscribed, and therefore doing nothing is the correct
		course action).
	*/
	class EventSet
	{
	public:
		/*!
		\brief
			Constructor for EventSet objects
		*/
		EventSet();


		/*!
		\brief
			Destructor for EventSet objects
		*/
		virtual ~EventSet(void);


		/*!
		\brief
			Add a new FunctorEvent to the EventSet with the given name.

		\param name
			String object containing the name to give the new FunctorEvent.  The name must be unique for the EventSet.

		\return
			Nothing

		\exception AlreadyExistsException	Thrown if an FunctorEvent already exists named \a name.
		*/
		void	addEvent(const String& name);


		/*!
		\brief
			Removes the FunctorEvent with the given name.  All connections to the event are disconnected.

		\param name
			String object containing the name of the FunctorEvent to remove.  If no such FunctorEvent exists, nothing happens.

		\return
			Nothing.
		*/
		void	removeEvent(const String& name);


		/*!
		\brief
			Remove all FunctorEvent objects from the EventSet

		\return
			Nothing
		*/
		void	removeAllEvents(void);


		/*!
		\brief
			Checks to see if an FunctorEvent with the given name is present in the EventSet.

		\return
			true if an FunctorEvent named \a name was found, or false if the FunctorEvent was not found
		*/
		bool	isEventPresent(const String& name);


		/*!
		\brief
			Subscribes a handler to the named FunctorEvent.  If the named FunctorEvent is not yet
			present in the EventSet, it is created and added.

		\param name
			String object containing the name of the FunctorEvent to subscribe to.

		\param subscriber
			Function or object that is to be subscribed to the FunctorEvent.

		\return
			Connection object that can be used to check the status of the FunctorEvent
			connection and to disconnect (unsubscribe) from the FunctorEvent.
		*/
		virtual FunctorEvent::Connection subscribeEvent(const String& name, FunctorEvent::Subscriber subscriber);


		/*!
		\brief
			Subscribes a handler to the specified group of the named FunctorEvent.  If the
			named FunctorEvent is not yet present in the EventSet, it is created and added.

		\param name
			String object containing the name of the FunctorEvent to subscribe to.

		\param group
			Group which is to be subscribed to.  Subscription groups are called in
			ascending order.

		\param subscriber
			Function or object that is to be subscribed to the FunctorEvent.

		\return
			Connection object that can be used to check the status of the FunctorEvent
			connection and to disconnect (unsubscribe) from the FunctorEvent.
		*/
		virtual FunctorEvent::Connection subscribeEvent(const String& name, FunctorEvent::Group group, FunctorEvent::Subscriber subscriber);


		/*!
		\brief
			Subscribes the named FunctorEvent to a scripted funtion

		\param name
			String object containing the name of the FunctorEvent to subscribe to.

		\param subscriber_name
			String object containing the name of the script funtion that is to be
			subscribed to the FunctorEvent.

		\return
			Connection object that can be used to check the status of the FunctorEvent
			connection and to disconnect (unsubscribe) from the FunctorEvent.
		*/
	   // virtual FunctorEvent::Connection subscribeScriptedEvent(const String& name, const String& subscriber_name);


		/*!
		\brief
			Subscribes the specified group of the named FunctorEvent to a scripted funtion.

		\param name
			String object containing the name of the FunctorEvent to subscribe to.

		\param group
			Group which is to be subscribed to.  Subscription groups are called in
			ascending order.

		\param subscriber_name
			String object containing the name of the script funtion that is to be
			subscribed to the FunctorEvent.

		\return
			Connection object that can be used to check the status of the FunctorEvent
			connection and to disconnect (unsubscribe) from the FunctorEvent.
		*/
		//virtual FunctorEvent::Connection subscribeScriptedEvent(const String& name, FunctorEvent::Group group, const String& subscriber_name);


		/*!
		\brief
			Fires the named event passing the given EventArgs object.

		\param name
			String object holding the name of the FunctorEvent that is to be fired
			(triggered)

		\param args
			The EventArgs (or derived) object that is to be bassed to each
			subscriber of the FunctorEvent.  Once all subscribers
			have been called the 'handled' field of the event is updated
			appropriately.

		\param eventNamespace
			String object describing the global event namespace prefix for this
			event.

		\return
			Nothing.
		*/
		virtual void fireEvent(const String& name, EventArgs& args, const String& eventNamespace = "");


		/*!
		\brief
			Return whether the EventSet is muted or not.

		\return
			- true if the EventSet is muted.  All requests to fire events will be ignored.
			- false if the EventSet is not muted.  All requests to fire events are processed as normal.
		*/
		bool	isMuted(void) const;


		/*!
		\brief
			Set the mute state for this EventSet.

		\param setting
			- true if the EventSet is to be muted (no further event firing requests will be honoured until EventSet is unmuted).
			- false if the EventSet is not to be muted and all events should fired as requested.

		\return
			Nothing.
		*/
		void	setMutedState(bool setting);


	protected:
		/*!
		\brief
			Return a pointer to the FunctorEvent object with the given name, optionally
			adding such an FunctorEvent object to the EventSet if it does not already
			exist.

		\param name
			String object holding the name of the FunctorEvent to return.

		\param autoAdd
			- true if an FunctorEvent object named \a name should be added to the set
			  if such an FunctorEvent does not currently exist.
			- false if no object should automatically be added to the set.  In this
			  case, if the FunctorEvent does not already exist 0 will be returned.

		\return
			Pointer to the FunctorEvent object in this EventSet with the specifed name.
			Or 0 if such an FunctorEvent does not exist and \a autoAdd was false.
		*/
		FunctorEvent* getEventObject(const String& name, bool autoAdd = false);

		/*!
		\brief
			Implementation event firing member
		*/
		void fireEvent_impl(const String& name, EventArgs& args);

		// Do not allow copying, assignment, or any other usage than simple creation.
		EventSet(EventSet&) {}
		EventSet& operator=(EventSet&) {return *this;}

		typedef std::map<String, FunctorEvent*>	EventMap;
		EventMap	m_events;

		bool		m_muted;	//!< true if events for this EventSet have been muted.
	};

} // End of  Ogre namespace section


#if defined(_MSC_VER)
#	pragma warning(pop)
#endif

#endif	// end of guard _CEGUIEventSet_h_
