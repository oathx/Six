#include "FunctorEvent.h"
#include "EventArgs.h"
#include "BoundSlot.h"

#include <algorithm>

namespace Server
{
/*!
\brief
    Implementation helper functor which is used to locate a BoundSlot in the
    multimap collection of BoundSlots.
*/
class SubComp
{
public:
    SubComp(const BoundSlot& s) :
        d_s(s)
    {}

    bool operator()(std::pair<FunctorEvent::Group, FunctorEvent::Connection> e) const
    {
        return *(e.second) == d_s;
    }

private:
    void operator=(const SubComp&) {}
    const BoundSlot& d_s;
};



FunctorEvent::FunctorEvent(const String& name) :
    d_name(name)
{
}


FunctorEvent::~FunctorEvent()
{
    SlotContainer::iterator iter(d_slots.begin());
    const SlotContainer::const_iterator end_iter(d_slots.end());

    for (; iter != end_iter; ++iter)
    {
        iter->second->m_event = 0;
        iter->second->m_subscriber->cleanup();
    }

    d_slots.clear();
}


FunctorEvent::Connection FunctorEvent::subscribe(const FunctorEvent::Subscriber& slot)
{
    return subscribe(static_cast<Group>(-1), slot);
}


FunctorEvent::Connection FunctorEvent::subscribe(FunctorEvent::Group group, const FunctorEvent::Subscriber& slot)
{
    FunctorEvent::Connection c(new BoundSlot(group, slot, *this));
    d_slots.insert(std::pair<Group, Connection>(group, c));
    return c;
}


void FunctorEvent::operator()(EventArgs& args)
{
    SlotContainer::iterator iter(d_slots.begin());
    const SlotContainer::const_iterator end_iter(d_slots.end());

    // execute all subscribers, updating the 'handled' state as we go
    for (; iter != end_iter; ++iter)
        if ((*iter->second->m_subscriber)(args))
            ++args.handled;
}


void FunctorEvent::unsubscribe(const BoundSlot& slot)
{
    // try to find the slot in our collection
    SlotContainer::iterator curr =
        std::find_if(d_slots.begin(),
                     d_slots.end(),
                     SubComp(slot));

    // erase our reference to the slot, if we had one.
    if (curr != d_slots.end())
        d_slots.erase(curr);
}

} // End of  Ogre namespace section

