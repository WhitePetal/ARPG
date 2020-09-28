/*********************************************************
	文件：SystemBase
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/12 17:21:31
	功能：业务系统基类
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SystemBase<T> : MonoSingleton<T> where T : MonoBehaviour
{
    protected ResSev resSev;
    protected AudioSev audioSev;
    protected NetSev netSev;
    protected TimerSev timerSev;

    public virtual void InitSys()
    {
        DontDestroyOnLoad(this);
        resSev = ResSev.Instance;
        audioSev = AudioSev.Instance;
        netSev = NetSev.Instance;
        timerSev = TimerSev.Instance;
    }

    protected N GetWindow<N>(string pathFromCanvas) where N : WindowRoot
    {
        return GameRoot.Instance.transform.Find("Canvas/" + pathFromCanvas).GetComponent<N>();
    }
}
