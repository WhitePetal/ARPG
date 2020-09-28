/*********************************************************
	文件：LoginWindow
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/12 15:27:45
	功能：登录注册界面
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Protocol;

public class LoginWindow : WindowRoot
{
    private InputField iptAcct;
    private InputField iptPass;

    protected override void InitWindow()
    {
        base.InitWindow();

        iptAcct = FindComponent<InputField>("RightPin/iptBG1/iptAccount");
        iptPass = FindComponent<InputField>("RightPin/iptBG2/iptPass");
        Button btnEnter = FindComponent<Button>("RightPin/btnEnter");
        Button btnNotice = FindComponent<Button>("btnNoice");

        btnEnter.onClick.AddListener(ClickEnterBtn);
        btnNotice.onClick.AddListener(ClickNoticeBtn);

        // 获取本地存储的账号密码
        if (PlayerPrefs.HasKey("Acct") && PlayerPrefs.HasKey("Pass"))
        {
            iptAcct.text = PlayerPrefs.GetString("Acct");
            iptPass.text = PlayerPrefs.GetString("Pass");
        }
        else
        {
            iptAcct.text = "";
            iptPass.text = "";
        }
    }

    /// <summary>
    /// 点击进入游戏
    /// </summary>
    public void ClickEnterBtn()
    {
        audioSev.PlayUIAudio(Constans.UILoginBtnAudio);

        string acct = iptAcct.text;
        string pass = iptPass.text;

        if(acct != "" && pass != "")
        {
            // 更新存储本地的账号密码
            PlayerPrefs.SetString("Acct", acct);
            PlayerPrefs.SetString("Pass", pass);

            // TODO 发送网络消息，请求登录
            netSev.SendMsg(new GameMsg
            {
                cmd = (int)CMD.ReqLogin,
                reqLogin = new ReqLogin
                {
                    account = acct,
                    pass = pass
                }
            });
            // TO Remove
            //LoginSys.Instance.RespLogin();
        }
        else
        {
            GameRoot.AddTips("账号或密码不能为空！");
        }
    }


    public void ClickNoticeBtn()
    {
        audioSev.PlayUIAudio(Constans.UIClickBtnAudio);
        GameRoot.AddTips("功能开发中...");
    }
}
