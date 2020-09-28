/*********************************************************
	文件：BattleMgr
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/8/30 8:49:32
	功能：战场管理类
***********************************************************/
using Protocol;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BattleMgr : MonoBehaviour
{
    private ResSev resSev;
    private AudioSev audioSev;

    private StateMgr stateMgr;
    private SkillMgr skillMgr;
    private MapMgr mapMgr;

    private MapCfg mapCfg;

    public EntityPlayer entityPlayer;
    private Dictionary<string, EntityMonster> monstersDic = new Dictionary<string, EntityMonster>();

    public void Init(int mapId)
    {
        resSev = ResSev.Instance;
        audioSev = AudioSev.Instance;

        stateMgr = gameObject.AddComponent<StateMgr>();
        skillMgr = gameObject.AddComponent<SkillMgr>();

        stateMgr.Init();
        skillMgr.Init();

        mapCfg = resSev.GetMapCfgData(mapId);
        resSev.AsyncLoadScene(mapCfg.sceneName, () =>
        {
            GameObject map = GameObject.FindGameObjectWithTag("MapRoot");
            mapMgr = map.GetComponent<MapMgr>();
            mapMgr.Init(this);

            map.transform.localPosition = Vector3.zero;
            map.transform.localScale = Vector3.one;

            Camera.main.transform.position = mapCfg.mainCamPos;
            Camera.main.transform.localEulerAngles = mapCfg.mainCamRote;

            LoadPlayer(mapCfg);

            audioSev.PlayBGM(Constans.HuangYeBGM);

            ActiveCurrentBatchMonsters();
        });
    }

    private void LoadPlayer(MapCfg mapCfg)
    {
        GameObject player = resSev.LoadGoPrefab(PathDefine.AssissBattlePlayerPrefab, true, mapCfg.playerBornPos, Quaternion.Euler(mapCfg.playerBornRote));
        player.transform.localScale = Vector3.one;

        entityPlayer = new EntityPlayer();
        PlayerController playerCtrl = player.GetComponent<PlayerController>();
        playerCtrl.BattleInit();

        PlayerData pd = GameRoot.Instance.PlayerData;
        BattleProps props = new BattleProps(pd.critical, pd.pierce, pd.dodge, pd.apdef, pd.addef, pd.ap, pd.ad, pd.hp);

        entityPlayer.SetBattleMgr(this);
        entityPlayer.SetLv(pd.lv);
        entityPlayer.SetController(playerCtrl);
        entityPlayer.SetStateMgr(stateMgr);
        entityPlayer.SetSkillMgr(skillMgr);
        entityPlayer.SetBattlePorps(props);

        entityPlayer.Idle();
    }

    #region 玩家角色控制
    public Vector2 GetDirInput()
    {
        return BattleSys.Instance.GetDirInput();
    }

    public void SetMoveDir(Vector2 dir)
    {
        if (entityPlayer == null || !entityPlayer.CanControl) return;
        if (entityPlayer.curState != AnimState.Idle && entityPlayer.curState != AnimState.Move) return;

        if(dir == Vector2.zero)
        {
            entityPlayer.Idle();
            entityPlayer.SetDir(Vector2.zero);
        }
        else
        {
            entityPlayer.Move();
            entityPlayer.SetDir(dir);
        }
    }

    public void ReleaseSkill(int skill)
    {
        switch (skill)
        {
            case 0:
                ReleaseNormalAttack();
                break;
            case 1:
                ReleaseSkill_1();
                break;
            case 2:
                ReleaseSkill_2();
                break;
            case 3:
                ReleaseSkill_3();
                break;
        }
    }

    private int[] comboAr = new int[] { 111, 112, 113, 114, 115 };
    private void ReleaseNormalAttack()
    {
        if (entityPlayer.curState == AnimState.Attack)
        {
            entityPlayer.Attack(comboAr[entityPlayer.nextCombo++], true);
        }   
        else if(entityPlayer.curState == AnimState.Idle || entityPlayer.curState == AnimState.Move)
        {
            entityPlayer.nextCombo = 0;
            entityPlayer.Attack(comboAr[entityPlayer.nextCombo++], true);
        }
        if (entityPlayer.nextCombo >= comboAr.Length) entityPlayer.nextCombo = 0;
        #region //
        //if(entityPlayer.curState == AnimState.Attack)
        //{
        //    double nowAtkTime = TimerSev.Instance.GetMillisecondsTime();
        //    if(lastAtkTime != -1d && nowAtkTime - lastAtkTime < Constans.ComboSpace)
        //    {
        //        if(comboIndex >= comboAr.Length)
        //        {
        //            comboIndex = 0;
        //            lastAtkTime = -1d;
        //            entityPlayer.nextCombo = -1;
        //            return;
        //        }
        //        entityPlayer.nextCombo = comboAr[comboIndex++];
        //        //entityPlayer.comboQue.Enqueue(comboAr[comboIndex++]);
        //    }        
        //    else entityPlayer.nextCombo = -1;

        //    lastAtkTime = nowAtkTime;
        //}
        //else if(entityPlayer.curState == AnimState.Idle || entityPlayer.curState == AnimState.Move)
        //{
        //    entityPlayer.nextCombo = -1;
        //    comboIndex = 0;
        //    entityPlayer.Attack(comboAr[comboIndex++]);
        //    lastAtkTime = TimerSev.Instance.GetMillisecondsTime();
        //}
        #endregion
    }
    private void ReleaseSkill_1()
    {
        entityPlayer.Attack(101);
    }
    private void ReleaseSkill_2()
    {
        entityPlayer.Attack(102);
    }
    private void ReleaseSkill_3()
    {
        entityPlayer.Attack(103);
    }
    #endregion

    public void LoadMonsterByWaveId(int wave)
    {
        var lst = mapCfg.monsterLst;
        var mlst = lst[wave];
        for(int i = 0; i < mlst.Count; ++i)
        {
            MonsterCfg mcfg = resSev.GetMonsterCfg(mlst[i].id);
            GameObject monster = resSev.LoadGoPrefab(mcfg.resPath, true, mlst[i].pos, Quaternion.Euler(0f, mlst[i].yAngle, 0f));
            StringBuilder sb = "m".ConnectStr(mlst[i].wave).ConnectStr('_').ConnectStr(mlst[i].index);
            monster.name = sb.EndConnectStr();
            monster.transform.localScale = Vector3.one;
            monster.SetActive(false);

            int lv = mlst[i].lv;
            BattleProps props = mcfg.props * lv;

            EntityMonster entity = new EntityMonster();
            entity.SetBattleMgr(this);
            entity.SetLv(lv);
            entity.SetController(monster.GetComponent<MonsterController>());
            entity.SetStateMgr(stateMgr);
            entity.SetSkillMgr(skillMgr);
            entity.SetBattlePorps(props);
            entity.SetMonsterCfg(mcfg);

            monstersDic.Add(monster.name, entity);

            GameRoot.Instance.dynamicWindow.AddHpItem(monster.name, entity.Hp, entity.GetHpRoot());

            monster.SetActive(false);
            entity.RegisterAILogic();
        }
    }

    private void ActiveCurrentBatchMonsters()
    {
        foreach(var pair in monstersDic)
        {
            pair.Value._gameObject.SetActive(true);
            pair.Value.Born();
        }
    }

    public EntityMonster[] GetEntityMonsters()
    {
        EntityMonster[] monsters = new EntityMonster[monstersDic.Count];
        int i = 0;
        foreach(var pair in monstersDic)
        {
            monsters[i++] = pair.Value;
        }
        return monsters;
    }

    public void RemoveMonster(string name)
    {
        EntityMonster monster;
        if(monstersDic.TryGetValue(name, out monster))
        {
            monstersDic.Remove(name);
            GameRoot.Instance.dynamicWindow.RemoveMonster(name);
        }
    }

    public bool CanRlsSkill()
    {
        return entityPlayer.canSkill;
    }
}
