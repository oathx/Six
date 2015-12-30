#ifndef _____SlotFunctorBase_h_
#define _____SlotFunctorBase_h_

namespace Server
{
	// forward refs
	class EventArgs;

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
	class SlotFunctorBase
	{
	public:
		/**
		 *
		 * \return 
		 */
		virtual ~SlotFunctorBase() {};

		/**
		 *
		 * \return 
		 */
		virtual bool operator()(const EventArgs& args) = 0;
	};
}

#endif
