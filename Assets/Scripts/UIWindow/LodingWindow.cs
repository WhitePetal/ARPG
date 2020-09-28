/*********************************************************
	文件：LoginWindow
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/12 14:57:45
	功能：加载界面
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XLua;

public class LodingWindow : WindowRoot
{
    private Text txtTips;
    private Image imgFG;
    private Image imgPoint;
    private Text txtPrg;

    private float fgWidth;

    private void Awake()
    {
        txtTips = FindComponent<Text>("BottomPin/textTips");
        imgFG = FindComponent<Image>("BottomPin/lodingFG");
        imgPoint = FindComponent<Image>("BottomPin/lodingFG/imgPoint");
        txtPrg = FindComponent<Text>("BottomPin/lodingFG/txtPrg");
    }

    protected override void InitWindow()
    {
        base.InitWindow();

        fgWidth = imgFG.rectTransform.sizeDelta.x;
        SetText(txtTips, "这是一条游戏Tips");
        SetText(txtPrg, "0%");
        imgFG.fillAmount = 0f;
        imgPoint.transform.localPosition = new Vector3(-fgWidth / 2f, 0f, 0f);
    }

    public void SetProgress(float prg)
    {
        txtPrg.text = (int)(prg * 100) + "%";
        imgFG.fillAmount = prg;
        float posX = -fgWidth / 2f + prg * fgWidth;
        imgPoint.transform.localPosition = new Vector3(posX, 0f, 0f);
    }
}
