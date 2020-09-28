/*********************************************************
	文件：BaseData
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/16 13:33:23
	功能：配置数据类
***********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseData<T>
{
    public int ID;
}

public class MapCfg : BaseData<MapCfg>
{
    public string mapName;
    public string sceneName;
    public int power;
    public Vector3 mainCamPos;
    public Vector3 mainCamRote;
    public Vector3 playerBornPos;
    public Vector3 playerBornRote;

    public List<List<MonsterData>> monsterLst;

    public struct MonsterData
    {
        public int id;
        public int index;
        public int wave;
        public Vector3 pos;
        public float yAngle;
        public int lv;

        public MonsterData(int id, int index, int wave, Vector3 pos, float yAngle, int lv)
        {
            this.id = id;
            this.index = index;
            this.wave = wave;
            this.pos = pos;
            this.yAngle = yAngle;
            this.lv = lv;
        }
    }
}

public class AutoGuideCfg : BaseData<AutoGuideCfg>
{
    public int npcID;
    public string dilogArr;
    public int actID;
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

public class TaskRewardData : BaseData<TaskRewardData>, IComparable<TaskRewardData>
{
    public int prgs;
    public bool take;

    public int CompareTo(TaskRewardData other)
    {
        return -prgs + other.prgs;
    }
}

public class SkillCfg : BaseData<SkillCfg>
{
    public string skillName;
    public int cdTime;
    public int skillTime;
    public int aninAction;
    public string fx;
    public bool isCombo;
    public bool isCollide;
    public bool isBreak;
    public int dmgType;

    public List<int> skillMove;
    public List<int> skillAction;

    public List<int> damageLst;
}

public class SkillMoveCfg : BaseData<SkillMoveCfg>
{
    public int delayTime;
    public int moveTime;
    public float moveDis; 
}

public class MonsterCfg : BaseData<MonsterCfg>
{
    public string name;
    public string resPath;
    public int skillId;
    public float atkDis;
    public bool isStop;
    public BattleProps props;
}

public class SkillActionCfg : BaseData<SkillActionCfg>
{
    public int delayTime;
    public float radius;
    public int angle;
}

public class BattleProps
{
    public int critical;
    public int pierce;
    public int dodge;
    public int apdef;
    public int addef;
    public int ap;
    public int ad;
    public int hp;

    public int pierceN = 1;
    public int dodgeN = 1;
    public int criticalN = 1;

    public BattleProps(int critical, int pierce, int dodge, int apdef, int addef, int ap, int ad, int hp)
    {
        this.critical = critical;
        this.pierce = pierce;
        this.dodge = dodge;
        this.apdef = apdef;
        this.addef = addef;
        this.ap = ap;
        this.ad = ad;
        this.hp = hp;
    }
    public BattleProps() { }

    public static BattleProps operator *(BattleProps m, int lv)
    {
        BattleProps res = new BattleProps(m.critical * lv, m.pierce * lv, m.dodge * lv, m.apdef * lv, m.addef * lv, m.ap * lv, m.ad * lv, m.hp * lv);
        return res;
    }
}