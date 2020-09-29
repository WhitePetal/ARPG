/*********************************************************
	文件：DBMgr
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/14 15:02:36
	功能：数据库管理类
***********************************************************/
using MySql.Data.MySqlClient;
using Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class DBMgr : Singleton<DBMgr>
    {
        private MySqlConnection conn;

        public void Init()
        {
            conn = new MySqlConnection("server=localhost;userid=root;password=;database=arpg;charset=utf8");
            conn.Open();
            
            NETCommon.Log(conn.Database);
            NETCommon.Log("DBMgr Init Done");
        }

        public PlayerData QueryPlayerData(string account, string password)
        {
            PlayerData playerData = null;
            MySqlDataReader reader = null;
            bool isNew = true;

            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from account where account = @account", conn);
                cmd.Parameters.AddWithValue("account", account);
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    isNew = false;
                    string _pass = reader.GetString("pass");
                    if(_pass.Equals(password))
                    {
                        playerData = new PlayerData
                        {
                            id = reader.GetInt32("id"),
                            name = reader.GetString("name"),
                            lv = reader.GetInt32("level"),
                            exp = reader.GetInt32("exp"),
                            power = reader.GetInt32("power"),
                            coin = reader.GetInt32("coin"),
                            diamond = reader.GetInt32("diamond"),
                            crystal = reader.GetInt32("crystal"),

                            hp = reader.GetInt32("hp"),
                            ad = reader.GetInt32("ad"),
                            ap = reader.GetInt32("ap"),
                            addef = reader.GetInt32("addef"),
                            apdef = reader.GetInt32("apdef"),
                            dodge = reader.GetInt32("dodge"),
                            pierce = reader.GetInt32("pierce"),
                            critical = reader.GetInt32("critical"),

                            guidid = reader.GetInt32("guidid"),

                            mission = reader.GetInt32("mission"),

                            time = reader.GetInt64("time")
                            // TOADD
                        };
                        #region Strong
                        // 1#2#5#7#10#6
                        string[] strongArStr = reader.GetString("strong").Split('#');
                        int[] strongAr = new int[6];
                        for (int i = 0; i < strongArStr.Length; i++)
                        {
                            if (strongArStr[i] == "") continue;
                            if (!int.TryParse(strongArStr[i], out strongAr[i]))
                            {
                                NETCommon.Log("Parse Strong Error", NETLogLevel.Error);
                            }
                        }
                        playerData.strongArr = strongAr;
                        #endregion
                        #region Task
                        // 1|1|0#2|0|0#3|3|1
                        string[] taskStrAr = reader.GetString("task").Split('#');
                        playerData.taskArr = new string[taskStrAr.Length - 1];
                        for(int i = 0; i < taskStrAr.Length; i++)
                        {
                            if (string.IsNullOrEmpty(taskStrAr[i]) || taskStrAr[i].Length < 5) continue;
                            playerData.taskArr[i] = taskStrAr[i];
                        }
                        #endregion
                        // TOADD
                    }
                }
            }
            catch(Exception e)
            {
                NETCommon.Log("Query PlayerData By Account&Password Error: " + e, NETLogLevel.Error);
            }
            finally
            {
                if (reader != null) reader.Close();
                if (isNew)
                {
                    // 不存在账号数据，创建新的默认账号数据，并返回
                    playerData = new PlayerData
                    {
                        id = -1,
                        name = "",
                        lv = 1,
                        exp = 0,
                        power = 150,
                        coin = 5000,
                        diamond = 500,
                        crystal = 500,

                        hp = 2000,
                        ad = 275,
                        ap = 265,
                        addef = 67,
                        apdef = 43,
                        dodge = 7,
                        pierce = 5,
                        critical = 2,

                        guidid = 1001,

                        strongArr = new int[6],

                        taskArr = new string[6],

                        mission = 10001,

                        time = (long)TimerSevrvice.Instance.GetMillisecondsTime()
                        //TOADD
                    };

                    for(int i = 0; i < playerData.taskArr.Length; i++)
                    {
                        playerData.taskArr[i] = (i + 1).ToString() + "|0|0";
                    }
                    playerData.id = InserNewAccountData(account, password, playerData);
                }
            }

            return playerData;
        }

        public bool QueryNameData(string name)
        {
            bool exit = false;
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from account where name=@name", conn);
                cmd.Parameters.AddWithValue("name", name);
                reader = cmd.ExecuteReader();


                if (reader.Read()) exit = true;
            }
            catch(Exception e)
            {
                NETCommon.Log("Query Name Data Error: " + e, NETLogLevel.Error);
            }
            finally
            {
                if (reader != null) reader.Close();
            }

            return exit;
        }

        public bool UpdatePlayerData(PlayerData data)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("update account set name=@name,level=@level,exp=@exp,power=@power,coin=@coin,diamond=@diamond,crystal=@crystal," +
                    "hp=@hp,ad=@ad,ap=@ap,addef=@addef,apdef=@apdef,dodge=@dodge,pierce=@pierce,critical=@critical,guidid=@guidid," +
                    "strong=@strong,time=@time,task=@task,mission=@mission where id=@id", conn);
                cmd.Parameters.AddWithValue("id", data.id);
                cmd.Parameters.AddWithValue("name", data.name);
                cmd.Parameters.AddWithValue("level", data.lv);
                cmd.Parameters.AddWithValue("exp", data.exp);
                cmd.Parameters.AddWithValue("power", data.power);
                cmd.Parameters.AddWithValue("coin", data.coin);
                cmd.Parameters.AddWithValue("diamond", data.diamond);
                cmd.Parameters.AddWithValue("crystal", data.crystal);

                cmd.Parameters.AddWithValue("hp", data.hp);
                cmd.Parameters.AddWithValue("ad", data.ad);
                cmd.Parameters.AddWithValue("ap", data.ap);
                cmd.Parameters.AddWithValue("addef", data.addef);
                cmd.Parameters.AddWithValue("apdef", data.apdef);
                cmd.Parameters.AddWithValue("dodge", data.dodge);
                cmd.Parameters.AddWithValue("pierce", data.pierce);
                cmd.Parameters.AddWithValue("critical", data.critical);

                cmd.Parameters.AddWithValue("guidid", data.guidid);

                cmd.Parameters.AddWithValue("mission", data.mission);

                cmd.Parameters.AddWithValue("time", data.time);

                #region Strong
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < data.strongArr.Length; i++)
                {
                    sb.Append(data.strongArr[i]);
                    sb.Append('#');
                }
                cmd.Parameters.AddWithValue("strong", sb.ToString());
                sb.Clear();
                #endregion
                #region Task
                for(int i = 0; i < data.taskArr.Length; i++)
                {
                    sb.Append(data.taskArr[i]);
                    sb.Append('#');
                }
                cmd.Parameters.AddWithValue("task", sb.ToString());
                sb.Clear();
                #endregion

                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                NETCommon.Log("Update PlayerData Error: " + e, NETLogLevel.Error);
                return false;
            }

            return true;
        }

        private int InserNewAccountData(string account, string password, PlayerData pd)
        {
            int id = -1;
            try
            {
                MySqlCommand cmd = new MySqlCommand("insert into account set account=@account,pass=@password,name=@name,level=@level,exp=@exp,power=@power,coin=@coin,diamond=@diamond,crystal=@crystal," +
                    "hp=@hp,ad=@ad,ap=@ap,addef=@addef,apdef=@apdef,dodge=@dodge,pierce=@pierce,critical=@critical,guidid=@guidid," +
                    "strong=@strong,time=@time,task=@task,mission=@mission", conn);
                cmd.Parameters.AddWithValue("account", account);
                cmd.Parameters.AddWithValue("password", password);
                cmd.Parameters.AddWithValue("name", pd.name);
                cmd.Parameters.AddWithValue("level", pd.lv);
                cmd.Parameters.AddWithValue("exp", pd.exp);
                cmd.Parameters.AddWithValue("power", pd.power);
                cmd.Parameters.AddWithValue("coin", pd.coin);
                cmd.Parameters.AddWithValue("diamond", pd.diamond);
                cmd.Parameters.AddWithValue("crystal", pd.crystal);

                cmd.Parameters.AddWithValue("hp", pd.hp);
                cmd.Parameters.AddWithValue("ad", pd.ad);
                cmd.Parameters.AddWithValue("ap", pd.ap);
                cmd.Parameters.AddWithValue("addef", pd.addef);
                cmd.Parameters.AddWithValue("apdef", pd.apdef);
                cmd.Parameters.AddWithValue("dodge", pd.dodge);
                cmd.Parameters.AddWithValue("pierce", pd.pierce);
                cmd.Parameters.AddWithValue("critical", pd.critical);

                cmd.Parameters.AddWithValue("guidid", pd.guidid);

                cmd.Parameters.AddWithValue("mission", pd.mission);

                cmd.Parameters.AddWithValue("time", pd.time);


                StringBuilder sb = new StringBuilder();
                #region Strong
                for(int i = 0; i < pd.strongArr.Length; i++)
                {
                    sb.Append(pd.strongArr[i]);
                    sb.Append('#');
                }
                cmd.Parameters.AddWithValue("strong", sb.ToString());
                sb.Clear();
                #endregion
                #region Task
                for (int i = 0; i < pd.taskArr.Length; i++)
                {
                    sb.Append(pd.taskArr[i]);
                    sb.Append('#');
                }
                cmd.Parameters.AddWithValue("task", sb.ToString());
                sb.Clear();
                #endregion
                cmd.ExecuteNonQuery();
                id = (int)cmd.LastInsertedId;
            }
            catch(Exception e)
            {
                NETCommon.Log("Query PlayerData By Account&Password Error: " + e, NETLogLevel.Error);
            }
            
            return id;
        }

    }
}
