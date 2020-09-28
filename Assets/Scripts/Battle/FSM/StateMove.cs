/*********************************************************
	文件：StateMove
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/8/30 16:14:33
	功能：移动状态
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMove : IState
{
    public void Enter(EntityBase entity, StateParam param = default)
    {
        
    }

    public void Exit(EntityBase entity, StateParam param = default)
    {
        
    }

    public void Process(EntityBase entity, StateParam param = default)
    {
        entity.SetBlend(1f);
    }
}
