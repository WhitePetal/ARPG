/*********************************************************
	文件：PRDCalcC
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/9/3 9:19:06
	功能：计算PRD算法的C值
***********************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Threading;

public class PRDCalcC : EditorWindow
{
    private static readonly string obj = "lock";
    private static Dictionary<int, int> prdDic = new Dictionary<int, int>();
    private string infoStr = "";
    private string dataStr = "等待数据运算...";

    [MenuItem("Tools/PRD_C")]
    static void ShowWindow()
    {
        GetWindow<PRDCalcC>();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        if (GUILayout.Button("运算数据"))
        {
            dataStr = "数据运算中...";
            // 计算 1% - 100% 暴击率范围所有的 PRD C值
            for (int i = 0; i <= 100; ++i)
            {
                int j = i;
                // 创建线程负责具体计算 C 值
                Thread thread = new Thread(() =>
                {
                    double p = i * 1d / 100d; // 显示给玩家的暴击率
                    double c = CFromP(p); // PRD算法 暴击增量
                    int ic = (int)Math.Round(c * 1000, 0); // 将百分数小数转换为整数
                    lock (obj)
                    {
                        prdDic[j] = ic; // 计算结果存放在字典中
                    }
                });
                thread.Start();
            }
        }
        GUILayout.Label(dataStr);
        if (prdDic.Count == 101)
        {
            dataStr = "数据运算完毕";
            if (GUILayout.Button("点击生成配置文件"))
            {
                try
                {
                    CreateXml();
                    infoStr = "配置文件生成成功！";
                    dataStr = "等待数据运算...";
                }
                catch (Exception e)
                {
                    infoStr = "配置文件生成失败！错误为：" + e;
                }
            }  
        }

        GUILayout.Label(infoStr);

        EditorGUILayout.EndVertical();
    }

    // 生成 XML 文件
    private void CreateXml()
    {
        string path = EditorUtility.OpenFolderPanel("选择目标文件夹", "", "") + @"/prd.xml";
        StringBuilder sb = new StringBuilder();
        sb.Append(@"<?xml version=""1.0"" encoding=""UTF - 8"" standalone=""yes""?>");
        sb.Append('\n');
        sb.Append(@"<root xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">");
        sb.Append('\n');

        string xml = null;
        lock (obj)
        {
            // 在主线程中 从字典中拿出多线程放入的数据，进行解析
            foreach(var pair in prdDic)
            {
                sb.Append("<item>\n");
                sb.Append("    <p>" + pair.Key + "</p>\n");
                sb.Append("    <c>" + pair.Value + "</c>\n");
                sb.Append("</item>\n");
            }
            sb.Append("</root>");
            xml = sb.ToString();
            sb.Clear();
            xml.Remove(xml.Length - 1);
        }
        using(FileStream fs = Directory.Exists(path) ? File.OpenWrite(path) : File.Create(path))
        {
            byte[] bytes = Encoding.UTF8.GetBytes(xml);
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
            fs.Close();
        }
        lock (obj)
        {
            prdDic.Clear();
        }
    }

    // 根据 传入 C 值，计算该C值下，最小暴击范围的平均暴击率
    private static double PFromC(double c)
    {
        double dCurP = 0d;
        double dPreSuccessP = 0d;
        double dPE = 0;
        int nMaxFail = (int)Math.Ceiling(1d / c);
        for (int i = 1; i <= nMaxFail; ++i)
        {
            dCurP = Math.Min(1d, i * c) * (1 - dPreSuccessP);
            dPreSuccessP += dCurP;
            dPE += i * dCurP;
        }
        return 1d / dPE;
    }

    // 根据传入的暴击率，计算 PRD 算法中的系数 C
    private static double CFromP(double p)
    {
        double dUp = p;
        double dLow = 0d;
        double dMid = p;
        double dPLast = 1d;
        while (true)
        {
            dMid = (dUp + dLow) / 2d;
            double dPtested = PFromC(dMid);

            if (Math.Abs(dPtested - dPLast) <= 0.00005d) break;

            if (dPtested > p) dUp = dMid;
            else dLow = dMid;

            dPLast = dPtested;
        }

        return dMid;
    }
}
