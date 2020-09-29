/*********************************************************
	文件：ServerSession
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/14 10:26:01
	功能：网络会话连接
***********************************************************/
using PENet;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ServerSession : PESession<GameMsg>
    {
        public int sessionID = 0;

        protected override void OnConnected()
        {
            sessionID = ServerRoot.Instance.GetSessionID();
            PETool.LogMsg("Client Connect ID: " + sessionID);
        }

        protected override void OnReciveMsg(GameMsg msg)
        {
            PETool.LogMsg("Client ReqCMD: " + (CMD)msg.cmd + "ID: " + sessionID);
            NetService.Instance.AddMsgQue(new MsgPack(this, msg));
        }

        protected override void OnDisConnected()
        {
            LoginSystem.Instance.ClearCache(this);
            PETool.LogMsg("Client DisConnect ID: " + sessionID);
        }
    }
}
