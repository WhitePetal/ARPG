/*********************************************************
	文件：BuyWindow
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/22 14:52:01
	功能：购买交易窗口
***********************************************************/
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyWindow : WindowRoot
{
    private Text txtInfo;
    private int buyType;
    private Button btnConfirm;

    private void Awake()
    {
        txtInfo = FindComponent<Text>("CenterPin/bgTxt/txtInfo");
        btnConfirm = FindComponent<Button>("CenterPin/btnConfirm");
        btnConfirm.onClick.AddListener(ClickConfirmBtn);
        FindComponent<Button>("CenterPin/btnClose").onClick.AddListener(ClickCloseBtn);
    }

    protected override void InitWindow()
    {
        base.InitWindow();
        btnConfirm.interactable = true;
        RefreshUI();
    }

    public void SetBuyType(int type)
    {
        buyType = type;
    }

    private void RefreshUI()
    {
        switch (buyType)
        {
            case 0:
                // 购买体力
                txtInfo.text = "是否花费<color=#FF0000FF>10钻石</color>购买<color=green>100体力</color>？";
                break;
            case 1:
                // 购买金币
                txtInfo.text = "是否花费<color=#FF0000FF>10钻石</color>购买<color=green>1000金币</color>";
                break;
        }
    }

    private void ClickConfirmBtn()
    {
        audioSev.PlayUIAudio(Constans.UIClickBtnAudio);

        GameMsg msg = new GameMsg
        {
            cmd = (int)CMD.ReqBuy,
            reqBuy = new ReqBuy
            {
                type = buyType,
                cost = 10
            }
        };
        netSev.SendMsg(msg);
        btnConfirm.interactable = false;
    }

    private void ClickCloseBtn()
    {
        audioSev.PlayUIAudio(Constans.UIClickBtnAudio);
        SetWindowState(false);
    }
}
