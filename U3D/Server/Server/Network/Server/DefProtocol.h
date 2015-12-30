#ifndef _____DefProtocol_H
#define _____DefProtocol_H

#include "JsonProtocol.h"

namespace Server
{	
#define CS_NET_HEART		1000

	// login cmd
#define CS_NET_LOGIN		1001
#define SC_NET_LOGIN		1002

	/**
	* \ingroup : Server
	*
	* \os&IDE  : Microsoft Windows XP (SP3)  &  Microsoft Visual C++ .NET 2008 & ogre1.8
	*
	* \VERSION : 1.0
	*
	* \@date   : 2014-05-09
	*
	* \Author  : 
	*
	* \Desc    : 
	*
	* \bug     : 
	*
	*/
	class CSNetLogin
	{
	public:
		CSNetLogin(const String& text)
		{
			Json::Value		vr;
			Json::Reader	rd;
			if (rd.parse(text, vr, true))
			{
				UserName = vr["UserName"].asString();
				Password = vr["Password"].asString();
			}
		}

	public:
		String		UserName;
		String		Password;
	};
	
	/*C.Net[
		public const int	CS_NET_LOGIN	= 1001;
		public class CSNetLogin
		{
			public string	UserName
			{get; set;}

			public string	Password
			{get; set;}
		}
	]**/

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
	class SCNetLogin : public JsonProtocol
	{
	public:
		SCNetLogin(int nID, const String& szName, int nLev, int nMoney, int nGold, int nExp)
		{
			m_vProperty["ID"]		= Json::Value(nID);
			m_vProperty["Name"]		= Json::Value(szName);
			m_vProperty["Lev"]		= Json::Value(nLev);
			m_vProperty["Money"]	= Json::Value(nMoney);
			m_vProperty["Gold"]		= Json::Value(nGold);
			m_vProperty["Exp"]		= Json::Value(nExp);
		}
	};

	/*C.Net[
		public const int	SC_NET_LOGIN	= 1002;
		public class CSNetLogin
		{
			public int		ID
			{get; set;}

			public string	Name
			{get; set;}

			public int		Lev
			{get; set;}

			public int		Money
			{get; set;}

			public int		Gold
			{get; set;}

			public int		Exp
			{get; set;}
		}
	]**/
}
#endif