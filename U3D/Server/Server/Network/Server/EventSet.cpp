#include "EventSet.h"

namespace Server
{
	/**
	 *
	 * \return 
	 */
	EventSet::EventSet() :
		m_muted(false)
	{
	}

	/**
	 *
	 * \param void 
	 * \return 
	 */
	EventSet::~EventSet(void)
	{
		removeAllEvents();
	}

	/**
	 *
	 * \param name 
	 */
	void EventSet::addEvent(const String& name)
	{
		if (!isEventPresent(name))
		{
			m_events[name] = new FunctorEvent(name);
		}
	}

	/**
	 *
	 * \param name 
	 */
	void EventSet::removeEvent(const String& name)
	{
		EventMap::iterator pos = m_events.find(name);

		if (pos != m_events.end())
		{
			delete pos->second;
			m_events.erase(pos);
		}

	}

	/**
	 *
	 * \param void 
	 */
	void EventSet::removeAllEvents(void)
	{
		EventMap::iterator pos = m_events.begin();
		EventMap::iterator end = m_events.end()	;

		for (; pos != end; ++pos)
		{
			delete pos->second;
		}

		m_events.clear();
	}

	/**
	 *
	 * \param name 
	 * \return 
	 */
	bool EventSet::isEventPresent(const String& name)
	{
		return (m_events.find(name) != m_events.end());
	}

	/**
	 *
	 * \param name 
	 * \param subscriber 
	 * \return 
	 */
	FunctorEvent::Connection EventSet::subscribeEvent(const String& name, FunctorEvent::Subscriber subscriber)
	{
		// do subscription & return connection
		return getEventObject(name, true)->subscribe(subscriber);
	}

	/**
	 *
	 * \param name 
	 * \param group 
	 * \param subscriber 
	 * \return 
	 */
	FunctorEvent::Connection EventSet::subscribeEvent(const String& name, FunctorEvent::Group group, FunctorEvent::Subscriber subscriber)
	{
		// do subscription with group & return connection
		return getEventObject(name, true)->subscribe(group, subscriber);
	}

	/**
	 *
	 * \param name 
	 * \param args 
	 * \param eventNamespace 
	 */
	void EventSet::fireEvent(const String& name, EventArgs& args, const String& eventNamespace)
	{
		// handle local event
		fireEvent_impl(name, args);
	}

	/**
	 *
	 * \param void 
	 * \return 
	 */
	bool EventSet::isMuted(void) const
	{
		return m_muted;
	}

	/**
	 *
	 * \param setting 
	 */
	void EventSet::setMutedState(bool setting)
	{
		m_muted = setting;
	}


	/**
	 *
	 * \param name 
	 * \param autoAdd 
	 * \return 
	 */
	FunctorEvent* EventSet::getEventObject(const String& name, bool autoAdd)
	{
		EventMap::iterator pos = m_events.find(name);

		// if event did not exist, add it and then find it.
		if (pos == m_events.end())
		{
			if (autoAdd)
			{
				addEvent(name);
				return m_events.find(name)->second;
			}
			else
			{
				return 0;
			}
		}

		return pos->second;
	}

	/**
	 *
	 * \param name 
	 * \param args 
	 */
	void EventSet::fireEvent_impl(const String& name, EventArgs& args)
	{
		// find event object
		FunctorEvent* ev = getEventObject(name);

		// fire the event if present and set is not muted
		if ((ev != 0) && !m_muted)
			(*ev)(args);
	}
} 
