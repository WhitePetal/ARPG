/*********************************************************
	文件：BuySystem
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/23 12:24:40
	功能：交易系统
***********************************************************/
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class BuySystem : SystemBase<BuySystem>
    {
        public override void Init()
        {
            base.Init();
        }

        public void ReqBuy(MsgPack pack)
        {
            ReqBuy data = pack.msg.reqBuy;

            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspBuy,
            };

            PlayerData pd = cacheSev.GetPlayerDataBySession(pack.session);
            switch (data.type)
            {
                case 0:
                    msg.pshTaskPrgs = TaskSystem.Instance.CalcTaskPrg(pd, 4);
                    break;
                case 1:
                    msg.pshTaskPrgs = TaskSystem.Instance.CalcTaskPrg(pd, 5);
                    break;
            }

            if (pd.diamond < data.cost)
            {
                msg.err = (int)ErrorCode.LackDiamond;
                pack.session.SendMsg(msg);
                return;
            }
            pd.diamond -= data.cost;
            switch (data.type)
            {
                case 0:
                    pd.power += 100;
                    break;
                case 1:
                    pd.coin += 1000;
                    break;
            }
            if (!cacheSev.UpdatePlayerData(pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
                pack.session.SendMsg(msg);
            }
            msg.rspBuy = new RspBuy
            {
                type = data.type,
                coin = pd.coin,
                power = pd.power,
                dimond = pd.diamond
            };
            pack.session.SendMsg(msg);
        }
    }
}
