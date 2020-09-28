/*********************************************************
	文件：InitPlayerTest
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/16 18:55:47
	功能：待定
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitPlayerTest : MonoBehaviour
{

    private void Awake()
    {
        //MapCfg data = ResSev.Instance.GetMapCfgData(Constans.MainCityID);
        //LoadPlayer(data);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    private void LoadPlayer(MapCfg mapData)
    {
        Camera.main.transform.position = mapData.mainCamPos;
        Camera.main.transform.localEulerAngles = mapData.mainCamRote;

        Vector3 pos = mapData.playerBornPos;
        Vector3 angle = mapData.playerBornRote;
        Quaternion rot = Quaternion.Euler(angle);
        GameObject player = ResSev.Instance.LoadGoPrefab(PathDefine.AssissCityPlayerPrefab, true, pos, rot);
        player.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
