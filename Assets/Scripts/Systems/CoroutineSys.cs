/*********************************************************
	文件：CoroutineSys
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/13 9:31:54
	功能：协程系统
***********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineSys : SystemBase<CoroutineSys>
{
    public override void InitSys()
    {
        base.InitSys();
    }

    public void MStartCoroutine(IEnumerator enumerator)
    {
        StartCoroutine(Coroutine(enumerator));
    }

    IEnumerator Coroutine(IEnumerator enumerator)
    {
        while (enumerator.MoveNext())
        {
            yield return enumerator.Current;
        }
    }

    IEnumerator MIEnumerator()
    {
        yield return 1;

        yield return 2;

        yield return 3;

        yield return 4;
    }

}
