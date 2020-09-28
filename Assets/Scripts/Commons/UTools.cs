/*********************************************************
	文件：UTools
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/13 16:32:04
	功能：工具类
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UTools
{
    private static System.Random dRd = new System.Random();
    public static int RDInt(int min, int max, System.Random rd = null)
    {
        if(rd == null) rd = dRd;
        int val = rd.Next(min, max + 1);
        return val;
    }
}
