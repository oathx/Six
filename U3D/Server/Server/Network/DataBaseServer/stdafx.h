// stdafx.h : ��׼ϵͳ�����ļ��İ����ļ���
// ���Ǿ���ʹ�õ��������ĵ�
// �ض�����Ŀ�İ����ļ�
//

#pragma once

#include "targetver.h"

#include <stdio.h>
#include <tchar.h>
#include <iostream>
#include <math.h>
#include <string>
#include <WinSock2.h>
#include <event.h>
#include <map>

namespace Server
{
	typedef std::string	String;
}

#ifndef MAX_LINE
#define MAX_LINE	1024
#endif

#include "NetProtocol.h"
