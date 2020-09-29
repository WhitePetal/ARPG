/*********************************************************
	文件：PowerSystem
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/26 13:56:24
	功能：待定
***********************************************************/
using BJTimer;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class PowerSystem : SystemBase<PowerSystem>
    {
        private TimerSevrvice timeSev;
        public override void Init()
        {
            base.Init();
            timeSev = TimerSevrvice.Instance;
            timeSev.AddbTimerTask(CalcPowerAdd, NETCommon.PowerAddSpace, 0, TimeUnit.Minute);
        }

        private void CalcPowerAdd(int id)
        {
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.PshPower,
                pshPower = new PshPower()
            };

            // 向所有在线玩家推送
            Dictionary<ServerSession, PlayerData> onlineDic = cacheSev.GetOnlinePlayers();
            foreach(var pair in onlineDic)
            {
                PlayerData pd = pair.Value;
                ServerSession session = pair.Key;

                int powerMax = NETCommon.GetPowerLimit(pd.lv);
                if (pd.power >= powerMax) continue;

                pd.power += NETCommon.PowerAddCount;
                if (pd.power > powerMax) pd.power = powerMax;

                pd.time = timeSev.GetNowTime();

                if (!cacheSev.UpdatePlayerData(pd)) msg.err = (int)ErrorCode.UpdateDBError;
                else msg.pshPower.power = pd.power;
                session.SendMsg(msg);
            }
        }
    }
}
