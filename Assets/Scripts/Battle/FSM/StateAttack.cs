/*********************************************************
	文件：StateAttack
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/8/31 9:32:39
	功能：待定
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateAttack : IState
{
    public void Enter(EntityBase entity, StateParam param = default)
    {
        entity.SetDir(Vector2.zero);
        entity.ReleaseControl();
    }

    public void Exit(EntityBase entity, StateParam param = default)
    {
        //if (ResSev.Instance.GetSkillCfg(entity.curSkillId).isCombo)
        //{
        //    if (entity.nextCombo != -1 /*entity.comboQue.Count > 0*/)
        //    {
        //        entity.nextSkillId = entity.nextCombo; /*entity.comboQue.Dequeue();*/
        //    }
        //    else entity.nextSkillId = -1;
        //}

        if (entity.entityState == EntityState.BatiState) entity.entityState = EntityState.None;
        entity.curSkillCfg = null;
        entity.nextCombo = 0;
        entity.HandleControl();
        entity.SetAction(Constans.ActionDefault);
        entity.SetSkillMoveState(false);
    }

    public void Process(EntityBase entity, StateParam param = default)
    {
        if (entity.entityType == EntityType.Player && !param._bool) entity.canSkill = false;
        entity.SkillAttack(param._int);
    }
}
