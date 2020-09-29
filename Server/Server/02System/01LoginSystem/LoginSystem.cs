/*********************************************************
	文件：LoginSystem
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/14 10:13:19
	功能：登录业务
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
    class LoginSystem : SystemBase<LoginSystem>
    {
        private TimerSevrvice timeSev;
        public override void Init()
        {
            base.Init();
            timeSev = TimerSevrvice.Instance;
        }

        public void ReqLogin(MsgPack pack)
        {
            ReqLogin data = pack.msg.reqLogin;
            // 判断当前账号是否已经上线
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspLogin,
                rspLogin = new RspLogin()
            };
            // 已上线：返回错误信息
            if (cacheSev.IsAccountOnline(data.account)) msg.err = (int)ErrorCode.AccountIsOnline;
            else // 未上线：
            {
                // 账号是否存在并检测密码
                PlayerData pd = cacheSev.GetPlayerData(data.account, data.pass);
                if (pd == null) msg.err = (int)ErrorCode.WrongPass;
                else
                {
                    int power = pd.power;
                    long milliseconds = timeSev.GetNowTime() - pd.time;
                    int addPower = (int)(milliseconds / (1000 * 60 * NETCommon.PowerAddSpace)) * NETCommon.PowerAddCount;
                    if(addPower > 0)
                    {
                        int powerMax = NETCommon.GetPowerLimit(pd.lv);
                        int targetPower = pd.power + addPower;
                        pd.power = pd.power < powerMax ? (targetPower > powerMax ? powerMax : targetPower) : powerMax;
                    }
                    if (power != pd.power) cacheSev.UpdatePlayerData(pd);

                    msg.rspLogin = new RspLogin
                    {
                        playerData = pd
                    };

                    // 缓存账号数据
                    cacheSev.CacheAccountOnline(data.account, pack.session, pd);
                }
            }
                
            // 回应客户端
            pack.session.SendMsg(msg);
        }

        public void ReqRename(MsgPack pack)
        {
            ReqRename data = pack.msg.reqRename;
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.RspRename
            };

            // 名字是否存在
            if (cacheSev.IsNameExist(data.name))
            {
                // 存在：返回错误码
                msg.err = (int)ErrorCode.NameIsExit;
            }
            else
            {
                // 不存在：更新缓存和数据库，再返回给客户端
                PlayerData playerData = cacheSev.GetPlayerDataBySession(pack.session);
                playerData.name = data.name;
                if(playerData == null)
                {
                    msg.err = (int)ErrorCode.GetCacheError;
                    pack.session.SendMsg(msg);
                    return;
                }

                if(!cacheSev.UpdatePlayerData(playerData))
                {
                    msg.err = (int)ErrorCode.UpdateDBError;
                }
                else
                {
                    msg.rspRename = new RspRename(data.name);
                }
            }

            pack.session.SendMsg(msg);
        }

        public void ClearCache(ServerSession session)
        {
            PlayerData pd = cacheSev.GetPlayerDataBySession(session);
            if(pd != null)
            {
                pd.time = timeSev.GetNowTime();
                if (!cacheSev.UpdatePlayerData(pd)) NETCommon.Log("Update Offline time Error", NETLogLevel.Error);
            }
            cacheSev.AcctOffLine(session);
        }
    }
}
