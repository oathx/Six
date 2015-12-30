#ifndef _____JsonProtocol_H
#define _____JsonProtocol_H

#include "TypeDef.h"
#include "NetProtocol.h"
#include "json.h"


namespace Server
{
	/**
	* \ingroup : Server
	*
	* \os&IDE  : Microsoft Windows XP (SP3)  &  Microsoft Visual C++ .NET 2008 & ogre1.8
	*
	* \VERSION : 1.0
	*
	* \@date   : 2014-05-08
	*
	* \Author  : 
	*
	* \Desc    : 
	*
	* \bug     : 
	*
	*/
	class JsonProtocol
	{
	public:
		/**
		 *
		 * \param nID 
		 * \return 
		 */
		JsonProtocol()
			: m_pBuffer(NULL), m_nLength(0)
		{
			
		}

		/**
		 *
		 * \param void 
		 * \return 
		 */
		virtual ~JsonProtocol(void)
		{
			if (m_pBuffer != NULL)
			{
				delete[] m_pBuffer;
				m_pBuffer = NULL;
			}
		}

		/**
		 *
		 * \return 
		 */
		virtual String	ToString()
		{
			Json::FastWriter fw;
			return fw.write(m_vProperty);
		}

		/**
		 *
		 * \param nID 
		 * \param pStream 
		 * \return 
		 */
		virtual int MakeNetStream(int nID)
		{
			String text = ToString();

			// calc package length
			int nLength		= HEAD_LENGTH + text.length();
			// alloc net buffer
			m_pBuffer	= new char[nLength];
			NetHead* nh = (NetHead*)(m_pBuffer);
			nh->ID		= nID;
			nh->Length	= nLength;

			if (!text.empty())
			{
				char* pData = (char*)(nh + 1);
				memcpy(pData, text.c_str(), text.length());
			}

			return nLength;
		}

		/**
		 *
		 * \return 
		 */
		virtual char*	GetBuffer() const
		{
			return m_pBuffer;
		}

	protected:
		Json::Value		m_vProperty;
		char*			m_pBuffer;
		int				m_nLength;
	};
}

#endif
