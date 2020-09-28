/*********************************************************
	文件：TipsWindow
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/13 8:56:32
	功能：待定
***********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicWindow : WindowRoot
{
    private Animation tipsAnim;
    private Text tipsText;
    private Queue<string> tipQue = new Queue<string>();
    private bool isTipsShow = false;

    private Transform hpItemRoot;
    private Dictionary<string, HPItem> hpDic = new Dictionary<string, HPItem>();

    private Animation playerDodgeAni;

    private void Awake()
    {
        Debug.Log("Dynamic Awake");
        tipsAnim = FindComponent<Animation>("CenterPin/textTips");
        tipsText = FindComponent<Text>("CenterPin/textTips");
        hpItemRoot = FindComponent<Transform>("LeftBottomPin/hpItemRoot");
        playerDodgeAni = FindComponent<Animation>("CenterPin/imgPlayerDodge");
    }

    protected override void InitWindow()
    {
        base.InitWindow();
        SetActive(tipsText, false);
    }

    #region Tips
    public void AddTips(string tips)
    {
        lock (tipQue) tipQue.Enqueue(tips);
    }

    private void SetTips(string tips)
    {
        isTipsShow = true;
        SetActive(tipsText, true);
        SetText(tipsText, tips);

        AnimationClip clip = tipsAnim.GetClip("TipsShowAnim");
        tipsAnim.Play();
        // 延迟关闭激活状态
        Invoke("AniPlayDone", clip.length);
    }
    private void AniPlayDone()
    {
        SetActive(tipsText, false);
        isTipsShow = false;
    }
    #endregion

    #region Battle
    public void AddHpItem(string name, int hp, Transform root)
    {
        GameObject go = resSev.LoadGoPrefab(PathDefine.HpItemPrefab, true, new Vector3(-1000f, 0, 0), Quaternion.identity);
        go.transform.SetParent(hpItemRoot);
        HPItem pItem = go.GetComponent<HPItem>();
        pItem.SetWindowState(true);
        hpDic[name] = pItem;
        pItem.SetItemInfo(hp, root);
    }

    public void SetDodge(string name)
    {
        HPItem pItem;
        if(hpDic.TryGetValue(name, out pItem))
        {
            pItem.SetDodge();
        }
    }

    public void SetDamage(string name, int damage)
    {
        HPItem pItem;
        if (hpDic.TryGetValue(name, out pItem))
        {
            pItem.SetDamage(damage);
        }
    }

    public void SetCritical(string name, int critical)
    {
        HPItem pItem;
        if (hpDic.TryGetValue(name, out pItem))
        {
            pItem.SetCritical(critical);
        }
    }

    public void SetHp(string name, int oldVal, int newVal)
    {
        HPItem pItem;
        if (hpDic.TryGetValue(name, out pItem))
        {
            pItem.SetHp(oldVal, newVal);
        }
    }

    public void RemoveMonster(string name)
    {
        HPItem pItem;
        if(hpDic.TryGetValue(name, out pItem))
        {
            hpDic.Remove(name);
            pItem.SetWindowState(false);
        }
    }

    public void SetPlayerDodge()
    {
        playerDodgeAni.Stop();
        playerDodgeAni.Play();
    }
    #endregion

    private void Update()
    {
        if(tipQue.Count > 0 && !isTipsShow)
        {
            lock (tipQue)
            {
                SetTips(tipQue.Dequeue());
            }
        }
    }
}
