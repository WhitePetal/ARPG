/*********************************************************
	文件：StrongSystem
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/20 14:46:43
	功能：强化升级系统
***********************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol;

namespace Server
{
    class StrongSystem : SystemBase<StrongSystem>
    {
        private CfgService cfgSev;
        public override void Init()
        {
            base.Init();
            cfgSev = CfgService.Instance;
        }

        public void ReqStrong(MsgPack pack)
        {
            ServerSession session = pack.session;
            ReqStrong data = pack.msg.reqStrong;

            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspStrong
            };

            PlayerData pd = cacheSev.GetPlayerDataBySession(session);
            int curStar = pd.strongArr[data.pos];
            StrongCfg nextSc = cfgSev.GetStrongCfgData(data.pos, curStar + 1);
            // 条件判断
            if(pd.lv < nextSc.minLv)
            {
                msg.err = (int)ErrorCode.LackLevel;
                session.SendMsg(msg);
                return;
            }
            if(pd.coin < nextSc.coin)
            {
                msg.err = (int)ErrorCode.LackCoin;
                session.SendMsg(msg);
                return;
            }
            if(pd.crystal < nextSc.crystal)
            {
                msg.err = (int)ErrorCode.LackCrystal;
                session.SendMsg(msg);
                return;
            }

            // 资源扣除
            pd.coin -= nextSc.coin;
            pd.crystal -= nextSc.crystal;

            // 属性增益
            pd.strongArr[data.pos] += 1;
            pd.hp += nextSc.addHp;
            pd.ad += nextSc.addHurt * (1 + 1 / (data.pos + 1));
            pd.ap += nextSc.addHurt;
            pd.addef += nextSc.addDef;
            pd.apdef += nextSc.addDef;

            // 任务进度更新
            msg.pshTaskPrgs = TaskSystem.Instance.CalcTaskPrg(pd, 3);

            // 更新数据库
            if (!cacheSev.UpdatePlayerData(pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
                session.SendMsg(msg);
                return;
            }

            // 返回消息
            msg.rspStrong = new RspStrong
            {
                coin = pd.coin,
                crystal = pd.crystal,
                hp = pd.hp,
                ad = pd.ad,
                ap = pd.ap,
                adddef = pd.addef,
                apdef = pd.apdef,
                strongArr = pd.strongArr
            };
            session.SendMsg(msg);
        }
    }
}
