/*********************************************************
	文件：EntityMonster
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/9/2 15:44:20
	功能：怪物逻辑实体
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMonster : EntityBase
{
    private int checkTime = 1500;

    private int atkTime = 2000;
    private int atkCurTime = 0;

    private MonsterCfg mc;

    public EntityMonster()
    {
        entityType = EntityType.Monster;
    }

    public void SetMonsterCfg(MonsterCfg mc)
    {
        this.mc = mc;
    }

    public override bool GetBreakState()
    {
        if (mc.isStop)
        {
            if (curSkillCfg != null) return curSkillCfg.isBreak;
            else return true;
        }
        return false;
    }

    public override void RegisterAILogic()
    {
        TimerSev.Instance.AddTimerTask((tid) =>
        {
            TickAILogic();
        }, checkTime - UTools.RDInt(100, 500), 0);
    }

    protected override void TickAILogic()
    {
        if (curState != AnimState.Idle && curState != AnimState.Move) return;
        Vector2 dir = CalcTargetDir();
        if (!AtkInRange())
        {
            if(dir != Vector2.zero)
            {
                SetDir(dir);
                Move();
            }
            else
            {
                Idle();
            }
        }
        else
        {
            SetDir(Vector2.zero);
            atkCurTime += checkTime;
            if(atkCurTime > atkTime)
            {
                SetAtkDir(dir);
                Attack(mc.skillId);
                atkCurTime = 0;
            }
            else
            {
                Idle();
            }
        }
    }

    public override Vector2 CalcTargetDir()
    {
        EntityBase player = battleMgr.entityPlayer;
        if (player == null || player.curState == AnimState.Die) return Vector2.zero;
        Vector3 pPos = player.GetPos();
        Vector3 self = GetPos();
        Vector2 dir = new Vector2(pPos.x - self.x, pPos.z - self.z);
        return dir.normalized;
    }

    private bool AtkInRange()
    {
        EntityBase player = battleMgr.entityPlayer;
        if (player == null || player.curState == AnimState.Die) return false;
        Vector3 ppos = player.GetPos();
        Vector3 spos = GetPos();
        Vector2 dir = new Vector2(ppos.x - spos.x, ppos.z - spos.z);
        float sqrDis = Vector2.SqrMagnitude(dir);
        if (sqrDis < mc.atkDis * mc.atkDis) return true;
        return false;
    }
}
