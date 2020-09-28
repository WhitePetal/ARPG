/*********************************************************
	文件：EntityPlayer
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/8/30 16:39:53
	功能：玩家逻辑实体
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityPlayer : EntityBase
{
    public EntityPlayer()
    {
        entityType = EntityType.Player;
    }

    public override Vector2 GetDirInput()
    {
        return battleMgr.GetDirInput();
    }

    public override Vector2 CalcTargetDir()
    {
        EntityMonster mtar = FindClosedTraget();
        if(mtar != null)
        {
            Vector3 target = mtar.GetPos();
            Vector3 self = GetPos();
            Vector2 dir = new Vector2(target.x - self.x, target.z - self.z);
            return dir.normalized;
        }
        return Vector2.zero;
    }
    private EntityMonster FindClosedTraget()
    {
        EntityMonster[] mar = battleMgr.GetEntityMonsters();
        if (mar == null || mar.Length == 0) return null;

        float dir = float.MaxValue;
        EntityMonster target = null;
        Vector3 self = GetPos();
        for(int i = 0; i < mar.Length; ++i)
        {
            float sqrDis = Vector3.SqrMagnitude(self - mar[i].GetPos());
            if (dir > sqrDis)
            {
                dir = sqrDis;
                target = mar[i];
            }
        }

        return target;
    }

    public override void SetDodge()
    {
        GameRoot.Instance.dynamicWindow.SetPlayerDodge();
    }

    public override void SetHp(int oldVal, int newVal)
    {
        BattleSys.Instance.SetPlayerHp(newVal);
    }
}
