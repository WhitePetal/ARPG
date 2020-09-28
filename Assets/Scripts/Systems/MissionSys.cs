/*********************************************************
	文件：MissionSys
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/8/29 11:02:11
	功能：副本系统
***********************************************************/
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionSys : SystemBase<MissionSys>
{
    private MissionWindow missionWindow;

    public override void InitSys()
    {
        base.InitSys();
        missionWindow = GetWindow<MissionWindow>("FubenWindow");
    }

    public void EnterMission()
    {
        OpenMissionWindow();
    }

    private void OpenMissionWindow()
    {
        missionWindow.SetWindowState(true);
    }

    public void RspMissionFight(GameMsg msg)
    {
        GameRoot.Instance.SetPlayerDataByMissionFight(msg.rspMissionFight);
        MainCitySys.Instance.CloseMainCityWindow();
        missionWindow.SetWindowState(false);
        BattleSys.Instance.StartBattle(msg.rspMissionFight.missionId);
    }
}
