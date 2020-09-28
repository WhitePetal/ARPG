/*********************************************************
	文件：AudioImporter
	作者：Administrator
	邮箱：630276388@qq.com
	日期：2020/8/13 13:37:09
	功能：音频导入预设器
***********************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NAudio.Wave;
using System.IO;

public class AudioImporter : AssetPostprocessor
{
    public static bool auto = false;

    void OnPreprocessAudio()
    {
        if (!auto) return;
        if (assetPath.EndsWith(".mp3"))
        {
            int t = assetPath.LastIndexOf('/');
            string dir = assetPath.Substring(0, t) + "/WAV";
            string targtPath = assetPath.Insert(t, "/WAV").Replace(".mp3", ".wav");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using (var reader = new Mp3FileReader(assetPath))
            {
                WaveFileWriter.CreateWaveFile(targtPath, reader);
            }
        }
    }
}
