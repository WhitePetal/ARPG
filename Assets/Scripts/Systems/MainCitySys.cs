/*********************************************************
	文件：MainCitySystem
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/15 15:35:22
	功能：待定
***********************************************************/
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MainCitySys : SystemBase<MainCitySys>
{
    private MainCityWindow mainCityWindow;
    private InfoWindow infoWindow;
    private GuidWindow guidWindow;
    private StrongWindow strongWindow;
    private ChatWindow chatWindow;
    private BuyWindow buyWindow;
    private TaskWindow taskWindow;

    private PlayerController playerCtrl;
    private Transform charShowCamTrans;
    private AutoGuideCfg curTaskData;
    private Transform[] npcPosTrans;

    public override void InitSys()
    {
        base.InitSys();

        NETCommon.Log("Init MainCitySys");

        mainCityWindow = GetWindow<MainCityWindow>("MainCityWindow");
        infoWindow = GetWindow<InfoWindow>("InfoWindow");
        guidWindow = GetWindow<GuidWindow>("GuidWindow");
        strongWindow = GetWindow<StrongWindow>("StrongWindow");
        chatWindow = GetWindow<ChatWindow>("ChatWindow");
        buyWindow = GetWindow<BuyWindow>("BuyWindow");
        taskWindow = GetWindow<TaskWindow>("TaskWindow");
    }

    public void EnterMainCity()
    {
        resSev.AsyncLoadScene(Constans.SceneMainCity, OnEnterMainCity);
    }
    private void OnEnterMainCity()
    {
        NETCommon.Log("Enter MainCity...");

        // 打开主城场景UI
        mainCityWindow.SetWindowState();

        // 播放主城背景音乐
        audioSev.PlayBGM(Constans.MainCityBGM);

        GameObject map = GameObject.FindGameObjectWithTag("MapRoot");
        MainCityMap cityMap = map.GetComponent<MainCityMap>();
        npcPosTrans = cityMap.NPCPosTrans;

        MapCfg mapData = resSev.GetMapCfgData(Constans.MainCityID);
        // 加载游戏主角
        LoadPlayer(mapData);
        // 设置角色展示相机
        if (charShowCamTrans != null) charShowCamTrans.gameObject.SetActive(false);
    }
    private void LoadPlayer(MapCfg mapData)
    {
        Camera.main.transform.position = mapData.mainCamPos;
        Camera.main.transform.localEulerAngles = mapData.mainCamRote;

        Vector3 pos = mapData.playerBornPos;
        Vector3 angle = mapData.playerBornRote;
        Quaternion rot = Quaternion.Euler(angle);
        GameObject player = ResSev.Instance.LoadGoPrefab(PathDefine.AssissCityPlayerPrefab, true, pos, rot);
        player.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        playerCtrl = player.GetComponent<PlayerController>();
        playerCtrl.Init();
    }

    public void SetPlayerMove(Vector2 dir)
    {
        if(dir == Vector2.zero)
        {
            playerCtrl.SetBlend(0);
        }
        else
        {
            playerCtrl.SetBlend(1);
        }
        playerCtrl.Dir = dir;
    }

    public void CloseMainCityWindow()
    {
        mainCityWindow.SetWindowState(false);
    }

    #region Info Window
    public void OpenInfoWindow()
    {
        if (charShowCamTrans == null) charShowCamTrans = GameObject.FindWithTag("charShowCam").transform;

        // 设置人物展示相机相对位置
        charShowCamTrans.position = playerCtrl.transform.position + playerCtrl.transform.forward * 3.8f + new Vector3(0, 1.2f, 0);
        charShowCamTrans.localEulerAngles = new Vector3(0f, 180f + playerCtrl.transform.localEulerAngles.y, 0f);
        charShowCamTrans.gameObject.SetActive(true);
        infoWindow.SetWindowState(true);
    }

    public void CloseInfoWindow()
    {
        playerCtrl.transform.localEulerAngles = new Vector3(0f, startRotate, 0f);
        charShowCamTrans.gameObject.SetActive(false);
        infoWindow.SetWindowState(false);
    }

    private float startRotate = 0f;
    public void SetStartRotate()
    {
        startRotate = playerCtrl.transform.localEulerAngles.y;
    }
    public void SetPlayerRotate(float rotate)
    {
        playerCtrl.transform.localEulerAngles += new Vector3(0f, rotate, 0f);
    }
    #endregion

    #region Guid Window
    public void RunTask(AutoGuideCfg cfg)
    {
        if(cfg != null)
        {
            curTaskData = cfg;
        }

        // 解析任务数据
        if(curTaskData.npcID != -1)
        {
            playerCtrl.StartNav(npcPosTrans[curTaskData.npcID].position, OpenGuideWindow);
        }
        else
        {
            OpenGuideWindow();
        }
    }
    public void StopTask()
    {
        playerCtrl.StopNav();
    }
    private void OpenGuideWindow()
    {
        guidWindow.SetWindowState(true);
    }
    public AutoGuideCfg GetCurTaskData()
    {
        return curTaskData;
    }

    public void RspGuide(GameMsg msg)
    {
        RspGuide data = msg.rspGuide;

        int addCoint = data.coin - GameRoot.Instance.PlayerData.coin;
        int addExp = data.addExp;

        GameRoot.AddTips(Constans.ColorStr("任务奖励 金币: " + addCoint + " 经验值: " + addExp, TxtColor.Blue));

        switch (curTaskData.actID)
        {
            case 0:
                // 与智者对话
                break;
            case 1:
                // 进入副本
                EnterMission();
                break;
            case 2:
                // 进入强化界面
                OpenStrongWindow();
                break;
            case 3:
                // 购买体力
                OpenBuyWindow(0);
                break;
            case 4:
                // 进入铸造界面
                OpenBuyWindow(1);
                break;
            case 5:
                // 进行世界聊天
                OpenChatWindow();
                break;
        }

        GameRoot.Instance.SetPlayerDataByGuide(data);
        mainCityWindow.RefreshUI();

        if (msg.pshTaskPrgs != null) PshTaskPrgs(msg);
    }
    #endregion

    #region Strong Window
    public void OpenStrongWindow()
    {
        strongWindow.SetWindowState(true);
    }

    public void RspStrong(GameMsg msg)
    {
        audioSev.PlayUIAudio(Constans.FBItem);
        int fight = NETCommon.GetFightByProps(GameRoot.Instance.PlayerData);
        GameRoot.Instance.SetPlayerDataByStrong(msg.rspStrong);
        int curFight = NETCommon.GetFightByProps(GameRoot.Instance.PlayerData);
        strongWindow.RefreshUI();
        mainCityWindow.RefreshUI();
        GameRoot.AddTips(Constans.ColorStr("战力提升：" + (curFight - fight), TxtColor.Blue));
        if (msg.pshTaskPrgs != null) PshTaskPrgs(msg);
    }
    #endregion

    #region Chat Window
    public void OpenChatWindow()
    {
        chatWindow.SetWindowState(true);
    }
    public void PshChat(GameMsg msg)
    {
        PshChat data = msg.pshChat;
        chatWindow.AddChatMsg(data.name, data.txt);
    }
    #endregion

    #region Buy Window
    public void OpenBuyWindow(int type)
    {
        buyWindow.SetBuyType(type);
        buyWindow.SetWindowState(true);
    }

    public void RspBuy(GameMsg msg)
    {
        RspBuy data = msg.rspBuy;
        GameRoot.Instance.SetPlayerDataByBuy(data);
        mainCityWindow.RefreshUI();
        buyWindow.SetWindowState(false);
        GameRoot.AddTips("购买成功");
        if(msg.pshTaskPrgs != null)
        {
            PshTaskPrgs(msg);
        }
    }
    #endregion

    #region Power
    public void PshPower(GameMsg msg)
    {
        GameRoot.Instance.SetPlayerDataPower(msg.pshPower);
        if(mainCityWindow.gameObject.activeSelf) mainCityWindow.RefreshUI();
    }
    #endregion

    #region Task
    public void OpenTaskWindow()
    {
        taskWindow.SetWindowState(true);
    }
    public void RspTakeReward(GameMsg msg)
    {
        RspTakeTaskReward data = msg.rspTakeTask;
        GameRoot.Instance.SetPlayerDataByTakeTask(data);
        GameRoot.AddTips(Constans.ColorStr("获取经验：" + data.exp + " 获取金币：" + data.coin, TxtColor.Green));
        taskWindow.RefreshUI();
        mainCityWindow.RefreshUI();
    }
    public void PshTaskPrgs(GameMsg msg)
    {
        PshTaskPrgs data = msg.pshTaskPrgs;
        GameRoot.Instance.SetPlayerDataTaskAr(data);
        if (taskWindow.gameObject.activeSelf) taskWindow.RefreshUI();
    }
    #endregion

    #region Mission
    public void EnterMission()
    {
        MissionSys.Instance.EnterMission();
    }
    #endregion

}
