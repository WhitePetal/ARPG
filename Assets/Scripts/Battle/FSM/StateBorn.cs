/*********************************************************
	文件：StateBorn
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/9/5 17:13:47
	功能：出生状态
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBorn : IState
{
    public void Enter(EntityBase entity, StateParam param = default)
    {
        entity.curState = AnimState.Born;
        entity.SetAction(0);
        TimerSev.Instance.AddTimerTask((tid) =>
        {
            entity.SetAction(Constans.ActionDefault);
            entity.Idle();
        }, 2000, 1);
    }

    public void Exit(EntityBase entity, StateParam param = default)
    {
        
    }

    public void Process(EntityBase entity, StateParam param = default)
    {
 
    }
}
