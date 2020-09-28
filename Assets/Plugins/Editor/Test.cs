/*********************************************************
	文件：Test
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/9/4 11:47:58
	功能：待定
***********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Test : Editor
{
    [MenuItem("Tools/Test")]
    static void Start()
    {
        Vector3 vec = new Vector3(1, 2, 3);
        int lastTime = DateTime.Now.Millisecond;
        for(int i = 0; i < 100000; ++i)
        {
            vec = vec * 2 * 3;
        }
        Debug.Log(DateTime.Now.Millisecond - lastTime);

        vec = new Vector3(1, 2, 3);
        lastTime = DateTime.Now.Millisecond;
        for (int i = 0; i < 100000; ++i)
        {
            vec = vec * (2 * 3);
        }
        Debug.Log(DateTime.Now.Millisecond - lastTime);
    }
}
