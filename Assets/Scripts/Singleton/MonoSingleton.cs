/*********************************************************
	文件：MonoSingleton
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/12 16:16:46
	功能：待定
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance;

    public static T InitSingleton()
    {
        if(Instance != null)
        {
            Destroy(Instance.gameObject);
        }

        Instance = new GameObject(typeof(T).Name).AddComponent<T>();

        return Instance;
    }
}
