#ifndef _____FreeFunctionSlot_h_
#define _____FreeFunctionSlot_h_

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
	class FreeFunctionSlot : public SlotFunctorBase
	{
	public:
		typedef bool (SlotFunction)(const EventArgs&);

		/**
		 *
		 * \param func 
		 * \return 
		 */
		FreeFunctionSlot(SlotFunction* func) :
			m_count(func)
		{}

		/**
		 *
		 * \return 
		 */
		virtual bool operator()(const EventArgs& args)
		{
			return m_count(args);
		}

	private:
		SlotFunction* m_count;
	};
}

#endif
