/*********************************************************
	文件：MissionWindow
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/8/29 10:53:43
	功能：副本选择界面
***********************************************************/
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionWindow : WindowRoot
{
    private PlayerData playerData;

    private Transform pointerTrans;
    private Button[] btnMissions;

    private void Awake()
    {
        FindComponent<Button>("TopRight/btnClose").onClick.AddListener(ClickCloseBtn);
        pointerTrans = FindComponent<Transform>("Center/Pointer");
        btnMissions = FindComponent<Transform>("Center").GetComponentsInChildren<Button>();
        for(int i = 0; i < btnMissions.Length; i++)
        {
            int j = i;
            btnMissions[i].onClick.AddListener(() =>
            {
                ClickMissionBtn(10001 + j);
            });
        }
    }

    protected override void InitWindow()
    {
        base.InitWindow();
        playerData = GameRoot.Instance.PlayerData;
        RefreshUI();
    }

    private void RefreshUI()
    {
        int mission = playerData.mission % 10000;
        for(int i = 0; i < btnMissions.Length; i++)
        {
            if (i < mission)
            {
                SetActive(btnMissions[i], true);
                if (i == mission - 1)
                {
                    pointerTrans.SetParent(btnMissions[i].transform);
                    pointerTrans.localPosition = new Vector3(36f, 130f, 0f);
                }
            }
            else SetActive(btnMissions[i], false);
        }
    }

    private void ClickCloseBtn()
    {
        audioSev.PlayUIAudio(Constans.UIClickBtnAudio);
        SetWindowState(false);
    }

    private void ClickMissionBtn(int id)
    {
        audioSev.PlayUIAudio(Constans.UIClickBtnAudio);
        int power = resSev.GetMapCfgData(id).power;
        if(power > playerData.power)
        {
            GameRoot.AddTips("体力不足！");
            return;
        }
        netSev.SendMsg(new GameMsg
        {
            cmd = (int)CMD.ReqMissionFight,
            reqMissionFight = new ReqMissionFight
            {
                missionId = id
            }
        });
    }
}
