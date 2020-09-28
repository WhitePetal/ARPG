/*********************************************************
	文件：ResSev
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/12 14:31:39
	功能：资源服务
***********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResSev : ServiceBase<ResSev>
{
    private Action prgCB = null;

    public override void InitSev()
    {
        Debug.Log("Init ResSev...");
        InitRDNameCfg();
        InitMapCfg();
        InitAutoGuidCfg();
        InitStrongCfg();
        InitFilterWordCfg();
        InitTaskCfg();
        InitSkillCfg();
        InitSkillMoveCfg();
        InitMonsterCfg();
        InitSkillActionCfg();
        InitPRDCfg();
    }

    public void AsyncLoadScene(string sceneName, Action loaded)
    {
        GameRoot.Instance.lodingWindow.SetWindowState(true);

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        
        prgCB = () =>
        {
            float val = async.progress;
            GameRoot.Instance.lodingWindow.SetProgress(async.progress);
            if (val >= 1.0f)
            {
                GameRoot.Instance.lodingWindow.SetWindowState(false);
                async = null;
                prgCB = null;
                if (loaded != null) loaded.Invoke();
            }
        };
        CoroutineSys.Instance.MStartCoroutine(AsyncLoadSceneCor());
    }
    private IEnumerator AsyncLoadSceneCor()
    {
        while(prgCB != null)
        {
            prgCB.Invoke();
            yield return null;
        }
    }

    private Dictionary<string, AudioClip> audioCache = new Dictionary<string, AudioClip>();
    public AudioClip LoadAudio(string path, bool cache = false)
    {
        AudioClip ac = null;
        if(!audioCache.TryGetValue(path, out ac))
        {
            ac = Resources.Load<AudioClip>(path);
            if (ac == null)
            {
                Debug.LogError("当前路径没有找到 AudioClip 路径为：" + path);
                return null;
            }
            if (cache) audioCache.Add(path, ac);
        }

        return ac;
    }

    private Dictionary<string, GameObject> prefabDic = new Dictionary<string, GameObject>();
    public GameObject LoadGoPrefab(string path, bool cache, Vector3 pos, Quaternion rot)
    {
        GameObject prefab = null;
        if (!prefabDic.TryGetValue(path, out prefab))
        {
            prefab = Resources.Load<GameObject>(path);
            if (cache) prefabDic.Add(path, prefab);
        }
        return GameObject.Instantiate(prefab, pos, rot);
    }
    public GameObject LoadPrefab(string path, bool cache)
    {
        GameObject prefab = null;
        if (!prefabDic.TryGetValue(path, out prefab))
        {
            prefab = Resources.Load<GameObject>(path);
            if (cache) prefabDic.Add(path, prefab);
        }
        return GameObject.Instantiate(prefab);
    }

    private Dictionary<string, Sprite> spDic = new Dictionary<string, Sprite>();
    public Sprite LoadSprite(string path, bool cache)
    {
        Sprite sp = null;
        if (!spDic.TryGetValue(path, out sp))
        {
            sp = Resources.Load<Sprite>(path);
            if (cache) spDic.Add(path, sp);
        }

        return sp;
    }

    private string DeSerializeXML(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        if(textAsset == null)
        {
            NETCommon.Log("未加载到配置文件，Path: " + path);
            return null;
        }
        byte[] bytes = textAsset.bytes;

        using(MemoryStream rms = new MemoryStream(bytes))
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream gzs = new GZipStream(rms, CompressionMode.Decompress, true))
                {
                    byte[] buffer = new byte[1024];
                    int len = 0;
                    while((len = gzs.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, len);
                    }
                    
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }
    }

    #region InitConfigs
    #region 随机名字
    private string[] surnameAr;
    private string[] manAr;
    private string[] womanAr;

    private void InitRDNameCfg()
    {
        string xml = DeSerializeXML(PathDefine.RDNameCfg);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);

        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;

        List<string> surnameList = new List<string>();
        List<string> manList = new List<string>();
        List<string> womanList = new List<string>();

        for(int i = 0; i < nodeList.Count; i++)
        {
            XmlElement ele = nodeList[i] as XmlElement;
            if (ele.GetAttribute("ID") == null) continue;
            int ID = int.Parse(ele.GetAttributeNode("ID").InnerText);
            foreach(XmlElement e in nodeList[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "surname":
                        surnameList.Add(e.InnerText);
                        break;
                    case "man":
                        manList.Add(e.InnerText);
                        break;
                    case "woman":
                        womanList.Add(e.InnerText);
                        break;

                }
            }
        }

        surnameAr = surnameList.ToArray();
        manAr = manList.ToArray();
        womanAr = womanList.ToArray();
    }

    public string GetRDNameData(bool man = true)
    {
        System.Random rd = new System.Random();
        string rdName = surnameAr[UTools.RDInt(0, surnameAr.Length - 1)];
        if (man) rdName += manAr[UTools.RDInt(0, manAr.Length - 1)];
        else rdName += womanAr[UTools.RDInt(0, womanAr.Length - 1)];

        return rdName;
    }
    #endregion

    #region 地图
    private Dictionary<int, MapCfg> mapCfgDic = new Dictionary<int, MapCfg>();
    private void InitMapCfg()
    {
        string xml = DeSerializeXML(PathDefine.RDMapCfg);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);

        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;

        for (int i = 0; i < nodeList.Count; i++)
        {
            XmlElement ele = nodeList[i] as XmlElement;
            if (ele.GetAttribute("ID") == null) continue;

            int ID = int.Parse(ele.GetAttributeNode("ID").InnerText);
            MapCfg mc = new MapCfg { ID = ID };

            foreach (XmlElement e in nodeList[i].ChildNodes)
            {
                switch (e.Name)
                {

                    case "mapName":
                        mc.mapName = e.InnerText;
                        break;
                    case "sceneName":
                        mc.sceneName = e.InnerText;
                        break;
                    case "power":
                        mc.power = int.Parse(e.InnerText);
                        break;
                    case "mainCamPos":
                        {
                            string[] valArr = e.InnerText.Split(',');
                            mc.mainCamPos = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]), float.Parse(valArr[2]));
                        }
                        break;
                    case "mainCamRote":
                        {
                            string[] valArr = e.InnerText.Split(',');
                            mc.mainCamRote = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]), float.Parse(valArr[2]));
                        }
                        break;
                    case "playerBornPos":
                        {
                            string[] valArr = e.InnerText.Split(',');
                            mc.playerBornPos = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]), float.Parse(valArr[2]));
                        }
                        break;
                    case "playerBornRote":
                        {
                            string[] valArr = e.InnerText.Split(',');
                            mc.playerBornRote = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]), float.Parse(valArr[2]));
                        }
                        break;
                    case "monsterLst":
                        {
                            string[] wave = e.InnerText.Split('#');
                            List<List<MapCfg.MonsterData>> list = new List<List<MapCfg.MonsterData>>();
                            for(int w = 1; w < wave.Length; ++w)
                            {
                                if (string.IsNullOrEmpty(wave[w])) continue;
                                List<MapCfg.MonsterData> waveLst = new List<MapCfg.MonsterData>();
                                string[] mStr = wave[w].Split('|');
                                for(int m = 1; m < mStr.Length; ++m)
                                {
                                    if (string.IsNullOrEmpty(mStr[m])) continue;
                                    string[] minfoStr = mStr[m].Split(',');
                                    int mid = int.Parse(minfoStr[0]);
                                    int mwave = w;
                                    int mindex = m;
                                    float px = float.Parse(minfoStr[1]);
                                    float py = float.Parse(minfoStr[2]);
                                    float pz = float.Parse(minfoStr[3]);
                                    float yagnle = float.Parse(minfoStr[4]);
                                    int lv = int.Parse(minfoStr[5]);
                                    MapCfg.MonsterData mdata = new MapCfg.MonsterData(mid, mindex, mwave, new Vector3(px, py, pz), yagnle, lv);
                                    waveLst.Add(mdata);
                                }
                                list.Add(waveLst);
                            }
                            mc.monsterLst = list;
                        }
                        break;
                }
            }

            mapCfgDic.Add(ID, mc);
        }
    }
    public MapCfg GetMapCfgData(int id)
    {
        if (mapCfgDic.ContainsKey(id)) return mapCfgDic[id];
        else
        {
            NETCommon.Log("MapCfg获取失败", NETLogLevel.Error);
            return null;
        }
    }
    #endregion

    #region 自动指引
    private Dictionary<int, AutoGuideCfg> autoGuidDic = new Dictionary<int, AutoGuideCfg>();
    private void InitAutoGuidCfg()
    {
        string xml = DeSerializeXML(PathDefine.AutoGuidCfg);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);

        XmlNodeList nodeList = doc.SelectSingleNode("root").ChildNodes;
        for(int i = 0; i < nodeList.Count; i++)
        {
            var ele = nodeList[i] as XmlElement;
            if (ele.GetAttribute("ID") == null) continue;
            int id = int.Parse(ele.GetAttributeNode("ID").InnerText);
            AutoGuideCfg cfg = new AutoGuideCfg { ID = id };
            foreach(XmlElement e in nodeList[i].ChildNodes)
            {
                switch (e.Name)
                {
                    case "npcID":
                        cfg.npcID = int.Parse(e.InnerText);
                        break;
                    case "dilogArr":
                        cfg.dilogArr = e.InnerText;
                        break;
                    case "actID":
                        cfg.actID = int.Parse(e.InnerText);
                        break;
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

    #region 强化数据
    private Dictionary<int, Dictionary<int,StrongCfg>> strongCfgDic = new Dictionary<int, Dictionary<int,StrongCfg>>();
    private void InitStrongCfg()
    {
        string xml = DeSerializeXML(PathDefine.StrongCfg);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);

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
                    case "addhurt":
                        sc.addHurt = val;
                        break;
                }
            }

            Dictionary<int, StrongCfg> dic = null;
            if(strongCfgDic.TryGetValue(sc.pos, out dic))
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

    }
    public StrongCfg GetStrongCfgData(int pos, int starLv)
    {
        StrongCfg res = null;
        if (strongCfgDic.ContainsKey(pos))
        {
            Dictionary<int, StrongCfg> dic = strongCfgDic[pos];
            if (dic.ContainsKey(starLv)) return dic[starLv];
        }

        if(res == null) NETCommon.Log("StrongCfg获取失败", NETLogLevel.Error);

        return res;
    }
    public int GetPropAddValPreLv(int pos, int starLv, int type)
    {
        Dictionary<int, StrongCfg> posDic = null;
        int val = 0;

        if(strongCfgDic.TryGetValue(pos, out posDic))
        {
            for(int i = 0; i < starLv; i++)
            {
                StrongCfg sc;
                if(posDic.TryGetValue(i, out sc))
                {
                    switch (type)
                    {
                        case 0: //hp
                            val += sc.addHp;
                            break;
                        case 1: //hurt
                            val += sc.addHurt;
                            break;
                        case 2: // def
                            val += sc.addDef;
                            break;
                    }
                }
            }
        }

        return val;
    }
    #endregion

    #region 敏感词
    // DFA算法，实现敏感词过滤
    private Hashtable map;

    private void InitFilterWordCfg()
    {
        List<string> filterWords = new List<string>();
        // LoadCfg
        string xml = DeSerializeXML(PathDefine.FilterWordCfg);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
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

    #region 任务
    private Dictionary<int, TaskRewardCfg> taskRewardCfgDic = new Dictionary<int, TaskRewardCfg>();
    private void InitTaskCfg()
    {
        string xml = DeSerializeXML(PathDefine.TaskRewardCfg);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
        XmlNode root = doc.SelectSingleNode("root");
        foreach (XmlElement item in root.ChildNodes)
        {
            if (item.GetAttributeNode("ID") == null) continue;
            int id = int.Parse(item.GetAttributeNode("ID").InnerText);
            TaskRewardCfg cfg = new TaskRewardCfg();
            cfg.ID = id;
            foreach (XmlElement ele in item.ChildNodes)
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
        if (taskRewardCfgDic.TryGetValue(id, out cfg))
        {
            return cfg;
        }
        NETCommon.Log("获取 TaskRewardCfg 失败，ID：" + id);
        return null;
    }
    #endregion

    #region 技能
    private Dictionary<int, SkillCfg> skillCfgDic = new Dictionary<int, SkillCfg>();
    private void InitSkillCfg()
    {
        string xml = DeSerializeXML(PathDefine.SkillCfg);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
        XmlNode root = doc.SelectSingleNode("root");
        foreach (XmlElement item in root.ChildNodes)
        {
            if (item.GetAttributeNode("ID") == null) continue;
            int id = int.Parse(item.GetAttributeNode("ID").InnerText);
            SkillCfg cfg = new SkillCfg();
            cfg.ID = id;

            cfg.skillMove = new List<int>();

            foreach (XmlElement ele in item.ChildNodes)
            {
                string value = ele.InnerText;
                switch (ele.Name)
                {
                    case "skillName":
                        cfg.skillName = value;
                        break;
                    case "cdTime":
                        cfg.cdTime = int.Parse(value);
                        break;
                    case "skillTime":
                        cfg.skillTime = int.Parse(value);
                        break;
                    case "aniAction":
                        cfg.aninAction = int.Parse(value);
                        break;
                    case "fx":
                        cfg.fx = value;
                        break;
                    case "isCombo":
                        cfg.isCombo = value.Equals("1");
                        break;
                    case "isCollide":
                        cfg.isCollide = value.Equals("1");
                        break;
                    case "isBreak":
                        cfg.isBreak = value.Equals("1");
                        break;
                    case "dmgType":
                        cfg.dmgType = int.Parse(value);
                        break;
                    case "skillMoveLst":
                        string[] strs = value.Split('|');
                        for(int i = 0; i < strs.Length; i++)
                        {
                            if (strs[i] == "") continue;
                            cfg.skillMove.Add(int.Parse(strs[i]));
                        }
                        break;
                    case "skillActionLst":
                        Debug.Log("Init SkillAction Lst");
                        string[] astrs = value.Split('|');
                        cfg.skillAction = new List<int>();
                        for(int i = 0; i < astrs.Length; ++i)
                        {
                            if (string.IsNullOrEmpty(astrs[i])) continue;
                            cfg.skillAction.Add(int.Parse(astrs[i]));
                        }
                        break;
                    case "skillDamageLst":
                        string[] dstr = value.Split('|');
                        cfg.damageLst = new List<int>();
                        for(int i = 0; i < dstr.Length; ++i)
                        {
                            if (string.IsNullOrEmpty(dstr[i])) continue;
                            cfg.damageLst.Add(int.Parse(dstr[i]));
                        }
                        break;
                }
            }
            skillCfgDic.Add(id, cfg);
        }
    }
    public SkillCfg GetSkillCfg(int id)
    {
        SkillCfg cfg = null;
        if (skillCfgDic.TryGetValue(id, out cfg))
        {
            return cfg;
        }
        NETCommon.Log("获取 SkillCfg 失败，ID：" + id);
        return null;
    }
    #endregion

    #region 技能移动
    private Dictionary<int, SkillMoveCfg> skillMoveCfgDic = new Dictionary<int, SkillMoveCfg>();
    private void InitSkillMoveCfg()
    {
        string xml = DeSerializeXML(PathDefine.SkillMoveCfg);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
        XmlNode root = doc.SelectSingleNode("root");
        foreach (XmlElement item in root.ChildNodes)
        {
            if (item.GetAttributeNode("ID") == null) continue;
            int id = int.Parse(item.GetAttributeNode("ID").InnerText);
            SkillMoveCfg cfg = new SkillMoveCfg();
            cfg.ID = id;
            foreach (XmlElement ele in item.ChildNodes)
            {
                string value = ele.InnerText;
                switch (ele.Name)
                {
                    case "delayTime":
                        cfg.delayTime = int.Parse(value);
                        break;
                    case "moveTime":
                        cfg.moveTime = int.Parse(value);
                        break;
                    case "moveDis":
                        cfg.moveDis = float.Parse(value);
                        break;
                }
            }
            skillMoveCfgDic.Add(id, cfg);
        }
    }
    public SkillMoveCfg GetSkillMoveCfg(int id)
    {
        SkillMoveCfg cfg = null;
        if (skillMoveCfgDic.TryGetValue(id, out cfg))
        {
            return cfg;
        }
        NETCommon.Log("获取 SkillMoveCfg 失败，ID：" + id);
        return null;
    }
    #endregion

    #region 技能伤害
    private Dictionary<int, SkillActionCfg> skillActionCfgDic = new Dictionary<int, SkillActionCfg>();
    private void InitSkillActionCfg()
    {
        string xml = DeSerializeXML(PathDefine.SkillActionCfg);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
        XmlNode root = doc.SelectSingleNode("root");
        foreach (XmlElement item in root.ChildNodes)
        {
            if (item.GetAttributeNode("ID") == null) continue;
            int id = int.Parse(item.GetAttributeNode("ID").InnerText);
            SkillActionCfg cfg = new SkillActionCfg();
            cfg.ID = id;
            foreach (XmlElement ele in item.ChildNodes)
            {
                string value = ele.InnerText;
                switch (ele.Name)
                {
                    case "delayTime":
                        cfg.delayTime = int.Parse(value);
                        break;
                    case "radius":
                        cfg.radius = float.Parse(value);
                        break;
                    case "angle":
                        cfg.angle = int.Parse(value);
                        break;
                }
            }
            skillActionCfgDic.Add(id, cfg);
        }
    }
    public SkillActionCfg GetSkillActionCfg(int id)
    {
        SkillActionCfg cfg = null;
        if (skillActionCfgDic.TryGetValue(id, out cfg))
        {
            return cfg;
        }
        NETCommon.Log("获取 SkillActionCfg 失败，ID：" + id);
        return null;
    }
    #endregion

    #region 怪物
    private Dictionary<int, MonsterCfg> monsterCfgDic = new Dictionary<int, MonsterCfg>();
    private void InitMonsterCfg()
    {
        string xml = DeSerializeXML(PathDefine.MonsterCfg);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
        XmlNode root = doc.SelectSingleNode("root");
        foreach (XmlElement item in root.ChildNodes)
        {
            if (item.GetAttributeNode("ID") == null) continue;
            int id = int.Parse(item.GetAttributeNode("ID").InnerText);
            MonsterCfg cfg = new MonsterCfg();
            BattleProps props = new BattleProps();
            cfg.ID = id;
            cfg.props = props;
            foreach (XmlElement ele in item.ChildNodes)
            {
                string value = ele.InnerText;
                switch (ele.Name)
                {
                    case "mName":
                        cfg.name = value;
                        break;
                    case "resPath":
                        cfg.resPath = value;
                        break;
                    case "skillID":
                        cfg.skillId = int.Parse(value);
                        break;
                    case "isStop":
                        cfg.isStop = value.Equals("1");
                        break;
                    case "atkDis":
                        cfg.atkDis = float.Parse(value);
                        break;
                    case "hp":
                        cfg.props.hp = int.Parse(value);
                        break;
                    case "ad":
                        cfg.props.ad = int.Parse(value);
                        break;
                    case "ap":
                        cfg.props.ap = int.Parse(value);
                        break;
                    case "addef":
                        cfg.props.addef = int.Parse(value);
                        break;
                    case "apdef":
                        cfg.props.apdef = int.Parse(value);
                        break;
                    case "dodge":
                        cfg.props.dodge = int.Parse(value);
                        break;
                    case "pierce":
                        cfg.props.pierce = int.Parse(value);
                        break;
                    case "critical":
                        cfg.props.critical = int.Parse(value);
                        break;
                }
            }
            monsterCfgDic.Add(id, cfg);
        }
    }
    public MonsterCfg GetMonsterCfg(int id)
    {
        MonsterCfg cfg = null;
        if (monsterCfgDic.TryGetValue(id, out cfg))
        {
            return cfg;
        }
        NETCommon.Log("获取 MonsterCfg 失败，ID：" + id);
        return null;
    }
    #endregion

    #region PRD
    private Dictionary<int, int> PRDDic = new Dictionary<int, int>();
    private void InitPRDCfg()
    {
        string xml = DeSerializeXML(PathDefine.PRDCfg);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(xml);
        XmlNode root = doc.SelectSingleNode("root");
        foreach (XmlElement item in root.ChildNodes)
        {
            if (item.ChildNodes.Count < 2) continue;
            int p = int.Parse(item.ChildNodes[0].InnerText);
            int c = int.Parse(item.ChildNodes[1].InnerText);
            PRDDic[p] = c;
        }
    }
    public int GetPRD_C(int p)
    {
        if (p > 100) p = 100;
        int c = -1;
        if (PRDDic.TryGetValue(p, out c))
        {
            return c;
        }
        NETCommon.Log("获取 PRD_C 失败，P：" + p);
        return c;
    }
    #endregion
    #endregion
}
