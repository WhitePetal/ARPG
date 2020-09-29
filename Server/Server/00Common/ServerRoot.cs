/*********************************************************
	文件：ServerRoot
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/14 10:07:44
	功能：服务器入口
***********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class ServerRoot : Singleton<ServerRoot>
    {
        public void Init()
        {
            // 数据层
            DBMgr.Instance.Init();

            // 服务层
            NetService.Instance.Init();
            CacheService.Instance.Init();
            CfgService.Instance.Init();
            TimerSevrvice.Instance.Init(20, true);
            TimerSevrvice.Instance.StartTimer();

            // 业务系统层
            LoginSystem.Instance.Init();
            GuidSystem.Instance.Init();
            StrongSystem.Instance.Init();
            ChatSystem.Instance.Init();
            BuySystem.Instance.Init();
            PowerSystem.Instance.Init();
            TaskSystem.Instance.Init();
            MissionSystem.Instance.Init();
        }

        public void Update()
        {
            NetService.Instance.Update();
            TimerSevrvice.Instance.DealTask();
        }

        private int SessionID = 0;
        public int GetSessionID()
        {
            if (SessionID == int.MaxValue) SessionID = 0;
            return SessionID += 1;
        }
    }
}
