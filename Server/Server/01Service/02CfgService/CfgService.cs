/*********************************************************
	文件：CfgService
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/18 15:23:53
	功能：待定
***********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Server
{
    class CfgService : Singleton<CfgService>
    {
        public void Init()
        {
            InitAutoGuidCfg();
            InitStrongCfg();
            InitFilterWordCfg();
            InitTaskCfg();
            InitMapCfg();
            NETCommon.Log("CfgSev Init Done");
        }

        #region Guid
        private Dictionary<int, AutoGuideCfg> autoGuidDic = new Dictionary<int, AutoGuideCfg>();
        private void InitAutoGuidCfg()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"F:\Unity\ARPG\Assets\Plugins\Editor\RawConfig\guide.xml");

            XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
            for (int i = 0; i < nodeList.Count; i++)
            {
                var ele = nodeList[i] as XmlElement;
                if (ele.GetAttribute("ID") == null) continue;
                int id = int.Parse(ele.GetAttributeNode("ID").InnerText);
                AutoGuideCfg cfg = new AutoGuideCfg { ID = id };
                foreach (XmlElement e in nodeList[i].ChildNodes)
                {
                    switch (e.Name)
                    {
                        case "coin":
                            cfg.coin = int.Parse(e.InnerText);
                            break;
                        case "exp":
                            cfg.exp = int.Parse(e.InnerText);
                            break;
                    }
                }
                autoGuidDic.Add(id, cfg);
            }
            NETCommon.Log("GuidCfg Init Done...");
        }
        public AutoGuideCfg GetAutoGuidCfg(int id)
        {
            AutoGuideCfg cfg = null;
            if (autoGuidDic.TryGetValue(id, out cfg)) return cfg;
            else
            {
                NETCommon.Log("AutoGuideCfg 获取失败", NETLogLevel.Error);
                return null;
            }
        }
        #endregion

        #region Strong
        private Dictionary<int, Dictionary<int, StrongCfg>> strongCfgDic = new Dictionary<int, Dictionary<int, StrongCfg>>();
        private void InitStrongCfg()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"F:\Unity\ARPG\Assets\Plugins\Editor\RawConfig\strong.xml");

            XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;

            for (int i = 0; i < nodeList.Count; i++)
            {
                XmlElement ele = nodeList[i] as XmlElement;
                if (ele.GetAttribute("ID") == null) continue;

                int ID = int.Parse(ele.GetAttributeNode("ID").InnerText);
                StrongCfg sc = new StrongCfg { ID = ID };

                foreach (XmlElement e in nodeList[i].ChildNodes)
                {
                    int val = int.Parse(e.InnerText);
                    switch (e.Name)
                    {
                        case "pos":
                            sc.pos = val;
                            break;
                        case "starlv":
                            sc.starLv = val;
                            break;
                        case "addhp":
                            sc.addHp = val;
                            break;
                        case "adddef":
                            sc.addDef = val;
                            break;
                        case "minlv":
                            sc.minLv = val;
                            break;
                        case "coin":
                            sc.coin = val;
                            break;
                        case "crystal":
                            sc.crystal = val;
                            break;
                    }
                }

                Dictionary<int, StrongCfg> dic = null;
                if (strongCfgDic.TryGetValue(sc.pos, out dic))
                {
                    dic.Add(sc.starLv, sc);
                }
                else
                {
                    dic = new Dictionary<int, StrongCfg>();
                    dic.Add(sc.starLv, sc);
                    strongCfgDic.Add(sc.pos, dic);
                }
            }
            NETCommon.Log("StrongCfg InitDone");
        }
        public StrongCfg GetStrongCfgData(int pos, int starLv)
        {
            StrongCfg res = null;
            if (strongCfgDic.ContainsKey(pos))
            {
                Dictionary<int, StrongCfg> dic = strongCfgDic[pos];
                if (dic.ContainsKey(starLv)) return dic[starLv];
            }

            if (res == null) NETCommon.Log("StrongCfg获取失败", NETLogLevel.Error);

            return res;
        }
        #endregion

        #region FilterWord
        private Hashtable map;

        private void InitFilterWordCfg()
        {
            List<string> filterWords = new List<string>();
            // LoadCfg
            XmlDocument doc = new XmlDocument();
            doc.Load(@"F:\Unity\ARPG\Assets\Plugins\Editor\RawConfig\filterword.xml");
            XmlNode root = doc.SelectSingleNode("root");
            foreach (XmlElement item in root.ChildNodes)
            {
                int id = int.Parse(item.GetAttribute("ID"));
                foreach (XmlElement ele in item.ChildNodes)
                {
                    if (ele.Name == "word") filterWords.Add(ele.InnerText);
                }
            }

            InitFilter(filterWords);
        }

        private void InitFilter(List<string> words)
        {
            map = new Hashtable(words.Count);
            for (int i = 0; i < words.Count; i++)
            {
                string word = words[i];
                Hashtable indexMap = map;
                for (int j = 0; j < word.Length; j++)
                {
                    char c = word[j];
                    if (indexMap.ContainsKey(c))
                    {
                        indexMap = (Hashtable)indexMap[c];
                    }
                    else
                    {
                        Hashtable newMap = new Hashtable();
                        newMap.Add("IsEnd", 0);
                        indexMap.Add(c, newMap);
                        indexMap = newMap;
                    }

                    if (j == word.Length - 1)
                    {
                        if (indexMap.ContainsKey("IsEnd")) indexMap["IsEnd"] = 1;
                        else indexMap.Add("IsEnd", 1);
                    }
                }
            }
        }

        private int CheckFilterWord(string txt, int beginIndex)
        {
            bool flag = false;
            int len = 0;
            Hashtable curMap = map;
            for (int i = beginIndex; i < txt.Length; i++)
            {
                char c = txt[i];
                Hashtable temp = (Hashtable)curMap[c];
                if (temp != null)
                {
                    if ((int)temp["IsEnd"] == 1) flag = true;
                    else curMap = temp;

                    len++;
                }
                else break;
            }

            if (!flag) len = 0;

            return len;
        }

        public string SerachFilterWordAndReplace(string txt)
        {
            int i = 0;
            StringBuilder sb = new StringBuilder(txt);
            while (i < txt.Length)
            {
                int len = CheckFilterWord(txt, i);
                if (len > 0)
                {
                    for (int j = 0; j < len; j++)
                    {
                        sb[i + j] = '*';
                    }
                    i += len;
                }
                else ++i;
            }
            return sb.ToString();
        }
        #endregion

        #region Task
        private Dictionary<int, TaskRewardCfg> taskRewardCfgDic = new Dictionary<int, TaskRewardCfg>();
        private void InitTaskCfg()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"F:\Unity\ARPG\Assets\Plugins\Editor\RawConfig\taskreward.xml");
            XmlNode root = doc.SelectSingleNode("root");
            foreach(XmlElement item in root.ChildNodes)
            {
                if (item.GetAttributeNode("ID") == null) continue;
                int id = int.Parse(item.GetAttributeNode("ID").InnerText);
                TaskRewardCfg cfg = new TaskRewardCfg();
                cfg.ID = id;
                foreach(XmlElement ele in item.ChildNodes)
                {
                    string value = ele.InnerText;
                    switch (ele.Name)
                    {
                        case "taskName":
                            cfg.taskName = value;
                            break;
                        case "exp":
                            cfg.exp = int.Parse(value);
                            break;
                        case "coin":
                            cfg.coin = int.Parse(value);
                            break;
                        case "count":
                            cfg.count = int.Parse(value);
                            break;
                    }
                }
                taskRewardCfgDic.Add(id, cfg);
            }
        }
        public TaskRewardCfg GetTaskRewardCfg(int id)
        {
            TaskRewardCfg cfg = null;
            if(taskRewardCfgDic.TryGetValue(id, out cfg))
            {
                return cfg;
            }
            NETCommon.Log("获取 TaskRewardCfg 失败，ID：" + id);
            return null;
        }
        #endregion

        #region Map
        private Dictionary<int, MapCfg> mapCfgDic = new Dictionary<int, MapCfg>();
        private void InitMapCfg()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"F:\Unity\ARPG\Assets\Plugins\Editor\RawConfig\map.xml");
            XmlNode root = doc.SelectSingleNode("root");
            foreach (XmlElement item in root.ChildNodes)
            {
                if (item.GetAttributeNode("ID") == null) continue;
                int id = int.Parse(item.GetAttributeNode("ID").InnerText);
                MapCfg cfg = new MapCfg();
                cfg.ID = id;
                foreach (XmlElement ele in item.ChildNodes)
                {
                    string value = ele.InnerText;
                    switch (ele.Name)
                    {
                        case "taskName":
                            cfg.power = int.Parse(value);
                            break;
                    }
                }
                mapCfgDic.Add(id, cfg);
            }
        }
        public MapCfg GetMapCfg(int id)
        {
            MapCfg cfg = null;
            if (mapCfgDic.TryGetValue(id, out cfg))
            {
                return cfg;
            }
            NETCommon.Log("获取 TaskRewardCfg 失败，ID：" + id);
            return null;
        }
        #endregion
    }

    public class BaseData<T>
    {
        public int ID;
    }

    public class AutoGuideCfg : BaseData<AutoGuideCfg>
    {
        public int coin;
        public int exp;
    }

    public class StrongCfg : BaseData<StrongCfg>
    {
        public int pos;
        public int starLv;
        public int addHp;
        public int addHurt;
        public int addDef;
        public int minLv;
        public int coin;
        public int crystal;
    }

    public class TaskRewardCfg : BaseData<TaskRewardCfg>
    {
        public string taskName;
        public int exp;
        public int coin;
        public int count;
    }

    public class TaskRewardData : BaseData<TaskRewardData>
    {
        public int prgs;
        public bool take;
    }

    public class MapCfg : BaseData<MapCfg>
    {
        public int power;
    }
}
