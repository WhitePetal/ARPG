/*********************************************************
	文件：InfoWindow
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/17 11:14:25
	功能：角色信息展示
***********************************************************/
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoWindow : WindowRoot
{
    private Text txtInfo;
    private Text txtExp;
    private Image imgExpPrg;
    private Text txtPower;
    private Image imgPowerPrg;
    private Text txtJob;
    private Text txtFight;
    private Text txtHp;
    private Text txtHurt;
    private Text txtDef;

    private Transform detilPanel;

    private Button btnCloseDetil;
    private Button btnDetial;
    private Button btnClose;
    private RawImage imgChar;

    private Text dtxtHp;
    private Text dtxtAd;
    private Text dtxtAp;
    private Text dtxtAdDef;
    private Text dtxtApDef;
    private Text dtxtDodge;
    private Text dtxtPierce;
    private Text dtxtCritical;

    private Vector2 lastTouchPos;

    private void Awake()
    {
        #region 主面板角色信息组件
        imgChar = FindComponent<RawImage>("charBG/charShow");
        txtInfo = FindComponent<Text>("charBG/infoBG/txtInfo");
        txtExp = FindComponent<Text>("minContent/valitItem0/barBG/txtPrg");
        imgExpPrg = FindComponent<Image>("minContent/valitItem0/barBG/imgPrg");
        txtPower = FindComponent<Text>("minContent/valitItem1/barBG/txtPrg");
        imgPowerPrg = FindComponent<Image>("minContent/valitItem1/barBG/imgPrg");
        txtJob = FindComponent<Text>("minContent/valitItem2/txt");
        txtFight = FindComponent<Text>("minContent/valitItem3/txt");
        txtHp = FindComponent<Text>("minContent/valitItem4/txt");
        txtHurt = FindComponent<Text>("minContent/valitItem5/txt");
        txtDef = FindComponent<Text>("minContent/valitItem6/txt");
        #endregion

        #region 详细属性面板组件
        detilPanel = transform.Find("DetilBG");
        SetActive(detilPanel, false);

        dtxtHp = FindComponent<Text>("DetilBG/group/propertyItem0/txtVal");
        dtxtAd = FindComponent<Text>("DetilBG/group/propertyItem1/txtVal");
        dtxtAp = FindComponent<Text>("DetilBG/group/propertyItem2/txtVal");
        dtxtAdDef = FindComponent<Text>("DetilBG/group/propertyItem3/txtVal");
        dtxtApDef = FindComponent<Text>("DetilBG/group/propertyItem4/txtVal");
        dtxtDodge = FindComponent<Text>("DetilBG/group/propertyItem5/txtVal");
        dtxtPierce = FindComponent<Text>("DetilBG/group/propertyItem6/txtVal");
        dtxtCritical = FindComponent<Text>("DetilBG/group/propertyItem7/txtVal");
        #endregion

        #region Button
        btnClose = FindComponent<Button>("btnClose");
        btnDetial = FindComponent<Button>("minContent/btnDetil");
        btnCloseDetil = FindComponent<Button>("DetilBG/btnClose");

        btnClose.onClick.AddListener(ClickCloseBtn);
        btnDetial.onClick.AddListener(ClickDetailBtn);
        btnCloseDetil.onClick.AddListener(ClickCloseDetialBtn);
        #endregion


        RegisterTouchEvent();
    }

    protected override void InitWindow()
    {
        base.InitWindow();

        RefreshUI();
    }

    private void RegisterTouchEvent()
    {
        OnClickDown(imgChar, (evt) =>
        {
            lastTouchPos = evt.position;
            MainCitySys.Instance.SetStartRotate();
        });

        OnClickDrag(imgChar, (evt) =>
        {
            float rotate = evt.position.x - lastTouchPos.x;
            MainCitySys.Instance.SetPlayerRotate(-rotate * 0.4f);
            lastTouchPos = evt.position;
        });
    }

    private void RefreshUI()
    {
        PlayerData pd = GameRoot.Instance.PlayerData;
        SetText(txtInfo, pd.name.ConnectStr("  LV.").ConnectStr(pd.lv).EndConnectStr());
        int lvVal = NETCommon.GetExpUpValByLv(pd.lv);
        SetText(txtExp, pd.exp.ConnectStr('/').ConnectStr(lvVal).EndConnectStr());
        imgExpPrg.fillAmount = pd.exp * 1f / lvVal;
        int powerVal = NETCommon.GetPowerLimit(pd.lv);
        SetText(txtPower, pd.power.ConnectStr('/').ConnectStr(powerVal).EndConnectStr());
        imgPowerPrg.fillAmount = pd.power * 1f / powerVal;

        SetText(txtJob, "职业    暗夜刺客");
        SetText(txtFight, "战力    ".ConnectStr(NETCommon.GetFightByProps(pd)).EndConnectStr());
        SetText(txtHp, "血量    ".ConnectStr(pd.hp).EndConnectStr());
        SetText(txtHurt, "伤害    ".ConnectStr(pd.ad + pd.ap).EndConnectStr());
        SetText(txtDef, "防御    ".ConnectStr(pd.addef + pd.apdef).EndConnectStr());

        // 刷新细节
        RefreshDetailPanel();
    }

    private void RefreshDetailPanel()
    {
        PlayerData pd = GameRoot.Instance.PlayerData;
        SetText(dtxtHp, pd.hp);
        SetText(dtxtAd, pd.ad);
        SetText(dtxtAp, pd.ap);
        SetText(dtxtAdDef, pd.addef);
        SetText(dtxtApDef, pd.apdef);
        SetText(dtxtDodge, pd.dodge.ConnectStr('%').EndConnectStr());
        SetText(dtxtPierce, pd.pierce.ConnectStr('%').EndConnectStr());
        SetText(dtxtCritical, pd.critical.ConnectStr('%').EndConnectStr());
    }

    private void ClickCloseBtn()
    {
        audioSev.PlayUIAudio(Constans.UIClickBtnAudio);
        MainCitySys.Instance.CloseInfoWindow();
    }

    private void ClickDetailBtn()
    {
        audioSev.PlayUIAudio(Constans.UIClickBtnAudio);
        SetActive(detilPanel, true);
    }
    private void ClickCloseDetialBtn()
    {
        audioSev.PlayUIAudio(Constans.UIClickBtnAudio);
        SetActive(detilPanel, false);
    }
}
