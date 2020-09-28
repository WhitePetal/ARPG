/*********************************************************
	文件：ScriptsInfoRecoder
	作者：白菊
	邮箱：630276388@qq.com
	日期：#CREATETIME#
	功能：记录脚本信息
***********************************************************/
using System;
using System.IO;
using UnityEngine;

public class ScriptsInfoRecoder : UnityEditor.AssetModificationProcessor
{
    private static void OnWillCreateAsset(string path)
    {
        path = path.Replace(".meta", "");
        if (path.EndsWith(".cs"))
        {
            string str = File.ReadAllText(path);
            str = str.Replace("#PLANE#", Environment.UserName)
                .Replace("#CREATETIME#", DateTime.Now.ToString());
            File.WriteAllText(path, str);
        }
    }
}
