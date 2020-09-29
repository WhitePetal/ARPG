/*********************************************************
	文件：CacheService
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/14 12:48:27
	功能：缓存层
***********************************************************/
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class CacheService : Singleton<CacheService>
    {
        private Dictionary<string, ServerSession> onlineAccountDic = new Dictionary<string, ServerSession>();
        private Dictionary<ServerSession, string> onlineAccountReversDic = new Dictionary<ServerSession, string>();
        private Dictionary<ServerSession, PlayerData> onlineSessionDic = new Dictionary<ServerSession, PlayerData>();
        private DBMgr dbMgr = null;

        public void Init()
        {
            dbMgr = DBMgr.Instance;
            NETCommon.Log("CacheSvc Init Done");
        }

        public bool IsAccountOnline(string account)
        {
            return onlineAccountDic.ContainsKey(account);
        }

        /// <summary>
        /// 根据账号密码返回账号数据，密码错误则返回null，账号不存在则创建账号
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public PlayerData GetPlayerData(string account, string pass)
        {
            // TODO
            // 从数据库中查找账号数据
            PlayerData pd = dbMgr.QueryPlayerData(account, pass);
            return pd;
        }

        /// <summary>
        /// 账号上线时缓存数据
        /// </summary>
        /// <param name="account"></param>
        /// <param name="session"></param>
        /// <param name="playerData"></param>
        public void CacheAccountOnline(string account, ServerSession session, PlayerData playerData)
        {
            onlineAccountDic.Add(account, session);
            onlineAccountReversDic.Add(session, account);
            onlineSessionDic.Add(session, playerData);
        }

        public List<ServerSession> GetOnlineServerSessions()
        {
            List<ServerSession> list = new List<ServerSession>();
            foreach(var pair in onlineSessionDic)
            {
                list.Add(pair.Key);
            }
            return list;
        }

        public bool IsNameExist(string name)
        {
            return dbMgr.QueryNameData(name);
        }

        public PlayerData GetPlayerDataBySession(ServerSession session)
        {
            if(onlineSessionDic.TryGetValue(session, out PlayerData playerData))
            {
                return playerData;
            }

            return null;
        }

        public bool UpdatePlayerData(PlayerData playerData)
        {
            return dbMgr.UpdatePlayerData(playerData);
        }

        public bool AcctOffLine(ServerSession session)
        {
            bool suc = true;
            string act = onlineAccountReversDic[session];
            suc = suc ? onlineAccountDic.Remove(act) : false;
            suc = suc ? onlineAccountReversDic.Remove(session) : false;
            suc = suc ? onlineSessionDic.Remove(session) : false;
            if (!suc) NETCommon.Log("Clear Cahce Error", NETLogLevel.Error);
            return suc;
        }

        public Dictionary<ServerSession, PlayerData> GetOnlinePlayers()
        {
            return onlineSessionDic;
        }
    }
}
