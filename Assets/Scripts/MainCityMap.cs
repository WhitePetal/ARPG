/*********************************************************
	文件：MainCityMap
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/18 9:21:20
	功能：待定
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCityMap : MonoBehaviour
{
    [SerializeField] private Transform NPCLayer = null;
    [HideInInspector] public Transform[] NPCPosTrans;

    private void Awake()
    {
        Transform npcPos = NPCLayer.Find("NPCPos");
        NPCPosTrans = new Transform[npcPos.childCount];
        for(int i = 0; i < NPCPosTrans.Length; i++)
        {
            NPCPosTrans[i] = npcPos.GetChild(i);
        }
    }
}
