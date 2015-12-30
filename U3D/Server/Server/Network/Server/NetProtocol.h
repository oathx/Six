#ifndef _____NetProtocol_H
#define _____NetProtocol_H

#include "TypeDef.h"

#ifndef MAX_BUFFERLENGTH
#define MAX_BUFFERLENGTH 1024
#endif

struct NetHead
{
	int			Length;
	int			ID;
};
#define HEAD_LENGTH	sizeof(NetHead)

#endif