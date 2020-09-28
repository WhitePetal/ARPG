/*********************************************************
	文件：PlayerCtrlWindow
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/8/30 14:33:31
	功能：战斗角色控制窗口
***********************************************************/
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrlWindow : WindowRoot
{
    private Text txtLevel;
    private Text txtName;

    private Text txtExpPrg;
    private GridLayoutGroup expGridGroup;
    private Image[] expItems;

    private Image imgTouch;
    private Image bgDir;
    private Image imgPoint;

    private Image imgSk1Cd;
    private Text txtSk1Cd;
    private bool isSk1Cd;
    private int sk1CdTime;

    private Image imgSk2Cd;
    private Text txtSk2Cd;
    private bool isSk2Cd;
    private int sk2CdTime;

    private Image imgSk3Cd;
    private Text txtSk3Cd;
    private bool isSk3Cd;
    private int sk3CdTime;

    private float sqrPointDis;
    private float pointDis;
    private Vector2 startToucPos;
    private Vector2 defaultPos;

    private Text txtHp;
    private Image imgHp;
    private int hp;

    public Vector2 curDir
    {
        get;
        private set;
    }

    private void Awake()
    {
        txtLevel = FindComponent<Text>("LeftTopPin/bgLv/txtLv");
        txtName = FindComponent<Text>("CenterPin/txtName");

        txtExpPrg = FindComponent<Text>("BottomPin/txtExp");
        expGridGroup = FindComponent<GridLayoutGroup>("BottomPin/bgExp/itemList");
        expItems = FindComponentsInChildren<Image>("BottomPin/bgExp/itemList");

        imgTouch = FindComponent<Image>("LeftBottomPin/imgTouch");
        bgDir = FindComponent<Image>("LeftBottomPin/imgTouch/bgDir");
        imgPoint = FindComponent<Image>("LeftBottomPin/imgTouch/bgDir/imgPoint");

        imgSk1Cd = FindComponent<Image>("RightBottomPin/btnSkill1/imgCd");
        txtSk1Cd = FindComponent<Text>("RightBottomPin/btnSkill1/imgCd/txtCd");

        imgSk2Cd = FindComponent<Image>("RightBottomPin/btnSkill2/imgCd");
        txtSk2Cd = FindComponent<Text>("RightBottomPin/btnSkill2/imgCd/txtCd");

        imgSk3Cd = FindComponent<Image>("RightBottomPin/btnSkill3/imgCd");
        txtSk3Cd = FindComponent<Text>("RightBottomPin/btnSkill3/imgCd/txtCd");

        txtHp = FindComponent<Text>("LeftTopPin/bgHp/txtSelfHp");
        imgHp = FindComponent<Image>("LeftTopPin/bgHp/imgSelfHp");

        sqrPointDis = Mathf.Pow(Screen.height * 1.0f / Constans.ScreenStandardHeight, 2) * Constans.SqrHandlePointDis;
        pointDis = Screen.height * 1.0f / Constans.ScreenStandardHeight * Constans.HandlePointDis;

        //RegisterTouchEvent();

        FindComponent<Button>("RightBottomPin/btnNormal").onClick.AddListener(ClickReleaseNormalAttack);
        FindComponent<Button>("RightBottomPin/btnSkill1").onClick.AddListener(ClickReleaseSkill_1);
        FindComponent<Button>("RightBottomPin/btnSkill2").onClick.AddListener(ClickReleaseSkill_2);
        FindComponent<Button>("RightBottomPin/btnSkill3").onClick.AddListener(ClickReleaseSkill_3);
    }

    protected override void InitWindow()
    {
        base.InitWindow();
        defaultPos = bgDir.transform.position;
        SetActive(bgDir, false);

        sk1CdTime = resSev.GetSkillCfg(101).cdTime;
        sk2CdTime = resSev.GetSkillCfg(102).cdTime;
        sk3CdTime = resSev.GetSkillCfg(103).cdTime;

        RefreshUI();
    }

    public void RefreshUI()
    {
        PlayerData pd = GameRoot.Instance.PlayerData;

        SetText(txtLevel, pd.lv);
        SetText(txtName, pd.name);
        hp = pd.hp;
        SetText(txtHp, string.Format("{0}/{1}", hp, hp));
        imgHp.fillAmount = 1f;

        #region ExpPrg
        int expPrgVal = (int)(pd.exp * 1.0f / NETCommon.GetExpUpValByLv(pd.lv) * 100);
        SetText(txtExpPrg, expPrgVal + "%");
        int index = expPrgVal / 10;

        float globalRate = 1f * Constans.ScreenStandardHeight / Screen.height;
        float screenWidth = globalRate * Screen.width;
        float itemWidht = (screenWidth - 181f) / 10f;
        expGridGroup.cellSize = new Vector2(itemWidht, 7f);

        for (int i = 0; i < expItems.Length; i++)
        {
            Image item = expItems[i];
            if (i < index) item.fillAmount = 1f;
            else if (i == index) item.fillAmount = (float)(expPrgVal % 10) / 10;
            else item.fillAmount = 0f;
        }
        #endregion
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector2 dir = new Vector2(h, v);
        if (dir != Vector2.zero) SetActive(bgDir, true);
        else SetActive(bgDir, false);
        if (dir.sqrMagnitude > sqrPointDis) dir = Vector2.ClampMagnitude(dir, pointDis);
        curDir = dir.normalized;
        BattleSys.Instance.SetMoveDir(curDir);
        imgPoint.transform.position = startToucPos + dir;

        if (Input.GetKeyDown(KeyCode.I)) ClickReleaseNormalAttack();
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
            curDir = Vector2.zero;
            BattleSys.Instance.SetMoveDir(curDir);
            SetActive(bgDir, false);
        });

        OnClickDrag(imgTouch, (evt) =>
        {
            Vector2 dir = evt.position - startToucPos;
            if (dir.sqrMagnitude > sqrPointDis) dir = Vector2.ClampMagnitude(dir, pointDis);
            curDir = dir.normalized;
            BattleSys.Instance.SetMoveDir(curDir);
            imgPoint.transform.position = startToucPos + dir;
        });
    }

    private void ClickReleaseNormalAttack()
    {
        if (!CanRlsSkill()) return;
        BattleSys.Instance.ReleaseSkill(0);
    }
    private void ClickReleaseSkill_1()
    {
        if (!CanRlsSkill()) return;
        if (!isSk1Cd)
        {
            BattleSys.Instance.ReleaseSkill(1);
            isSk1Cd = true;
            SetActive(imgSk1Cd, true);
            SetActive(txtSk1Cd, true);
            imgSk1Cd.fillAmount = 1f;
            txtSk1Cd.text = (sk1CdTime / 1000).ToString();
            int curTime = sk1CdTime;

            timerSev.AddTimerTask((tid) =>
            {
                curTime -= 100;
                if(curTime % 1000 == 0) txtSk1Cd.text = (curTime / 1000).ToString();
                imgSk1Cd.fillAmount = curTime * 1f / sk1CdTime;
                if(curTime <= 0)
                {
                    SetActive(imgSk1Cd, false);
                    SetActive(txtSk1Cd, false);
                    isSk1Cd = false;
                    timerSev.DeleteTimeTask(tid);
                }
            }, 100, sk1CdTime);
        }
    }
    private void ClickReleaseSkill_2()
    {
        if (!CanRlsSkill()) return;
        if (!isSk2Cd)
        {
            BattleSys.Instance.ReleaseSkill(2);
            isSk2Cd = true;
            SetActive(imgSk2Cd, true);
            SetActive(txtSk2Cd, true);
            imgSk2Cd.fillAmount = 1f;
            txtSk2Cd.text = (sk2CdTime / 1000).ToString();
            int curTime = sk2CdTime;

            timerSev.AddTimerTask((tid) =>
            {
                curTime -= 100;
                if (curTime % 1000 == 0) txtSk2Cd.text = (curTime / 1000).ToString();
                imgSk2Cd.fillAmount = curTime * 1f / sk2CdTime;
                if (curTime <= 0)
                {
                    SetActive(imgSk2Cd, false);
                    SetActive(txtSk2Cd, false);
                    isSk2Cd = false;
                    timerSev.DeleteTimeTask(tid);
                }
            }, 100, sk2CdTime);
        }
    }
    private void ClickReleaseSkill_3()
    {
        if (!CanRlsSkill()) return;
        if (!isSk3Cd)
        {
            BattleSys.Instance.ReleaseSkill(3);
            isSk3Cd = true;
            SetActive(imgSk3Cd, true);
            SetActive(txtSk3Cd, true);
            imgSk3Cd.fillAmount = 1f;
            txtSk3Cd.text = (sk3CdTime / 1000).ToString();
            int curTime = sk3CdTime;

            timerSev.AddTimerTask((tid) =>
            {
                curTime -= 100;
                if (curTime % 1000 == 0) txtSk3Cd.text = (curTime / 1000).ToString();
                imgSk3Cd.fillAmount = curTime * 1f / sk3CdTime;
                if (curTime <= 0)
                {
                    SetActive(imgSk3Cd, false);
                    SetActive(txtSk3Cd, false);
                    isSk3Cd = false;
                    timerSev.DeleteTimeTask(tid);
                }
            }, 100, sk3CdTime);
        }
    }

    public void SetPlayerHp(int curHp)
    {
        txtHp.text = string.Format("{0}/{1}", curHp, hp);
        imgHp.fillAmount = curHp * 1f / hp;
    }

    private bool CanRlsSkill()
    {
        return BattleSys.Instance.battleMgr.CanRlsSkill();
    }
}
