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
#define MAX_LINE			1024
#endif

#ifndef IDP_LOGIN_EVENT
#define IDP_LOGIN_EVENT		0
#endif

#define IDI_CENTER			0
#define IDI_DATABASE		1

#define IDT_CLEINT_TIEMER	33

#include "NetProtocol.h"