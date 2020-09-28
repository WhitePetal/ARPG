/*********************************************************
	文件：NetSev
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/14 10:43:02
	功能：网络服务
***********************************************************/
using PENet;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSev : ServiceBase<NetSev>
{
    public Queue<GameMsg> msgQue = new Queue<GameMsg>();
    public static readonly string obj = "lock";

    private PESocket<ClientSession, GameMsg> client;   

    public override void InitSev()
    {
        NetMonoSev.InitSingleton().Init();
        client = new PESocket<ClientSession, GameMsg>();
        client.StartAsClient(SerCfg.serIP, SerCfg.serProt);

        client.SetLog(true, (msg, lv) =>
        {
            switch (lv)
            {
                case 0:
                    msg = "Log: " + msg;
                    Debug.Log(msg);
                    break;
                case 1:
                    msg = "Warm: " + msg;
                    Debug.LogWarning(msg);
                    break;
                case 2:
                    msg = "Error: " + msg;
                    Debug.LogError(msg);
                    break;
                case 3:
                    msg = "Info: " + msg;
                    Debug.Log(msg);
                    break;
            }
        });

        Debug.Log("Init NetSev...");
    }

    public void SendMsg(GameMsg msg)
    {
        if(client.session != null) client.session.SendMsg(msg);
        else
        {
            GameRoot.AddTips("服务器未连接");
            InitSev();
        }
    }

    public void AddNetPack(GameMsg msg)
    {
        lock (obj)
        {
            msgQue.Enqueue(msg);
        }
    }

    // 数据分发处理转至 NetMonoSev

}
