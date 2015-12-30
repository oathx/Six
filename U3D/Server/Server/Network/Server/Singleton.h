#ifndef _____Singleton_H
#define _____Singleton_H

#include "TypeDef.h"

namespace Server
{
	template <typename T> class Singleton
	{
	private:
		/**
			*
			* \param & 
			* \return 
			*/
		Singleton(const Singleton<T> &);

		/**
			*
			* \param & 
			* \return 
			*/
		Singleton& operator=(const Singleton<T> &);

	protected:
		// ����ʵ��
		static T* mpSingleton;

	public:
		/** ���캯��
			*
			* \param void 
			* \return 
			*/
		Singleton( void )
		{
			assert( !mpSingleton );
	#if defined( _MSC_VER ) && _MSC_VER < 1200	 
			int offset = (int)(T*)1 - (int)(Singleton <T>*)(T*)1;
			mpSingleton = (T*)((int)this + offset);
	#else
			mpSingleton = static_cast< T* >( this );
	#endif
		}
		/** ��������
			*
			* \param void 
			* \return 
			*/
		~Singleton( void )
		{  
			assert( mpSingleton );  mpSingleton = 0; 
		}

		/** ��ȡ��������
			*
			* \param void 
			* \return 
			*/
		static T* GetSingleton( void )
		{	
			assert( mpSingleton );  return ( mpSingleton ); 
		}
	};
}


#endif