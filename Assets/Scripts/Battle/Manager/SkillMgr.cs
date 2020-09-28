/*********************************************************
	文件：SkillMgr
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/8/30 8:58:01
	功能：待定
***********************************************************/
using BJTimer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillMgr : MonoBehaviour
{
    private ResSev resSev;
    private TimerSev timerSev;

    public void Init()
    {
        resSev = ResSev.Instance;
        timerSev = TimerSev.Instance;
    }

    public void SkillAttack(EntityBase entity, int skillId)
    {
        AttackEffect(entity, skillId);
        AttackDamage(entity, skillId);
    }

    private void AttackEffect(EntityBase entity, int skillId)
    {
        SkillCfg skillCfg = resSev.GetSkillCfg(skillId);
        entity.curSkillCfg = skillCfg;

        if (!skillCfg.isBreak) entity.entityState = EntityState.BatiState;

        if (!skillCfg.isCollide)
        {
            Physics.IgnoreLayerCollision(9, 10);
            timerSev.AddTimerTask((tid) =>
            {
                Physics.IgnoreLayerCollision(9, 10, false);
            }, skillCfg.skillTime);
        }

        IDPack idpack = timerSev.AddTimerTask((tid) =>
        {
            entity.Idle();
        }, skillCfg.skillTime);
        entity.SetAction(skillCfg.aninAction);
        entity.SetFX(skillCfg.fx, skillCfg.skillTime);

        if(entity.entityType == EntityType.Player)
        {
            while (entity.toIdleSkillQue.Count > 0)
            {
                int deId = entity.toIdleSkillQue.Dequeue();
                timerSev.ReplaceTimeTask(deId, (tid) =>
                {
                    entity.SetAction(skillCfg.aninAction);
                }, 200, 1);
                timerSev.DeleteTimeTask(deId);
            }

            entity.toIdleSkillQue.Enqueue(idpack.id);
            if (entity.GetDirInput() == Vector2.zero)
            {
                Vector2 tarDir = entity.CalcTargetDir();
                if (tarDir != Vector2.zero) entity.SetAtkDir(tarDir, false);
            }
            else
            {
                entity.SetAtkDir(entity.GetDirInput(), true);
            }
        }

        CalcSkillMove(entity, skillCfg);
    }
    private void CalcSkillMove(EntityBase entity, SkillCfg skillCfg)
    {
        int tempDelay = 0;
        for (int i = 0; i < skillCfg.skillMove.Count; i++)
        {
            SkillMoveCfg skillMove = resSev.GetSkillMoveCfg(skillCfg.skillMove[i]);
            float speed = skillMove.moveDis / (skillMove.moveTime / 1000f);
            if (skillMove.delayTime > 0f)
            {
                IDPack pack_1 = timerSev.AddTimerTask((tid) =>
                {
                    entity.SetSkillMoveState(true, speed);
                    entity.sklMoveCBSet.Remove(tid);
                }, tempDelay + skillMove.delayTime);
                entity.sklMoveCBSet.Add(pack_1.id);
            }
            else
            {
                entity.SetSkillMoveState(true, speed);
            }

            IDPack pack_2 = timerSev.AddTimerTask((ttid) =>
            {
                entity.SetSkillMoveState(false);
                entity.sklMoveCBSet.Remove(ttid);
            }, tempDelay + skillMove.moveTime + skillMove.delayTime);
            entity.sklMoveCBSet.Add(pack_2.id);

            tempDelay += skillMove.delayTime + skillMove.moveTime;
        }
    }

    private void AttackDamage(EntityBase entity, int skillId)
    {
        SkillCfg cfg = resSev.GetSkillCfg(skillId);
        List<int> actions = cfg.skillAction;
        int tempTime = 0;
        for(int i = 0; i < actions.Count; ++i)
        {
            int j = i;
            SkillActionCfg act = resSev.GetSkillActionCfg(actions[j]);
            IDPack iDPack = timerSev.AddTimerTask((tid) =>
            {
                SkillAction(entity, cfg, act, j);
                entity.sklActCBSet.Remove(tid);
            }, tempTime + act.delayTime);
            entity.sklActCBSet.Add(iDPack.id);

            tempTime += act.delayTime;
        }
    }
    private void SkillAction(EntityBase entity, SkillCfg skillCfg, SkillActionCfg act, int actIndex)
    {
        int damage = skillCfg.damageLst[actIndex];
        int type = skillCfg.dmgType;

        if(entity.entityType == EntityType.Player) // 怪物攻击
        {
            EntityMonster[] monsters = entity.BattleMgr.GetEntityMonsters();
            for (int i = 0; i < monsters.Length; ++i)
            {
                EntityMonster monster = monsters[i];
                if (InRange(entity.GetPos(), monster.GetPos(), act.radius) &&
                    InAngle(entity.GetTransform(), monster.GetPos(), act.angle))
                {
                    CalcDamage(entity, monster, damage, type);
                }
            }
        }
        else if(entity.entityType == EntityType.Monster)
        {
            EntityBase player = entity.BattleMgr.entityPlayer;
            if (InRange(entity.GetPos(), player.GetPos(), act.radius) &&
                InAngle(entity.GetTransform(), player.GetPos(), act.angle))
            {
                CalcDamage(entity, player, damage, type);
            }
        }
    }

    private void CalcDamage(EntityBase send, EntityBase recive, int damage, int type)
    {
        int curDmg = damage;
        BattleProps sendProp = send.Props;
        BattleProps reciveProp = recive.Props;

        bool critical = false;
        if (type == 1) // Ad
        {
            // 计算闪避
            int c = resSev.GetPRD_C(reciveProp.dodge);
            int p = c * reciveProp.dodgeN;
            int cur = UTools.RDInt(1, 1000);
            if(cur <= p)
            {
                reciveProp.dodgeN = 1;
                recive.SetDodge();
                return;
            }
            else
            {
                ++reciveProp.dodgeN;
            }

            // 计算属性加成
            curDmg += sendProp.ad;
            // 计算暴击
            c = resSev.GetPRD_C(sendProp.critical);
            p = c * sendProp.criticalN;
            cur = UTools.RDInt(1, 1000);

            if(cur <= p)
            {
                sendProp.criticalN = 1;
                curDmg *= 2;
                critical = true;
            }
            else
            {
                ++sendProp.criticalN;
            }
            // 计算穿甲
            int addef = (int)((1f - sendProp.pierce * 1f / 100f) * reciveProp.addef);
            curDmg -= addef;
        }
        else if (type == 2) // AP
        {
            // 计算属性加成
            curDmg += sendProp.ap;
            // 计算魔法抗性
            curDmg -= reciveProp.apdef;
        }

        if (curDmg <= 0) return;

        if (critical) recive.SetCritical(curDmg);
        else recive.SetDamage(curDmg);

        if(recive.Hp < curDmg)
        {
            recive.Hp = 0;
            recive.Die();
            recive.BattleMgr.RemoveMonster(recive._gameObject.name);
        }
        else
        {
            recive.Hp -= curDmg;
            if(recive.entityState != EntityState.BatiState && recive.GetBreakState()) recive.Hit();
        }
    }

    private bool InRange(Vector3 from, Vector3 to, float range)
    {
        float sqrDis = Vector3.SqrMagnitude(from - to);
        return range * range >= sqrDis;
    }
    private bool InAngle(Transform tran, Vector3 target, float angle)
    {
        if (angle == 360) return true;
        Vector3 f = tran.forward;
        Vector3 to = target - tran.position;
        float a = Vector3.Angle(f, to);
        return a <= angle / 2f;
    }
}
