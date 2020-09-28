/*********************************************************
	文件：LuaTimerSevMono
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/9/21 12:26:33
	功能：待定
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

[LuaCallCSharp]
public class LuaTimerSevMono : LuaBehaviour
{
    protected override void Awake()
    {
        luaScriptPath = "Lua/Timer/LuaTimerSevMono.lua";
        base.Awake();
    }
}
