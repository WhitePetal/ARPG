/*********************************************************
	文件：StrongWindow
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/19 10:59:56
	功能：强化界面
***********************************************************/
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrongWindow : WindowRoot
{
    #region UI
    private Image imgIcon;
    private Text txtStarLv;
    private Transform starTransGroup;
    private Text propHP1;
    private Text propHurt1;
    private Text propDef1;
    private Text propHP2;
    private Text propHurt2;
    private Text propDef2;
    private Image propArr1;
    private Image propArr2;
    private Image propArr3;

    private Transform costInfo;
    private Text txtNeedLv;
    private Text txtCostCoin;
    private Text txtCostCrystal;

    private Text txtCoin;
    #endregion

    private PlayerData playerData;
    private int curIndex;
    private StrongCfg nextSc;

    private void Awake()
    {
        #region UI
        imgIcon = FindComponent<Image>("bg/mainContent/infoBG/starBG/imgCur");
        txtStarLv = FindComponent<Text>("bg/mainContent/infoBG/starBG/txtStarLV");
        starTransGroup = FindComponent<Transform>("bg/mainContent/infoBG/starGroup");
        txtCoin = FindComponent<Text>("bg/mainContent/infoBG/coinBG/txtCoin");
        propHP1 = FindComponent<Text>("bg/mainContent/infoBG/prop0/val0");
        propHurt1 = FindComponent<Text>("bg/mainContent/infoBG/prop1/val0");
        propDef1 = FindComponent<Text>("bg/mainContent/infoBG/prop2/val0");
        propHP2 = FindComponent<Text>("bg/mainContent/infoBG/prop0/val1");
        propHurt2 = FindComponent<Text>("bg/mainContent/infoBG/prop1/val1");
        propDef2 = FindComponent<Text>("bg/mainContent/infoBG/prop2/val1");
        propArr1 = FindComponent<Image>("bg/mainContent/infoBG/prop0/imgAr");
        propArr2 = FindComponent<Image>("bg/mainContent/infoBG/prop1/imgAr");
        propArr3 = FindComponent<Image>("bg/mainContent/infoBG/prop2/imgAr");
        costInfo = FindComponent<Transform>("bg/mainContent/infoBG/costInfoBG");
        txtNeedLv = FindComponent<Text>("bg/mainContent/infoBG/costInfoBG/txtNeedLV");
        txtCostCoin = FindComponent<Text>("bg/mainContent/infoBG/costInfoBG/txtCostCoin");
        txtCostCrystal = FindComponent<Text>("bg/mainContent/infoBG/costInfoBG/txtCostCrystal");
        #endregion

        Button btnClose = FindComponent<Button>("bg/mainContent/btnClose");
        btnClose.onClick.AddListener(ClickCloseBtn);

        FindComponent<Button>("bg/mainContent/infoBG/btnStrong").onClick.AddListener(ClickStrongBtn);
        RegsClickEvents();
    }

    protected override void InitWindow()
    {
        base.InitWindow();
        playerData = GameRoot.Instance.PlayerData;
        RefreshItem(3);
    }

    private void RegsClickEvents()
    {
        Button[] btnItems = FindComponentsInChildren<Button>("bg/mainContent/btnGroup");
        for(int i = 0; i < btnItems.Length; i++)
        {
            int j = i;
            btnItems[j].onClick.AddListener(() => {
                audioSev.PlayUIAudio(Constans.UIClickBtnAudio);
                ClickBtnItem(btnItems, j);
            });
        }
    }
    private void ClickBtnItem(Button[] btns, int index)
    {
        for(int i = 0; i < btns.Length; i++)
        {
            btns[i].image.sprite = resSev.LoadSprite(PathDefine.StrongNotSelectBG, true);
        }
        btns[index].image.sprite = resSev.LoadSprite(PathDefine.StrongSelectBG, true);

        RefreshItem(index);
    }

    private void RefreshItem(int index)
    {
        curIndex = index;
        // 金币
        SetText(txtCoin, playerData.coin);
        switch (index)
        {
            case 0:
                SetSprite(imgIcon, PathDefine.IconToukui);
                break;
            case 1:
                SetSprite(imgIcon, PathDefine.IconShenti);
                break;
            case 2:
                SetSprite(imgIcon, PathDefine.IconYaobu);
                break;
            case 3:
                SetSprite(imgIcon, PathDefine.IconShoubu);
                break;
            case 4:
                SetSprite(imgIcon, PathDefine.IconTuibu);
                break;
            case 5:
                SetSprite(imgIcon, PathDefine.IconJiaobu);
                break;
        }
        SetText(txtStarLv, playerData.strongArr[index] + "星级");

        int curStarLv = playerData.strongArr[index];
        for(int i = 0; i < starTransGroup.childCount; i++)
        {
            Image img = starTransGroup.GetChild(i).GetComponent<Image>();
            Sprite sprite = i < curStarLv ? resSev.LoadSprite(PathDefine.star2, true) : resSev.LoadSprite(PathDefine.star1, true);
            img.sprite = sprite;
        }

        int nextStarLv = curStarLv + 1;

        int hasAddHp = resSev.GetPropAddValPreLv(index, nextStarLv, 0);
        int hasAddHurt = resSev.GetPropAddValPreLv(index, nextStarLv, 1);
        int hasAddDef = resSev.GetPropAddValPreLv(index, nextStarLv, 2);
        SetText(propHP1, "生命 +" + hasAddHp);
        SetText(propHurt1, "伤害 +" + hasAddHurt);
        SetText(propDef1, "防御 +" + hasAddDef);

        StrongCfg nextSc = resSev.GetStrongCfgData(index, nextStarLv);
        this.nextSc = nextSc;
        if(nextSc != null)
        {
            #region Show
            SetActive(propHP2, true);
            SetActive(propHurt2, true);
            SetActive(propDef2, true);

            SetActive(propArr1, true);
            SetActive(propArr2, true);
            SetActive(propArr3, true);

            SetActive(costInfo, true);
            #endregion
            SetText(propHP2, "强化后 +" + nextSc.addHp);
            SetText(propHurt2, "强化后 +" + nextSc.addHurt);
            SetText(propDef2, "强化后 +" + nextSc.addDef);

            SetText(txtNeedLv,"需要等级：" +  nextSc.minLv);
            SetText(txtCostCoin,"需要消耗：         " + nextSc.coin);
            SetText(txtCostCrystal, "需要消耗：         " + playerData.crystal + "/" + nextSc.crystal);
        }
        else
        {
            #region Hiden
            SetActive(propHP2, false);
            SetActive(propHurt2, false);
            SetActive(propDef2, false);

            SetActive(propArr1, false);
            SetActive(propArr2, false);
            SetActive(propArr3, false);

            SetActive(costInfo, false);
            #endregion
        }
    }

    public void RefreshUI()
    {
        RefreshItem(curIndex);
    }

    private void ClickStrongBtn()
    {
        audioSev.PlayUIAudio(Constans.UIClickBtnAudio);

        if (playerData.strongArr[curIndex] < 10)
        {
            if(playerData.lv < nextSc.minLv)
            {
                GameRoot.AddTips("角色等级不足");
                return;
            }

            if(playerData.coin < nextSc.coin)
            {
                GameRoot.AddTips("金币不足");
                return;
            }

            if(playerData.crystal < nextSc.crystal)
            {
                GameRoot.AddTips("水晶不足");
                return;
            }

            netSev.SendMsg(new GameMsg
            {
                cmd = (int)CMD.ReqStrong,
                reqStrong = new ReqStrong
                {
                    pos = curIndex
                }
            });
        }
        else
        {
            GameRoot.AddTips("星级已满");
        }
    }

    private void ClickCloseBtn()
    {
        audioSev.PlayUIAudio(Constans.UIClickBtnAudio);
        SetWindowState(false);
    }
}
