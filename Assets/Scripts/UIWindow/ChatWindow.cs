/*********************************************************
	文件：ChatWindow
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/21 11:18:37
	功能：聊天窗口
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Protocol;
using BJTimer;

public class ChatWindow : WindowRoot
{
    private Text txtChat;
    private InputField iptChat;
    private int curChatType;
    private bool canSend = true;

    private Queue<string> chatQue = new Queue<string>();
    private StringBuilder chatSB = new StringBuilder();



    private void Awake()
    {
        txtChat = FindComponent<Text>("BottomPin/txtBG/txtChat");
        iptChat = FindComponent<InputField>("BottomPin/iptChat");

        FindComponent<Button>("BottomPin/txtBG/btnClose").onClick.AddListener(ClickCloseBtn);
        FindComponent<Button>("BottomPin/btnSend").onClick.AddListener(ClickSendBtn);

        Button[] chatBtns = new Button[3];
        chatBtns[0] = FindComponent<Button>("BottomPin/txtBG/btnWorld");
        chatBtns[1] = FindComponent<Button>("BottomPin/txtBG/btnGuild");
        chatBtns[2] = FindComponent<Button>("BottomPin/txtBG/btnFrend");
        for(int i = 0; i < chatBtns.Length; i++)
        {
            int j = i;
            chatBtns[i].onClick.AddListener(() => 
            {
                ClickChatBtn(j, chatBtns);
            });
        }
    }

    protected override void InitWindow()
    {
        base.InitWindow();
        RefreshUI(curChatType);
    }

    public void AddChatMsg(string name, string chat)
    {
        int len = chat.Length;
        name = Constans.ColorStr(name, TxtColor.Blue);
        string txt = name + ": " + chat;
        if(len <= 10)
        {
            txt = Constans.SizeStr(txt, 25);
        }
        else
        {
            int size = -len + 35;
            txt = Constans.SizeStr(txt, size);
        }
        chatQue.Enqueue(txt);
        if (chatQue.Count > 12) chatQue.Dequeue();
        if(gameObject.activeSelf) RefreshUI(curChatType);
    }

    private void ClickChatBtn(int index, Button[] btns)
    {
        curChatType = index;
        for (int k = 0; k < 3; k++)
        {
            SetSprite(btns[k].image, PathDefine.BtnType2);
        }
        SetSprite(btns[index].image, PathDefine.BtnType1);

        RefreshUI(index);
    }

    private void RefreshUI(int index)
    {
        switch (index)
        {
            case 0:
                while(chatQue.Count > 0)
                {
                    chatSB.Append(chatQue.Dequeue());
                    chatSB.Append('\n');
                }
                SetText(txtChat, chatSB.ToString());
                if (chatSB.Length > 4096) chatSB.Clear();
                break;
            case 1:
                SetText(txtChat, "尚未加入公会");
                break;
            case 2:
                SetText(txtChat, "暂无好友信息");
                break;
        }
    }

    private void ClickCloseBtn()
    {
        audioSev.PlayUIAudio(Constans.UIClickBtnAudio);

        SetWindowState(false);
    }

    private void ClickSendBtn()
    {
        if (!canSend)
        {
            GameRoot.AddTips("消息发送太频繁啦！请稍后再试");
            return;
        }
        if (iptChat.text == null || iptChat.text == "" || iptChat.text == " ")
        {
            GameRoot.AddTips("未输入聊天信息");
            return;
        }

        if(iptChat.text.Length  > 20)
        {
            GameRoot.AddTips("输入信息不能超过20个字符");
            return;
        }
        string txt = resSev.SerachFilterWordAndReplace(iptChat.text);
        Debug.Log("GM TXT: " + txt);
        iptChat.text = "";
        netSev.SendMsg(new GameMsg
        {
            cmd = (int)CMD.SndChat,
            sndChat = new SndChat { txt = txt}
        });

        canSend = false;
        timerSev.AddTimerTask((id) => { canSend = true; }, 5, 1, TimeUnit.Secound);
    }
}
