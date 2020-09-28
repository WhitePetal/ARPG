/*********************************************************
	文件：GameRoot
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/12 14:29:21
	功能：游戏启动入口
***********************************************************/
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;
using System.Reflection;
using System.Linq;

public class GameRoot : MonoBehaviour
{
    public static GameRoot Instance;

    [HideInInspector] public LodingWindow lodingWindow;
    [HideInInspector] public DynamicWindow dynamicWindow;

    private LuaEnv luaEnv;

    private void Awake()
    {
        Debug.Log("Mono Game Awake...");
    }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        Debug.Log("Mono Game Start...");

        luaEnv = new LuaEnv();

        List<Type> tl = (from type in Assembly.GetExecutingAssembly().GetTypes()
         where type.Namespace == "UnityEngine"
         select type).ToList();
        //Debug.Log(Assembly.GetAssembly(typeof(MonoBehaviour)));
        foreach (var type in Assembly.GetAssembly(typeof(MonoBehaviour)).GetTypes())
        {
            Debug.Log("name: " + type.Name + "   space: " + (type.Namespace == null ? "null" : type.Namespace));
        }
        //Debug.Log("Count: " + tl.Count);
        //for(int i = 0; i < tl.Count; ++i)
        //{
        //    Debug.Log(tl[i].Name);
        //}

        InitWindow();

        ClearUIRoot();

        Init();
    }

    private void InitWindow()
    {
        lodingWindow = transform.Find("Canvas/LodingWindow").GetComponent<LodingWindow>();
        dynamicWindow = transform.Find("Canvas/DynamicWindow").GetComponent<DynamicWindow>();
    }

    private void OnDestroy()
    {

    }

    private void ClearUIRoot()
    {
        Transform canvas = transform.Find("Canvas");
        foreach(Transform child in canvas)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void Init()
    {
        // 初始化服务模块
        NetSev.Instance.InitSev();
        ResSev.Instance.InitSev();
        AudioSev.Instance.InitSev();
        TimerSev.Instance.InitSev();
        TimerSev.Instance.StartTimer();

        // 初始化业务系统
        XLuaSys.InitSingleton().InitSys();
        CoroutineSys cor = CoroutineSys.InitSingleton();
        cor.InitSys();
        LoginSys login = LoginSys.InitSingleton();
        login.InitSys();
        MainCitySys mainCity = MainCitySys.InitSingleton();
        mainCity.InitSys();
        MissionSys missionSys = MissionSys.InitSingleton();
        missionSys.InitSys();
        BattleSys battleSys = BattleSys.InitSingleton();
        battleSys.InitSys();

        dynamicWindow.SetWindowState(true);
        // 进入登录场景并加载相应UI
        login.EnterLogin();

        //AddTips("Tips1");
        //AddTips("Tips2");
    }

    public static void AddTips(string tips)
    {
        Instance.dynamicWindow.AddTips(tips);
    }

    private PlayerData playerData = null;
    public PlayerData PlayerData
    {
        get
        {
            return playerData;
        }
        private set
        {
            playerData = value;
        }
    }
    public void SetPlayerData(RspLogin data)
    {
        playerData = data.playerData;
    }

    public void SetPlayerName(string name)
    {
        playerData.name = name;
    }

    public void SetPlayerDataByGuide(RspGuide rsp)
    {
        playerData.coin = rsp.coin;
        playerData.lv = rsp.lv;
        playerData.exp = rsp.exp;
        playerData.guidid = rsp.guideid;
    }

    public void SetPlayerDataByStrong(RspStrong data)
    {
        playerData.coin = data.coin;
        playerData.crystal = data.crystal;
        playerData.hp = data.hp;
        playerData.ad = data.ad;
        playerData.ap = data.ap;
        playerData.addef = data.adddef;
        playerData.apdef = data.apdef;
        playerData.strongArr = data.strongArr;
    }

    public void SetPlayerDataByBuy(RspBuy data)
    {
        playerData.diamond = data.dimond;
        playerData.coin = data.coin;
        playerData.power = data.power;
    }

    public void SetPlayerDataPower(PshPower data)
    {
        playerData.power = data.power;
    }

    public void SetPlayerDataByTakeTask(RspTakeTaskReward data)
    {
        playerData.coin = data.coin;
        playerData.exp = data.exp;
        playerData.lv = data.lv;
        playerData.taskArr = data.taskArr;
    }
    public void SetPlayerDataTaskAr(PshTaskPrgs data)
    {
        playerData.taskArr = data.taskAr;
    }

    public void SetPlayerDataByMissionFight(RspMissionFight data)
    {
        playerData.power = data.power;
    }
}
