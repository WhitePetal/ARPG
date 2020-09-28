/*********************************************************
	文件：VectorTest
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/9/4 16:26:58
	功能：待定
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorTest
{
    public static void Add(ref this Vector3 vec1, Vector3 vec2)
    {
        vec1.x += vec2.x;
        vec1.y += vec2.y;
        vec1.z += vec2.z;
    }
}
