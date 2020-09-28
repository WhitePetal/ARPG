/*********************************************************
	文件：GuidWindow
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/18 11:30:10
	功能：待定
***********************************************************/
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuidWindow : WindowRoot
{
    private Text txtName;
    private Text txtTalk;
    private Image imgIcon;
    private Button btnNext;
    private PlayerData pd;

    private AutoGuideCfg curTaskData;
    private string[] dialogArr;
    private int index;

    private void Awake()
    {
        txtName = FindComponent<Text>("BottomPin/bgTalk/bgDiallog/txtTalk/txtName");
        txtTalk = FindComponent<Text>("BottomPin/bgTalk/bgDiallog/txtTalk");
        imgIcon = FindComponent<Image>("BottomPin/imgIcon");
        btnNext = FindComponent<Button>("BottomPin/btnNext");

        btnNext.onClick.AddListener(ClickNextBtn);
    }

    protected override void InitWindow()
    {
        base.InitWindow();
        pd = GameRoot.Instance.PlayerData;
        curTaskData = MainCitySys.Instance.GetCurTaskData();
        dialogArr = curTaskData.dilogArr.Split('#');
        index = 1;

        SetTalk();
    }

    private void SetTalk()
    {
        string[] talkArr = dialogArr[index].Split('|');
        if(talkArr[0] == "0")
        {
            // 自己
            SetSprite(imgIcon, PathDefine.PlayerIcon);
            SetText(txtName, pd.name);
        }
        else
        {
            // NPC
            switch (curTaskData.npcID)
            {
                case 0:
                    SetSprite(imgIcon, PathDefine.WiseManIcon);
                    SetText(txtName, Constans.WiseManName);
                    break;
                case 1:
                    SetSprite(imgIcon, PathDefine.GeneralIcon);
                    SetText(txtName, Constans.GeneralName);
                    break;
                case 2:
                    SetSprite(imgIcon, PathDefine.ArtisanIcon);
                    SetText(txtName, Constans.ArtisanName);
                    break;
                case 3:
                    SetSprite(imgIcon, PathDefine.TraderIcon);
                    SetText(txtName, Constans.TraderName);
                    break;
                default:
                    SetSprite(imgIcon, PathDefine.GuideIcon);
                    SetText(txtName, Constans.GuidName);
                    break;
            }
        }

        imgIcon.SetNativeSize();
        SetText(txtTalk, talkArr[1].Replace("$name", pd.name));
    }

    private void ClickNextBtn()
    {
        audioSev.PlayUIAudio(Constans.UIClickBtnAudio);
        ++index;
        if (index == dialogArr.Length)
        {
            // 发送任务完成消息
            GameMsg msg = new GameMsg
            {
                cmd = (int)CMD.ReqGuided,
                reqGuide = new ReqGuide
                {
                    guidID = curTaskData.ID
                }
            };
            netSev.SendMsg(msg);

            SetWindowState(false);
            return;
        }
        SetTalk();
    }

}
