/*********************************************************
	文件：XLuaSys
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/9/12 18:36:41
	功能：待定
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class XLuaSys : SystemBase<XLuaSys>
{
    private LuaEnv luaEnv;
    public override void InitSys()
    {
        base.InitSys();
        luaEnv = new LuaEnv();
        luaEnv.DoString("require 'Lua/Test'");
    }
}
