/*********************************************************
	文件：BattleSys
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/8/30 8:39:22
	功能：战斗系统
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSys : SystemBase<BattleSys>
{
    private PlayerCtrlWindow playerCtrlWindow;
    public  BattleMgr battleMgr;

    private void Awake()
    {
        playerCtrlWindow = GetWindow<PlayerCtrlWindow>("PlayerCtrlWindow");
    }

    public override void InitSys()
    {
        base.InitSys();
    }

    public void StartBattle(int mapId)
    {
        GameObject battleRoot = new GameObject("BattleRoot");
        battleRoot.transform.SetParent(GameRoot.Instance.transform);
        battleMgr = battleRoot.AddComponent<BattleMgr>();
        battleMgr.Init(mapId);
        SetPlayerCtrlWindowState(true);
    }

    public Vector2 GetDirInput()
    {
        return playerCtrlWindow.curDir;
    }

    public void SetPlayerCtrlWindowState(bool isActive)
    {
        playerCtrlWindow.SetWindowState(isActive);
    }

    public void SetMoveDir(Vector2 dir)
    {
        battleMgr.SetMoveDir(dir);
    }
    
    public void ReleaseSkill(int skill)
    {
        battleMgr.ReleaseSkill(skill);
    }

    public void SetPlayerHp(int hp)
    {
        playerCtrlWindow.SetPlayerHp(hp);
    }
}
