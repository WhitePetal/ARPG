/*********************************************************
	文件：NetService
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/14 10:11:13
	功能：网络服务
***********************************************************/
using PENet;
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
    public class MsgPack
    {
        public ServerSession session;
        public GameMsg msg;

        public MsgPack(ServerSession session, GameMsg msg)
        {
            this.session = session;
            this.msg = msg;
        }
    }
    class NetService : Singleton<NetService>
    {
        private Queue<MsgPack> msgPackQue = new Queue<MsgPack>();

        public static readonly string obj = "lock";

        public void Init()
        {
            PESocket<ServerSession, GameMsg> server = new PESocket<ServerSession, GameMsg>();
            server.StartAsServer(SerCfg.serIP, SerCfg.serProt);

            PETool.LogMsg("NetSer Init Done");
        }

        public void AddMsgQue(MsgPack pack)
        {
            lock (obj)
            {
                msgPackQue.Enqueue(pack);
            } 
        }

        public void Update()
        {
            if(msgPackQue.Count > 0)
            {
                lock (obj)
                {
                    MsgPack pack = msgPackQue.Dequeue();
                    DealWithMsg(pack);
                }
            }
        }

        private void DealWithMsg(MsgPack pack)
        {
            switch ((CMD)pack.msg.cmd)
            {
                case CMD.ReqLogin:
                    LoginSystem.Instance.ReqLogin(pack);
                    break;
                case CMD.ReqRename:
                    LoginSystem.Instance.ReqRename(pack);
                    break;
                case CMD.ReqGuided:
                    GuidSystem.Instance.ReqGuide(pack);
                    break;
                case CMD.ReqStrong:
                    StrongSystem.Instance.ReqStrong(pack);
                    break;
                case CMD.SndChat:
                    ChatSystem.Instance.SndChat(pack);
                    break;
                case CMD.ReqBuy:
                    BuySystem.Instance.ReqBuy(pack);
                    break;
                case CMD.ReqTakeTaskReward:
                    TaskSystem.Instance.ReqTakeTaskReward(pack);
                    break;
                case CMD.ReqMissionFight:
                    MissionSystem.Instance.ReqMissionFight(pack);
                    break;
            }
        }
    }
}
