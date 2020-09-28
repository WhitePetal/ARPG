/*********************************************************
	文件：ServiceBase
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/13 9:16:20
	功能：待定
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ServiceBase<T> : NormalSingleton<T> where T : new()
{
    public abstract void InitSev();
}
