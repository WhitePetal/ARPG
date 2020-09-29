using System;
using System.Xml.Serialization;
using PENet;

// 消息协议
namespace Protocol
{
    [Serializable]
    public class GameMsg : PEMsg
    {
        public ReqLogin reqLogin;
        public RspLogin rspLogin;
        public ReqRename reqRename;
        public RspRename rspRename;

        public ReqGuide reqGuide;
        public RspGuide rspGuide;

        public ReqStrong reqStrong;
        public RspStrong rspStrong;

        public SndChat sndChat;
        public PshChat pshChat;

        public ReqBuy reqBuy;
        public RspBuy rspBuy;

        public PshPower pshPower;

        public ReqTakeTaskReward reqTakeTask;
        public RspTakeTaskReward rspTakeTask;
        public PshTaskPrgs pshTaskPrgs;

        public ReqMissionFight reqMissionFight;
        public RspMissionFight rspMissionFight;
    }

    [Serializable]
    public class PlayerData
    {
        public int id;
        public string name;
        public int lv;
        public int exp;
        public int power;
        public int coin;
        public int diamond;
        public int crystal;

        public int hp;
        public int ad;
        public int ap;
        public int addef;
        public int apdef;
        public int dodge;
        public int pierce;
        public int critical;

        public int guidid;

        public int[] strongArr;

        public string[] taskArr;

        public int mission;

        public long time;
        // TOADD
    }

    #region 登录相关
    [Serializable]
    public class ReqLogin
    {
        public string account;
        public string pass;
    }

    [Serializable]
    public class RspLogin
    {
        public PlayerData playerData;
    }

    [Serializable]
    public class ReqRename
    {
        public string name;
    }
    [Serializable]
    public class RspRename
    {
        public string name;

        public RspRename(string name)
        {
            this.name = name;
        }
    }
    #endregion

    #region Guid
    [Serializable]
    public class ReqGuide
    {
        public int guidID;
    }
    [Serializable]
    public class RspGuide
    {
        public int guideid;
        public int coin;
        public int lv;
        public int exp;
        public int addExp;
    }

    #endregion

    #region Strong
    [Serializable]
    public class ReqStrong
    {
        public int pos;
    }
    [Serializable]
    public class RspStrong
    {
        public int coin;
        public int crystal;
        public int hp;
        public int ad;
        public int ap;
        public int adddef;
        public int apdef;
        public int[] strongArr;
    }
    #endregion

    #region Chat
    [Serializable]
    public class SndChat
    {
        public string txt;
    }

    [Serializable]
    public class PshChat
    {
        public string name;
        public string txt;
    }
    #endregion

    #region Buy
    [Serializable]
    public class ReqBuy
    {
        public int type;
        public int cost;
    }

    [Serializable]
    public class RspBuy
    {
        public int type;
        public int dimond;
        public int coin;
        public int power;
    }

    #endregion

    #region Power
    [Serializable]
    public class PshPower
    {
        public int power;
    }
    #endregion

    #region Task
    [Serializable]
    public class ReqTakeTaskReward
    {
        public int id;
    }
    [Serializable]
    public class RspTakeTaskReward
    {
        public int coin;
        public int lv;
        public int exp;
        public string[] taskArr;
    }
    [Serializable]
    public class PshTaskPrgs
    {
        public string[] taskAr;
    }
    #endregion

    #region Mission
    [Serializable]
    public class ReqMissionFight
    {
        public int missionId;
    }
    [Serializable]
    public class RspMissionFight
    {
        public int missionId;
        public int power;
    }
    #endregion

    public enum CMD
    {
        None = 0,
        // 登录相关
        ReqLogin = 101,
        RspLogin = 102,
        ReqRename = 103,
        RspRename = 104,

        // 主城相关
        ReqGuided = 301,
        RspGuided = 302,

        // 强化相关
        ReqStrong = 401,
        RspStrong = 402,

        // 聊天相关
        SndChat = 501,
        PshChat = 502,

        // 交易相关
        ReqBuy = 601,
        RspBuy = 602,

        PshPower = 701,

        ReqTakeTaskReward = 801,
        RspTakeTaskReward,
        PshTaskPrgs,

        ReqMissionFight = 901,
        RspMissionFight
    }

    public enum ErrorCode
    {
        None = 0, // 没有错误

        AccountIsOnline = 100, // 账号已经上线
        WrongPass = 101,
        NameIsExit = 102, // 名字已经存在

        UpdateDBError = 201,
        GetCacheError = 202,
        ServerDataError = 203,
        CliendDataError,

        LackLevel = 301,
        LackCoin,
        LackCrystal,
        LackDiamond,
        LackPower,

        TakeTaskError = 401
    }

    public class SerCfg
    {
        public const string serIP = "127.0.0.1";
        public const int serProt = 9366;
    }
}
