/*********************************************************
	文件：StateHit
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/9/5 19:45:17
	功能：受击状态
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateHit : IState
{
    public void Enter(EntityBase entity, StateParam param = default)
    {
        entity.curState = AnimState.Hit;

        foreach (int tid in entity.sklActCBSet)
        {
            TimerSev.Instance.DeleteTimeTask(tid);
        }
        foreach (int tid in entity.sklMoveCBSet)
        {
            TimerSev.Instance.DeleteTimeTask(tid);
        }
        while (entity.toIdleSkillQue.Count > 0)
        {
            int deId = entity.toIdleSkillQue.Dequeue();
            TimerSev.Instance.DeleteTimeTask(deId);
        }
        entity.sklActCBSet.Clear();
        entity.sklMoveCBSet.Clear();
    }

    public void Exit(EntityBase entity, StateParam param = default)
    {
        
    }

    public void Process(EntityBase entity, StateParam param = default)
    {
        if (entity.entityType == EntityType.Player)
        {
            entity.canSkill = false;
            entity.PlayAudio("PlayerAudio", Constans.playerHit);
        }
        entity.SetDir(Vector2.zero);
        entity.SetAction(50);

        TimerSev.Instance.AddTimerTask((tid) =>
        {
            entity.SetAction(Constans.ActionDefault);
            entity.Idle();
        }, GetHitAniLen(entity));
    }

    private float GetHitAniLen(EntityBase entity)
    {
        AnimationClip[] clips = entity.GetAnimRunClips();
        for(int i = 0; i < clips.Length; ++i)
        {
            string name = clips[i].name;
            if(name.Contains("hit") || name.Contains("Hit") || name.Contains("HIT"))
            {
                return clips[i].length * 1000f;
            }
        }
        return 1000f;
    }
}
