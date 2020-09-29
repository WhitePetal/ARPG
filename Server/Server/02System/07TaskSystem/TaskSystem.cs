using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class TaskSystem : SystemBase<TaskSystem>
    {
        private CfgService cfgService;

        public override void Init()
        {
            base.Init();
            cfgService = CfgService.Instance;
        }

        public void ReqTakeTaskReward(MsgPack pack)
        {
            ReqTakeTaskReward data = pack.msg.reqTakeTask;

            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspTakeTaskReward
            };

            PlayerData pd = cacheSev.GetPlayerDataBySession(pack.session);

            TaskRewardCfg cfg = cfgService.GetTaskRewardCfg(data.id);
            TaskRewardData trd = CalcTaskRewardData(pd, data.id);

            if (trd.prgs != cfg.count || trd.take)
            {
                msg.err = (int)ErrorCode.TakeTaskError;
                pack.session.SendMsg(msg);
                return;
            }

            pd.coin += cfg.coin;
            NETCommon.CalcExp(pd, cfg.exp);
            trd.take = true;
            CalcTaskArr(pd, trd);
            if (!cacheSev.UpdatePlayerData(pd))
            {
                msg.err = (int)ErrorCode.UpdateDBError;
                pack.session.SendMsg(msg);
                return;
            }
            msg.rspTakeTask = new RspTakeTaskReward
            {
                coin = pd.coin,
                exp = pd.exp,
                lv = pd.lv,
                taskArr = pd.taskArr
            };
            pack.session.SendMsg(msg);
        }

        private TaskRewardData CalcTaskRewardData(PlayerData data, int id)
        {
            for(int i = 0; i < data.taskArr.Length; i++)
            {
                if (data.taskArr[i][0] - 48 == id)
                {
                    TaskRewardData trd = new TaskRewardData
                    {
                        ID = id,
                        prgs =  data.taskArr[i][2] - 48,
                        take = data.taskArr[i][4] - 48 == 1
                    };
                    return trd;
                }
            }

            return null;
        }

        private void CalcTaskArr(PlayerData pd, TaskRewardData trd)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(trd.ID.ToString());
            sb.Append('|');
            sb.Append(trd.prgs);
            sb.Append('|');
            sb.Append(trd.take ? '1' : '0');
            string res = sb.ToString();
            sb.Clear();
            for(int i = 0; i < pd.taskArr.Length; i++)
            {
                if(pd.taskArr[i][0] - 48 == trd.ID)
                {
                    pd.taskArr[i] = res;
                    return;
                }
            }
        }

        public PshTaskPrgs CalcTaskPrg(PlayerData pd, int id)
        {
            TaskRewardData trd = CalcTaskRewardData(pd, id);
            TaskRewardCfg cfg = cfgService.GetTaskRewardCfg(id);
            PshTaskPrgs psh = null;
            if(trd.prgs < cfg.count)
            {
                ++trd.prgs;
                CalcTaskArr(pd, trd);
                psh = new PshTaskPrgs
                {
                    taskAr = pd.taskArr
                };
            }
            return psh;
        }
    }
}
