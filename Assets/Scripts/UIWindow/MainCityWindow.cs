/*********************************************************
	文件：MainCityWindow
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/15 15:17:20
	功能：主城UI界面
***********************************************************/
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCityWindow : WindowRoot
{
    #region Define
    private Text txtFight;
    private Text txtPower;
    private Image imgPowerPrg;
    private Text txtLevel;
    private Text txtName;

    private Text txtExpPrg;
    private GridLayoutGroup expGridGroup;
    private Image[] expItems;

    private Button btnGuid;

    private Animation menuAnim;
    private bool menuState = true;

    private Image imgTouch;
    private Image bgDir;
    private Image imgPoint;

    private float sqrPointDis;
    private float pointDis;
    private Vector2 startToucPos;
    private Vector2 defaultPos;

    private AutoGuideCfg curTaskData;

    #endregion

    private void Awake()
    {
        txtFight = FindComponent<Text>("LeftTopPin/bgFight/txtFight");
        txtPower = FindComponent<Text>("LeftTopPin/bgPower/txtPower");
        imgPowerPrg = FindComponent<Image>("LeftTopPin/bgPower/bgPowerPrg");
        txtLevel = FindComponent<Text>("LeftTopPin/bgLv/txtLv");
        txtName = FindComponent<Text>("CenterPin/txtName");
        txtExpPrg = FindComponent<Text>("BottomPin/txtExp");
        expGridGroup = FindComponent<GridLayoutGroup>("BottomPin/bgExp/itemList");
        expItems = FindComponentsInChildren<Image>("BottomPin/bgExp/itemList");
        menuAnim = FindComponent<Animation>("RightBottomPin/menuRoot");
        imgTouch = FindComponent<Image>("LeftBottomPin/imgTouch");
        bgDir = FindComponent<Image>("LeftBottomPin/imgTouch/bgDir");
        imgPoint = FindComponent<Image>("LeftBottomPin/imgTouch/bgDir/imgPoint");

        Button btnMenu = FindComponent<Button>("RightBottomPin/menuRoot/btnMenu");
        Button btnHead = FindComponent<Button>("LeftTopPin/btnHead");
        btnGuid = FindComponent<Button>("RightTopPin/btnGuid");
        Button btnStrong = FindComponent<Button>("RightBottomPin/menuRoot/btnStrong");

        sqrPointDis = Mathf.Pow(Screen.height * 1.0f / Constans.ScreenStandardHeight, 2) * Constans.SqrHandlePointDis;
        pointDis = Screen.height * 1.0f / Constans.ScreenStandardHeight * Constans.HandlePointDis;

        FindComponent<Button>("BottomPin/btnChat").onClick.AddListener(ClickChatBtn);
        FindComponent<Button>("LeftTopPin/bgPower/btnUpPower").onClick.AddListener(ClickBuyPowerBtn);
        FindComponent<Button>("RightBottomPin/menuRoot/btnMKCoin").onClick.AddListener(ClickMKCoinBtn);
        FindComponent<Button>("RightBottomPin/menuRoot/btnTask").onClick.AddListener(ClickTaskBtn);
        FindComponent<Button>("RightBottomPin/menuRoot/btnArena").onClick.AddListener(ClickMissionBtn);
        btnStrong.onClick.AddListener(ClickStrongBtn);
        btnGuid.onClick.AddListener(ClickGuidBtn);
        btnMenu.onClick.AddListener(ClickMenuBtn);
        btnHead.onClick.AddListener(ClickHeadBtn);

        RegisterTouchEvent();
    }

    protected override void InitWindow()
    {
        base.InitWindow();

        defaultPos = bgDir.transform.position;
        SetActive(bgDir, false);
        
        RefreshUI();
    }

    public void RefreshUI()
    {
        PlayerData pd = GameRoot.Instance.PlayerData;

        SetText(txtFight, NETCommon.GetFightByProps(pd));
        int powerLimit = NETCommon.GetPowerLimit(pd.lv);
        SetText(txtPower, "体力：" + pd.power + "/" + powerLimit);
        imgPowerPrg.fillAmount = pd.power * 1.0f / powerLimit;
        SetText(txtLevel, pd.lv);
        SetText(txtName, pd.name);

        #region ExpPrg
        int expPrgVal = (int)(pd.exp * 1.0f / NETCommon.GetExpUpValByLv(pd.lv) * 100);
        SetText(txtExpPrg, expPrgVal + "%");
        int index = expPrgVal / 10;

        float globalRate = 1f * Constans.ScreenStandardHeight / Screen.height;
        float screenWidth = globalRate * Screen.width;
        float itemWidht = (screenWidth - 181f) / 10f;
        expGridGroup.cellSize = new Vector2(itemWidht, 7f);

        for(int i = 0; i < expItems.Length; i++)
        {
            Image item = expItems[i];
            if (i < index) item.fillAmount = 1f;
            else if (i == index) item.fillAmount = (float)(expPrgVal % 10) / 10;
            else item.fillAmount = 0f;
        }
        #endregion

        // 设置自动任务图标
        curTaskData = resSev.GetAutoGuidCfg(pd.guidid);
        if(curTaskData != null)
        {
            SetGuideBtnIcon(curTaskData.npcID);
        }
    }

    private void SetGuideBtnIcon(int npcId)
    {
        string iconPath = "";
        Image img = btnGuid.GetComponent<Image>();
        switch (npcId)
        {
            case Constans.NpcWiseMan:
                iconPath = PathDefine.WiseManHead;
                break;
            case Constans.NpcGeneral:
                iconPath = PathDefine.GeneraHead;
                break;
            case Constans.NpcArtisan:
                iconPath = PathDefine.ArtisanHead;
                break;
            case Constans.NpcTrader:
                iconPath = PathDefine.TraderHead;
                break;
            default:
                iconPath = PathDefine.TaskHead;
                break;
        }

        SetSprite(img, iconPath);
        
    }

    private void ClickGuidBtn()
    {
        audioSev.PlayUIAudio(Constans.UIClickBtnAudio);
        if (curTaskData != null)
        {
            MainCitySys.Instance.RunTask(curTaskData);
        }
        else GameRoot.AddTips("更多引导任务，正在开发中...");
    }

    private void ClickMenuBtn()
    {
        audioSev.PlayUIAudio(Constans.UIExtenBtn);
        menuState = !menuState;
        string clip = menuState ? "MenuCloseAnim" : "MenuShowAnim";
        menuAnim.Play(clip);
    }

    private void ClickHeadBtn()
    {
        audioSev.PlayUIAudio(Constans.UIOpenPage);
        MainCitySys.Instance.OpenInfoWindow();
        MainCitySys.Instance.StopTask();
    }

    private void ClickStrongBtn()
    {
        audioSev.PlayUIAudio(Constans.UIOpenPage);
        MainCitySys.Instance.OpenStrongWindow();
        MainCitySys.Instance.StopTask();
    }

    private void ClickChatBtn()
    {
        audioSev.PlayUIAudio(Constans.UIClickBtnAudio);
        MainCitySys.Instance.OpenChatWindow();
    }

    private void RegisterTouchEvent()
    {
        OnClickDown(imgTouch, (evt) =>
        {
            bgDir.transform.position = evt.position;
            startToucPos = evt.position;
            SetActive(bgDir, true);
        });

        OnClickUp(imgTouch, (evt) =>
        {
            bgDir.transform.position = defaultPos;
            imgPoint.transform.localPosition = Vector3.zero;
            MainCitySys.Instance.SetPlayerMove(Vector2.zero);
            SetActive(bgDir, false);
        });

        OnClickDrag(imgTouch, (evt) =>
        {
            Vector2 dir = evt.position - startToucPos;
            if (dir.sqrMagnitude > sqrPointDis) dir = Vector2.ClampMagnitude(dir, pointDis);
            MainCitySys.Instance.SetPlayerMove(dir.normalized);
            imgPoint.transform.position = startToucPos + dir;
        });
    }

    private void ClickBuyPowerBtn()
    {
        audioSev.PlayUIAudio(Constans.UIClickBtnAudio);
        MainCitySys.Instance.OpenBuyWindow(0);
    }

    private void ClickMKCoinBtn()
    {
        audioSev.PlayUIAudio(Constans.UIClickBtnAudio);
        MainCitySys.Instance.OpenBuyWindow(1);
    }

    private void ClickTaskBtn()
    {
        audioSev.PlayUIAudio(Constans.UIClickBtnAudio);
        MainCitySys.Instance.OpenTaskWindow();
    }

    private void ClickMissionBtn()
    {
        audioSev.PlayUIAudio(Constans.UIClickBtnAudio);
        MainCitySys.Instance.EnterMission();
    }
}
