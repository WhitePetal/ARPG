/*********************************************************
	文件：NormalSingleton
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/12 16:16:26
	功能：普通泛型单例
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalSingleton<T> where T : new()
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new T();
            }

            return instance;
        }
    }
    
}
