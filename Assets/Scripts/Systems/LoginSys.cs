/*********************************************************
	文件：LoginSys
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/12 14:32:04
	功能：登录入口
***********************************************************/
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginSys : SystemBase<LoginSys>
{
    private LoginWindow loginWindow;
    private CreateWindow createWindow;

    public override void InitSys()
    {
        base.InitSys();
        Debug.Log("InitSys...");
        loginWindow = GameRoot.Instance.transform.Find("Canvas/LoginWindow").GetComponent<LoginWindow>();
        createWindow = GameRoot.Instance.transform.Find("Canvas/CreateWindow").GetComponent<CreateWindow>();
    }

    /// <summary>
    /// 进入登录场景
    /// </summary>
    public void EnterLogin()
    {
        resSev.AsyncLoadScene(Constans.SceneLogin, OpenLoginWindow);
    }

    public void OpenLoginWindow()
    {
        loginWindow.SetWindowState(true);
        audioSev.PlayBGM(Constans.LoginBGM);
    }

    public void RespLogin(GameMsg msg)
    {
        GameRoot.AddTips("登录成功");
        GameRoot.Instance.SetPlayerData(msg.rspLogin);

        if(msg.rspLogin.playerData.name == "")
        {
            // 打开角色创建面板
            createWindow.SetWindowState();
        }
        else
        {
            // 进入主城
            MainCitySys.Instance.EnterMainCity();
        }


        // 关闭登录面板
        loginWindow.SetWindowState(false);
    }

    public void RspRename(GameMsg msg)
    {
        GameRoot.Instance.SetPlayerName(msg.rspRename.name);

        // 跳转场景进入主城
        // 打开主城的界面
        MainCitySys.Instance.EnterMainCity();

        // 关闭创建界面
        createWindow.SetWindowState(false);
    }
}
