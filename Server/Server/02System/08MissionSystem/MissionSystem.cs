using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*********************************************************
	文件：MissionWindow
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/8/29 20:08
	功能：副本业务系统
***********************************************************/

namespace Server
{
    class MissionSystem : SystemBase<MissionSystem>
    {
        private CfgService cfgSev;

        public override void Init()
        {
            base.Init();
            cfgSev = CfgService.Instance;
        }

        public void ReqMissionFight(MsgPack pack)
        {
            ReqMissionFight data = pack.msg.reqMissionFight;
            PlayerData pd = cacheSev.GetPlayerDataBySession(pack.session);
            int power = cfgSev.GetMapCfg(data.missionId).power;
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspMissionFight
            };
            if(pd.mission < data.missionId)
            {
                msg.err = (int)ErrorCode.CliendDataError;
                pack.session.SendMsg(msg);
                return;
            }
            if(power > pd.power)
            {
                msg.err = (int)ErrorCode.LackPower;
                pack.session.SendMsg(msg);
                return;
            }
            pd.power -= power;
            if (!cacheSev.UpdatePlayerData(pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
                pack.session.SendMsg(msg);
                return;
            }
            msg.rspMissionFight = new RspMissionFight
            {
                missionId = data.missionId,
                power = pd.power
            };
            pack.session.SendMsg(msg);
        }
    }
}
