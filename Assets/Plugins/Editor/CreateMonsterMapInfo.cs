/*********************************************************
	文件：CreateMonsterMapInfo
	作者：dell
	邮箱：630276388@qq.com
	日期：2020/9/2 9:19:03
	功能：待定
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System.IO;
using System;

public class CreateMonsterMapInfo : EditorWindow
{
    private static CreateMonsterMapInfo window;

    private string filename = "";
    private string infoStr = "";

    [MenuItem("Tools/CreateMonsterMapInfo")]
    static void ShowWindow()
    {
        window = GetWindow<CreateMonsterMapInfo>();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        GUILayout.Label("使用要求：\n" +
            "所有怪物相关物体应放在名称为 Monsters 的空物体下(位置 与 旋转 都为 0)\n" +
            "每一小关的怪物应该放在命名规范为 Level_关卡号 的空物体下(位置 与 旋转 都为 0)\n" +
            "例如 第一小关 的怪物 就应该放在名称为 Level_1 的空物体下\n" +
            "第二小关 就放在 Level_2 下\n" +
            "---------------------------------------------------------");
        EditorGUILayout.LabelField("输入要生成的配置文件名称(无后缀)：");
        filename = EditorGUILayout.TextField(filename);
        if (GUILayout.Button("生成配置文件"))
        {
            string path = EditorUtility.OpenFolderPanel("选择目标文件夹", "", "");
            CreateCfg(filename, path);
        }
        EditorGUILayout.LabelField(infoStr);
        EditorGUILayout.EndVertical();
    }

    private void CreateCfg(string name, string path)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("请输入 目标文件名！");
            return;
        }
        try
        {
            Transform root = GameObject.Find("Monsters").transform;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < root.childCount; i++)
            {
                sb.Append('#');
                Transform level = root.GetChild(i);
                for (int j = 0; j < level.childCount; j++)
                {
                    sb.Append('|');
                    Transform monster = level.GetChild(j);
                    GameObject source = PrefabUtility.GetCorrespondingObjectFromSource(monster.gameObject);
                    string resPath = AssetDatabase.GetAssetPath(source);
                    resPath = resPath.Replace(@"Assets/Resources/", "");
                    sb.Append(source.name + "," + monster.position.ToString() + "," + resPath + "," + monster.localEulerAngles.y.ToString());
                    sb.Append('\n');
                }
            }

            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
            sb.Clear();
            string target = path + @"\" + name + ".txt";
            using (FileStream stream = Directory.Exists(target) ? File.OpenWrite(target) : File.Create(target))
            {
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush();
                stream.Close();
            }
            infoStr = "生成完毕！请刷新一下目标文件夹";
        }
        catch(Exception e)
        {
            infoStr = "生成失败！报错：" + e.ToString();
        }
    }
}
