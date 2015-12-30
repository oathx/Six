
#ifndef __LUA_EXTRA_H_
#define __LUA_EXTRA_H_

#if defined(_USRDLL)
    #define LUA_EXTENSIONS_DLL     __declspec(dllexport)
#else         /* use a DLL library */
    #define LUA_EXTENSIONS_DLL
#endif

#if __cplusplus
extern "C" {
#endif

#include "lauxlib.h"

void LUA_EXTENSIONS_DLL luaL_cjson(lua_State *L);
    
#if __cplusplus
}
#endif

#endif /* __LUA_EXTRA_H_ */
