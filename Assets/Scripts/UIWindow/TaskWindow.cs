/*********************************************************
	文件：TaskWindow
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/8/28 11:03:38
	功能：待定
***********************************************************/
using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskWindow : WindowRoot
{
    private PlayerData pd = null;
    private Transform scrollTrans = null;

    private TaskRewardData[] taskrewardDatas = new TaskRewardData[6];

    private void Awake()
    {
        scrollTrans = FindComponent<Transform>("Panel/MainContent/ScrollView/Viewport/Content");
        FindComponent<Button>("Panel/MainContent/btnClose").onClick.AddListener(ClickCloseBtn);
    }

    protected override void InitWindow()
    {
        base.InitWindow();
        pd = GameRoot.Instance.PlayerData;
        RefreshUI();
    }

    protected override void ClearWindow()
    {
        base.ClearWindow();
        pd = null;
    }

    public void RefreshUI()
    {
        for(int i = 0; i < scrollTrans.childCount; i++)
        {
            Destroy(scrollTrans.GetChild(i).gameObject);
        }
        List<TaskRewardData> todoList = new List<TaskRewardData>();
        List<TaskRewardData> doneList = new List<TaskRewardData>();

        // 1|0|0
        for(int i = 0; i < pd.taskArr.Length; i++)
        {
            TaskRewardData data = new TaskRewardData();
            string[] strar = pd.taskArr[i].Split('|');
            data.ID = int.Parse(strar[0]);
            data.prgs = int.Parse(strar[1]);
            data.take = int.Parse(strar[2]) == 1;

            if (data.take) doneList.Add(data);
            else todoList.Add(data);
        }

        todoList.Sort();
        todoList.CopyTo(taskrewardDatas);
        doneList.CopyTo(0, taskrewardDatas, todoList.Count, doneList.Count);

        for(int i = 0; i < taskrewardDatas.Length; i++)
        {
            GameObject taskItem = resSev.LoadPrefab(PathDefine.TaskItemPrefab, true);
            taskItem.transform.SetParent(scrollTrans);

            TaskRewardData data = taskrewardDatas[i];
            TaskRewardCfg cfg = resSev.GetTaskRewardCfg(data.ID);

            Text txtName = taskItem.transform.Find("txtName").GetComponent<Text>();
            Text txtPrg = taskItem.transform.Find("txtPrg").GetComponent<Text>();
            Image imgPrg = taskItem.transform.Find("prgBar/imgPrg").GetComponent<Image>();
            Text txtExp = taskItem.transform.Find("txtExp").GetComponent<Text>();
            Text txtCoin = taskItem.transform.Find("txtCoin").GetComponent<Text>();
            Button btnTake = taskItem.transform.Find("btnTake").GetComponent<Button>();
            Image imgComp = taskItem.transform.Find("imgComp").GetComponent<Image>();

            txtName.text = cfg.taskName;
            txtPrg.text = data.prgs.ToString() + '/' + cfg.count;
            txtExp.text = "奖励：  经验" + cfg.exp.ToString();
            txtCoin.text = "金币" + cfg.coin.ToString();

            imgPrg.fillAmount = (float)data.prgs / cfg.count;


            SetActive(imgComp, false);
            btnTake.interactable = false;
            if (data.prgs == cfg.count && !data.take)
            {
                btnTake.onClick.AddListener(() =>
                {
                    ClickTakeBtn(data.ID);
                });
                btnTake.interactable = true;
            }
            else if(data.take)
            {
                SetActive(imgComp, true);
            }
        }
    }

    private void ClickTakeBtn(int id)
    {
        netSev.SendMsg(new GameMsg
        {
            cmd = (int)CMD.ReqTakeTaskReward,
            reqTakeTask = new ReqTakeTaskReward
            {
                id = id
            }
        });
    }

    private void ClickCloseBtn()
    {
        audioSev.PlayUIAudio(Constans.UIClickBtnAudio);
        SetWindowState(false);
    }
}
