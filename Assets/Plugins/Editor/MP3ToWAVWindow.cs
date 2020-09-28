/*********************************************************
	文件：MP3ToWAVWindow
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/13 12:43:00
	功能：MP3 转 WAV 工具
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using NAudio.Wave;
using UnityEditorInternal;
using System.IO;

public class MP3ToWAVWindow : EditorWindow
{
    private static MP3ToWAVWindow window;

    private ReorderableList reoclipList;
    private List<AudioClip> data;

    [MenuItem("Tools/MP3ToWAV")]
    static void ShowWindow()
    {
        window = (MP3ToWAVWindow)GetWindow(typeof(MP3ToWAVWindow));
    }

    private void OnEnable()
    {
        data = new List<AudioClip>();
        reoclipList = new ReorderableList(data, typeof(AudioClip));

        reoclipList.onAddCallback = (list) =>
        {
            data.Add(null);
        };
        reoclipList.onRemoveCallback = (list) =>
        {
            data.RemoveAt(data.Count - 1);
        };
        reoclipList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            data[index] = (AudioClip)EditorGUI.ObjectField(rect, "Mp3 Audio", data[index], typeof(AudioClip), false);
        };
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        reoclipList.DoLayoutList();


        if (GUILayout.Button("转换"))
        {
            foreach (AudioClip mp3Audio in data)
            {
                string curPath = AssetDatabase.GetAssetPath(mp3Audio);
                if (!curPath.EndsWith(".mp3"))
                {
                    Debug.LogError("AudioClip 格式不合法，请重新输入Clip");
                    return;
                }
                int t = curPath.LastIndexOf('/');
                string dir = curPath.Substring(0, t) + "/WAV";
                string targtPath = curPath.Insert(t, "/WAV").Replace(".mp3", ".wav");
                using (var reader = new Mp3FileReader(curPath))
                {
                    WaveFileWriter.CreateWaveFile(targtPath, reader);
                }
            }

        }

        if (GUILayout.Button("转换文件夹下Audio"))
        {
            string foldPath = EditorUtility.OpenFolderPanel("选择文件夹", "", "");
            DirectoryInfo di = new DirectoryInfo(foldPath);
            FileInfo[] files = di.GetFiles();
            for(int i = 0; i < files.Length; i++)
            {
                string name = files[i].Name;
                if (!name.EndsWith(".mp3")) continue;
                string curPath = foldPath + "/" + name;
                string targtDir = foldPath + "/WAV/";
                string targtPath = targtDir + name.Replace(".mp3", ".wav");
                if (!Directory.Exists(targtDir)) Directory.CreateDirectory(targtDir);
                using (var reader = new Mp3FileReader(curPath))
                {
                    WaveFileWriter.CreateWaveFile(targtPath, reader);
                }
            }
        }

        string autoStr = AudioImporter.auto ? "关闭 《Audio 导入时自动转换格式》 模式" : "开启 《Audio 导入时自动转换格式》 模式";
        if (GUILayout.Button(autoStr))
        {
            AudioImporter.auto = !AudioImporter.auto;
        }

        EditorGUILayout.EndVertical();
    }
}
