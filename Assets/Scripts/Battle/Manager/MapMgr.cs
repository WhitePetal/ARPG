/*********************************************************
	文件：MapMgr
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/8/30 9:00:19
	功能：地图管理器
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMgr : MonoBehaviour
{
    private int waveIndex = 0;
    public void Init(BattleMgr battleMgr)
    {
        battleMgr.LoadMonsterByWaveId(waveIndex);
    }
}
