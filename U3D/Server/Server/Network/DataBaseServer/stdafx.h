// stdafx.h : 标准系统包含文件的包含文件，
// 或是经常使用但不常更改的
// 特定于项目的包含文件
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
