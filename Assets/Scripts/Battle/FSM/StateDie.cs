/*********************************************************
	文件：StateDie
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/9/5 17:42:31
	功能：死亡状态
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateDie : IState
{
    public void Enter(EntityBase entity, StateParam param = default)
    {
        entity.curState = AnimState.Die;
    }

    public void Exit(EntityBase entity, StateParam param = default)
    {
        
    }

    public void Process(EntityBase entity, StateParam param = default)
    {
        entity.SetAction(100);
        entity.ReleaseControl();
        TimerSev.Instance.AddTimerTask((tid) =>
        {
            entity._gameObject.SetActive(false);
        }, Constans.DieDestoryTime);
    }
}
