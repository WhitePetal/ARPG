/*********************************************************
	文件：StateMgr
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/8/30 8:56:43
	功能：状态管理器
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMgr : MonoBehaviour
{
    private Dictionary<AnimState, IState> fsm = new Dictionary<AnimState, IState>();

    public void Init()
    {
        fsm.Add(AnimState.Idle, new StateIdle());
        fsm.Add(AnimState.Move, new StateMove());
        fsm.Add(AnimState.Attack, new StateAttack());
        fsm.Add(AnimState.Born, new StateBorn());
        fsm.Add(AnimState.Die, new StateDie());
        fsm.Add(AnimState.Hit, new StateHit());
    }

    public void ChangeState(EntityBase entity, AnimState target, StateParam param = default)
    {
        if (entity.curState == target)
        {
            fsm[target].Process(entity, param);
            return;
        }

        if (fsm.ContainsKey(target))
        {
            if(fsm.ContainsKey(entity.curState)) fsm[entity.curState].Exit(entity, param);
            fsm[target].Enter(entity, param);
            entity.curState = target;
            fsm[target].Process(entity, param);
        }
    }
}
