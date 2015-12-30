#ifndef _____EventArgs_h_
#define _____EventArgs_h_

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
	class EventArgs
	{
	public:
		/**
		 *
		 * \param void 
		 * \return 
		 */
		EventArgs(void);

		/**
		 *
		 * \param void 
		 * \return 
		 */
		virtual ~EventArgs(void);

		//! handlers should increment this if they handled the event.
		unsigned int	handled;
	};

}

#endif
