/*********************************************************
	文件：ChatSystem
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/22 9:07:30
	功能：聊天系统
***********************************************************/
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Server
{
    class ChatSystem : SystemBase<ChatSystem>
    {
        private CfgService cfgSev;

        public override void Init()
        {
            cfgSev = CfgService.Instance;
            base.Init();
        }

        public void SndChat(MsgPack pack)
        {
            SndChat data = pack.msg.sndChat;
            PlayerData pd = cacheSev.GetPlayerDataBySession(pack.session);
            string txt = cfgSev.SerachFilterWordAndReplace(data.txt);
            Console.WriteLine("Recive TXT: " + txt);

            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.PshChat,
                pshChat = new PshChat
                {
                    name = pd.name,
                    txt = txt
                }
            };

            byte[] buffer = PENet.PETool.PackNetMsg(msg);

            // 广播
            List<ServerSession> list = cacheSev.GetOnlineServerSessions();
            Console.WriteLine("List Len: " + list.Count);
            for(int i = 0; i < list.Count; i++)
            {
                list[i].SendMsg(buffer);
            }

            PshTaskPrgs ptp = TaskSystem.Instance.CalcTaskPrg(pd, 6);
            if (ptp != null)
            {
                GameMsg pshTaskMsg = new GameMsg
                {
                    cmd = (int)CMD.PshTaskPrgs,
                    pshTaskPrgs = ptp
                };
                if (!cacheSev.UpdatePlayerData(pd)) msg.err = (int)ErrorCode.UpdateDBError;
                pack.session.SendMsg(pshTaskMsg);
            }
        }
    }
}
