/*********************************************************
	文件：GuidSystem
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/18 14:38:20
	功能：待定
***********************************************************/
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class GuidSystem : SystemBase<GuidSystem>
    {
        private CfgService cfgSev;

        public override void Init()
        {
            cfgSev = CfgService.Instance;
            base.Init();
        }

        public void ReqGuide(MsgPack pack)
        {
            ReqGuide data = pack.msg.reqGuide;

            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspGuided
            };

            // 更新任务 ID
            PlayerData pd = cacheSev.GetPlayerDataBySession(pack.session);

            if(pd.guidid == data.guidID)
            {
                switch (pd.guidid)
                {
                    case 1001:
                        msg.pshTaskPrgs = TaskSystem.Instance.CalcTaskPrg(pd, 1);
                        break;
                }
                pd.guidid += 1;
                // 更新玩家数据
                AutoGuideCfg guidCfg = cfgSev.GetAutoGuidCfg(data.guidID);
                pd.coin += guidCfg.coin;
                NETCommon.CalcExp(pd, guidCfg.exp);

                if (!cacheSev.UpdatePlayerData(pd)) msg.err = (int)ErrorCode.UpdateDBError;
                else
                {
                    msg.rspGuide = new RspGuide
                    {
                        guideid = pd.guidid,
                        coin = pd.coin,
                        lv = pd.lv,
                        exp = pd.exp,
                        addExp = guidCfg.exp
                    };
                }
            }
            else
            {
                msg.err = (int)ErrorCode.ServerDataError;
            }

            pack.session.SendMsg(msg);
        }
    }
}
