/*********************************************************
	文件：CreateWindow
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/13 10:34:40
	功能：角色创建界面
***********************************************************/
using Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateWindow : WindowRoot
{
    private InputField iptName;
    private Button btnRd;
    private Button btnEnter;

    private void Awake()
    {
        iptName = FindComponent<InputField>("RightPin/iptName");
        btnRd = FindComponent<Button>("RightPin/iptName/btnRand");
        btnEnter = FindComponent<Button>("RightPin/btnEnter");

        btnRd.onClick.AddListener(ClickRandBtn);
        btnEnter.onClick.AddListener(ClickEnterBtn);
    }

    protected override void InitWindow()
    {
        base.InitWindow();

        // TODO
        // 显示一个随机名字
        iptName.text = resSev.GetRDNameData(false);
    }

    private void ClickRandBtn()
    {
        audioSev.PlayUIAudio(Constans.UIClickBtnAudio);
        iptName.text = resSev.GetRDNameData(false);
    }

    private void ClickEnterBtn()
    {
        audioSev.PlayUIAudio(Constans.UIClickBtnAudio);

        string _name = iptName.text;
        if(_name != "")
        {
            //TODO
            // 发送名字数据到服务器
            netSev.SendMsg(new GameMsg
            {
                cmd = (int)CMD.ReqRename,
                reqRename = new ReqRename
                {
                    name = _name
                }
            });
        }
        else
        {
            GameRoot.AddTips("当前名称不符合规范！");
        }
    }
}
