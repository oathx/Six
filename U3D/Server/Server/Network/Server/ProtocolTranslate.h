#pragma once

#include "TypeDef.h"

namespace Server
{
	/**
	* \ingroup : Server
	*
	* \os&IDE  : Microsoft Windows XP (SP3)  &  Microsoft Visual C++ .NET 2008 & ogre1.8
	*
	* \VERSION : 1.0
	*
	* \@date   : 2014-05-11
	*
	* \Author  : lp
	*
	* \Desc    : 
	*
	* \bug     : 
	*
	*/
	class ProtocolTranslate
	{
	public:
		/**
		 *
		 * \param void 
		 * \return 
		 */
		ProtocolTranslate(void);
		/**
		 *
		 * \param void 
		 * \return 
		 */
		virtual ~ProtocolTranslate(void);

		/**
		 *
		 * \param source 
		 * \param target 
		 */
		virtual void		Translate(const String& source, const String& target);
	};

}
