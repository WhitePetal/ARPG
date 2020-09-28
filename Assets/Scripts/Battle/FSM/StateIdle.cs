/*********************************************************
	文件：StateIdle
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/8/30 15:58:43
	功能：待机状态
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateIdle : IState
{
    public void Enter(EntityBase entity, StateParam param = default)
    {
        
    }

    public void Exit(EntityBase entity, StateParam param = default)
    {
        
    }

    public void Process(EntityBase entity, StateParam param = default)
    {
        //entity.nextCombo = 0;
        //if(entity.nextSkillId != -1)
        //{
        //    entity.Attack(entity.nextSkillId);
        //    return;
        //}
        if (entity.entityType == EntityType.Player) entity.canSkill = true;
        Vector2 dir = entity.GetDirInput();
        if(dir != Vector2.zero)
        {
            entity.SetDir(dir);
            entity.Move();
            return;
        }
        entity.SetBlend(0f);
    }
}
