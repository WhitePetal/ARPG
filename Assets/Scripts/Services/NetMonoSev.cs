/*********************************************************
	文件：NetMonoSev
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/14 13:15:22
	功能：网络服务Mono分支
***********************************************************/
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetMonoSev : MonoSingleton<NetMonoSev>
{
    private NetSev netSev = null;

    public void Init()
    {
        netSev = NetSev.Instance;
        DontDestroyOnLoad(this);
    }

    public void DealWithMsg(GameMsg msg)
    {
        if(msg.err != (int)ErrorCode.None)
        {
            switch ((ErrorCode)msg.err)
            {
                case ErrorCode.AccountIsOnline:
                    GameRoot.AddTips("当前账号已经上线");
                    break;
                case ErrorCode.WrongPass:
                    GameRoot.AddTips("密码错误");
                    break;
                case ErrorCode.GetCacheError:
                    NETCommon.Log("错误代码202：缓存获取错误", NETLogLevel.Error);
                    GameRoot.AddTips("网络不稳定");
                    break;
                case ErrorCode.UpdateDBError:
                    NETCommon.Log("错误代码201：数据库更新错误", NETLogLevel.Error);
                    GameRoot.AddTips("网络不稳定");
                    break;
                case ErrorCode.ServerDataError:
                    NETCommon.Log("错误代码203：服务器数据异常", NETLogLevel.Error);
                    GameRoot.AddTips("客户端数据异常");
                    break;
                case ErrorCode.LackCoin:
                    GameRoot.AddTips("金币不足！");
                    break;
                case ErrorCode.LackCrystal:
                    GameRoot.AddTips("水晶不足！");
                    break;
                case ErrorCode.LackLevel:
                    GameRoot.AddTips("等级不足！");
                    break;
                case ErrorCode.LackDiamond:
                    GameRoot.AddTips("钻石不足！");
                    break;
                case ErrorCode.TakeTaskError:
                    NETCommon.Log("错误代码：401, 领取任务数据异常", NETLogLevel.Error);
                    GameRoot.AddTips("客户端数据异常");
                    break;
                case ErrorCode.CliendDataError:
                    NETCommon.Log("错误代码：204, 客户端数据异常", NETLogLevel.Error);
                    GameRoot.AddTips("客户端数据异常");
                    break;
                case ErrorCode.LackPower:
                    GameRoot.AddTips("体力不足！");
                    break;
            }
            return;
        }

        switch ((CMD)msg.cmd)
        {
            case CMD.RspLogin:
                LoginSys.Instance.RespLogin(msg);
                break;
            case CMD.RspRename:
                LoginSys.Instance.RspRename(msg);
                break;
            case CMD.RspGuided:
                MainCitySys.Instance.RspGuide(msg);
                break;
            case CMD.RspStrong:
                MainCitySys.Instance.RspStrong(msg);
                break;
            case CMD.PshChat:
                MainCitySys.Instance.PshChat(msg);
                break;
            case CMD.RspBuy:
                MainCitySys.Instance.RspBuy(msg);
                break;
            case CMD.PshPower:
                MainCitySys.Instance.PshPower(msg);
                break;
            case CMD.RspTakeTaskReward:
                MainCitySys.Instance.RspTakeReward(msg);
                break;
            case CMD.PshTaskPrgs:
                MainCitySys.Instance.PshTaskPrgs(msg);
                break;
            case CMD.RspMissionFight:
                MissionSys.Instance.RspMissionFight(msg);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(netSev.msgQue.Count > 0)
        {
            lock (NetSev.obj)
            {
                DealWithMsg(netSev.msgQue.Dequeue());
            }
        }
    }
}
