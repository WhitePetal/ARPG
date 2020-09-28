/*********************************************************
	文件：ClientSession
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/14 10:48:13
	功能：客户端网络会话
***********************************************************/
using PENet;
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSession : PESession<GameMsg>
{
    protected override void OnConnected()
    {

    }

    protected override void OnReciveMsg(GameMsg msg)
    {
        NETCommon.Log("Recive Msg: " + (CMD)msg.cmd);
        NetSev.Instance.AddNetPack(msg);
    }

    protected override void OnDisConnected()
    {

    }
}
